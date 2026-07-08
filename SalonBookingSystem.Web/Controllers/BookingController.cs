using Microsoft.AspNetCore.Mvc;
using SalonBookingSystem.Web.Models;
using SalonBookingSystem.Web.Services;

namespace SalonBookingSystem.Web.Controllers;

/// <summary>
/// Customer self-service booking — 3-step wizard.
/// Step 1 (Index)        : Select services
/// Step 2 (SelectBarber) : Choose barber, date, available slot
/// Step 3 (Confirm)      : Review and confirm
/// All protected actions redirect to Login if CustomerId not in session.
/// </summary>
public class BookingController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<BookingController> _logger;

    public BookingController(IApiService apiService, ILogger<BookingController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // ── Step 1: Select services ───────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = await BuildWizardAsync(step: 1);
        return View(vm);
    }

    // ── Step 2: Choose barber, date, time slot ────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> SelectBarber(
        [FromQuery] string? serviceIds,
        [FromQuery] int? barberId,
        [FromQuery] string? date,
        [FromQuery] string? time)
    {
        var parsedIds = ParseIds(serviceIds);
        if (parsedIds.Count == 0)
            return RedirectToAction(nameof(Index));

        var vm = await BuildWizardAsync(
            step: 2,
            selectedServiceIds: parsedIds,
            selectedBarberId: barberId,
            selectedDate: date,
            selectedTime: time);

        // Fetch available slots only when barber + date are both chosen
        if (barberId.HasValue && !string.IsNullOrEmpty(date))
        {
            var duration = vm.TotalDurationMinutes;
            if (duration > 0)
            {
                try
                {
                    var slots = await _apiService.GetAsync<ApiResponseViewModel<List<string>>>(
                        $"appointments/available-slots?barberId={barberId}&appointmentDate={date}&durationMinutes={duration}");
                    vm.AvailableSlots = slots?.Data ?? new List<string>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to fetch available slots");
                }
            }
        }

        return View(vm);
    }

    // ── Step 3: Confirm ───────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Confirm(
        [FromQuery] string? serviceIds,
        [FromQuery] int barberId,
        [FromQuery] string? date,
        [FromQuery] string? time)
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
        {
            TempData["InfoMessage"] = "Please login to confirm your booking.";
            TempData["ReturnUrl"] = Url.Action("Confirm", new { serviceIds, barberId, date, time });
            return RedirectToAction("Login", "Auth");
        }

        var parsedIds = ParseIds(serviceIds);
        if (parsedIds.Count == 0 || barberId == 0
            || string.IsNullOrEmpty(date) || string.IsNullOrEmpty(time))
            return RedirectToAction(nameof(Index));

        var vm = await BuildWizardAsync(
            step: 3,
            selectedServiceIds: parsedIds,
            selectedBarberId: barberId,
            selectedDate: date,
            selectedTime: time);

        vm.CustomerId = customerId.Value;
        return View(vm);
    }

    // ── POST: Book ────────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(
        [FromForm] string? serviceIds,
        [FromForm] int barberId,
        [FromForm] string? date,
        [FromForm] string? time)
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
            return RedirectToAction("Login", "Auth");

        var parsedIds = ParseIds(serviceIds);

        try
        {
            var request = new
            {
                CustomerId = customerId.Value,
                BarberId = barberId,
                AppointmentDate = date,
                StartTime = time,
                ServiceIds = parsedIds
            };

            // POST /api/v1/appointments — requires CustomerAuthorization (cookie forwarded)
            var response = await _apiService.PostAsync<ApiResponseViewModel<AppointmentViewModel>>(
                "appointments", request);

            if (response?.Success == true)
            {
                TempData["SuccessMessage"] = "Your appointment has been confirmed!";
                _logger.LogInformation("Customer {CustomerId} booked appointment", customerId);
                return RedirectToAction(nameof(MyAppointments));
            }

            var error = response?.Message ?? "Booking failed. Please try again.";
            if (response?.Errors?.Count > 0)
                error = string.Join(", ", response.Errors);

            TempData["ErrorMessage"] = error;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Booking failed for customer {CustomerId}", customerId);
            TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
        }

        return RedirectToAction(nameof(Confirm),
            new { serviceIds, barberId, date, time });
    }

    // ── My Appointments ───────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> MyAppointments()
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
            return RedirectToAction("Login", "Auth");

        try
        {
            // GET /api/v1/appointments/customer/{id} — requires CustomerAuthorization
            var response = await _apiService.GetAsync<ApiResponseViewModel<List<AppointmentViewModel>>>(
                $"appointments/customer/{customerId}");

            return View(response?.Data ?? new List<AppointmentViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load appointments for customer {CustomerId}", customerId);
            TempData["ErrorMessage"] = "Failed to load your appointments.";
            return View(new List<AppointmentViewModel>());
        }
    }

    // ── Cancel ────────────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
            return RedirectToAction("Login", "Auth");

        try
        {
            // POST /api/v1/appointments/{id}/cancel — requires CustomerAuthorization
            var response = await _apiService.PostAsync<ApiResponseViewModel<object>>(
                $"appointments/{id}/cancel", new { });

            if (response?.Success == true)
                TempData["SuccessMessage"] = "Appointment cancelled successfully.";
            else
                TempData["ErrorMessage"] = response?.Message ?? "Failed to cancel appointment.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cancel failed for appointment {Id}", id);
            TempData["ErrorMessage"] = "An unexpected error occurred.";
        }

        return RedirectToAction(nameof(MyAppointments));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<BookingViewModel> BuildWizardAsync(
        int step,
        List<int>? selectedServiceIds = null,
        int? selectedBarberId = null,
        string? selectedDate = null,
        string? selectedTime = null)
    {
        List<BarberViewModel> barbers = new();
        List<ServiceViewModel> services = new();

        try
        {
            // Both endpoints have no auth — no cookie needed
            var barbersTask = _apiService.GetAsync<ApiResponseViewModel<PagedDataViewModel<BarberViewModel>>>(
                "barbers?pageSize=100");
            var servicesTask = _apiService.GetAsync<ApiResponseViewModel<PagedDataViewModel<ServiceViewModel>>>(
                "services?pageSize=100");

            await Task.WhenAll(barbersTask, servicesTask);

            barbers = (barbersTask.Result?.Data?.Data ?? new List<BarberViewModel>())
                .Where(b => b.IsActive).ToList();
            services = servicesTask.Result?.Data?.Data ?? new List<ServiceViewModel>();

            _logger.LogInformation(
                "Wizard loaded: {BarberCount} barbers, {ServiceCount} services",
                barbers.Count, services.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load barbers/services for booking wizard");
        }

        return new BookingViewModel
        {
            Step = step,
            Barbers = barbers,
            Services = services,
            SelectedServiceIds = selectedServiceIds ?? new List<int>(),
            SelectedBarberId = selectedBarberId,
            SelectedDate = selectedDate,
            SelectedTime = selectedTime
        };
    }

    private static List<int> ParseIds(string? ids)
    {
        if (string.IsNullOrWhiteSpace(ids)) return new List<int>();
        return ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(s => int.TryParse(s.Trim(), out var n) ? n : 0)
                  .Where(n => n > 0)
                  .ToList();
    }
}
