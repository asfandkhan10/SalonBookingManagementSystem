using Microsoft.AspNetCore.Mvc;
using SalonBookingSystem.Web.Models;
using SalonBookingSystem.Web.Services;

namespace SalonBookingSystem.Web.Controllers;

/// <summary>
/// Receptionist module — manage customers, view/create/complete appointments.
/// Thin controller: all business logic stays in the API.
/// </summary>
public class ReceptionistController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<ReceptionistController> _logger;

    public ReceptionistController(IApiService apiService, ILogger<ReceptionistController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // ── Customers ────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Customers()
    {
        try
        {
            var response = await _apiService.GetAsync<ApiResponseViewModel<PagedDataViewModel<CustomerViewModel>>>(
                "customers?pageSize=100");
            return View(response?.Data?.Data ?? new List<CustomerViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load customers");
            TempData["ErrorMessage"] = "Failed to load customers.";
            return View(new List<CustomerViewModel>());
        }
    }

    // ── Appointments ─────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Appointments()
    {
        try
        {
            var response = await _apiService.GetAsync<ApiResponseViewModel<List<AppointmentViewModel>>>(
                "appointments");
            return View(response?.Data ?? new List<AppointmentViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load appointments");
            TempData["ErrorMessage"] = "Failed to load appointments.";
            return View(new List<AppointmentViewModel>());
        }
    }

    // ── Create Appointment ───────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> CreateAppointment()
    {
        var vm = await BuildReceptionistBookingVmAsync();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAppointment(ReceptionistBookingViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Reload lists before re-rendering the form
            var reloaded = await BuildReceptionistBookingVmAsync();
            model.Barbers = reloaded.Barbers;
            model.Services = reloaded.Services;
            ViewBag.Customers = reloaded.Customers();
            return View(model);
        }

        try
        {
            var request = new
            {
                CustomerId = model.CustomerId,
                BarberId = model.BarberId,
                AppointmentDate = model.AppointmentDate.ToString("yyyy-MM-dd"),
                StartTime = model.StartTime,
                ServiceIds = model.SelectedServiceIds
            };

            var response = await _apiService.PostAsync<ApiResponseViewModel<AppointmentViewModel>>(
                "appointments/admin/create", request);

            if (response?.Success == true)
            {
                TempData["SuccessMessage"] = "Appointment created successfully!";
                _logger.LogInformation("Receptionist created appointment for customer {CustomerId}", model.CustomerId);
                return RedirectToAction(nameof(Appointments));
            }

            TempData["ErrorMessage"] = response?.Message ?? "Failed to create appointment.";
            if (response?.Errors?.Count > 0)
                TempData["ErrorMessage"] = string.Join(", ", response.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create appointment failed");
            TempData["ErrorMessage"] = "An unexpected error occurred.";
        }

        var vm = await BuildReceptionistBookingVmAsync();
        model.Barbers = vm.Barbers;
        model.Services = vm.Services;
        ViewBag.Customers = vm.Customers();
        return View(model);
    }

    // ── Complete Appointment ─────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteAppointment(int id)
    {
        try
        {
            var response = await _apiService.PostAsync<ApiResponseViewModel<object>>(
                $"appointments/{id}/complete", new { });

            if (response?.Success == true)
                TempData["SuccessMessage"] = "Appointment marked as completed.";
            else
                TempData["ErrorMessage"] = response?.Message ?? "Failed to complete appointment.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Complete appointment {Id} failed", id);
            TempData["ErrorMessage"] = "An unexpected error occurred.";
        }

        return RedirectToAction(nameof(Appointments));
    }

    // ── Cancel Appointment ───────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        try
        {
            var response = await _apiService.PostAsync<ApiResponseViewModel<object>>(
                $"appointments/admin/{id}/cancel", new { });

            if (response?.Success == true)
                TempData["SuccessMessage"] = "Appointment cancelled.";
            else
                TempData["ErrorMessage"] = response?.Message ?? "Failed to cancel appointment.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cancel appointment {Id} failed", id);
            TempData["ErrorMessage"] = "An unexpected error occurred.";
        }

        return RedirectToAction(nameof(Appointments));
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private async Task<ReceptionistBookingViewModelWithCustomers> BuildReceptionistBookingVmAsync()
    {
        var barbersTask = _apiService.GetAsync<ApiResponseViewModel<PagedDataViewModel<BarberViewModel>>>("barbers?pageSize=100");
        var servicesTask = _apiService.GetAsync<ApiResponseViewModel<PagedDataViewModel<ServiceViewModel>>>("services?pageSize=100");
        var customersTask = _apiService.GetAsync<ApiResponseViewModel<PagedDataViewModel<CustomerViewModel>>>("customers?pageSize=500");

        await Task.WhenAll(barbersTask, servicesTask, customersTask);

        return new ReceptionistBookingViewModelWithCustomers
        {
            Barbers = barbersTask.Result?.Data?.Data ?? new List<BarberViewModel>(),
            Services = servicesTask.Result?.Data?.Data ?? new List<ServiceViewModel>(),
            CustomerList = customersTask.Result?.Data?.Data ?? new List<CustomerViewModel>()
        };
    }
}

/// <summary>
/// Internal helper that extends ReceptionistBookingViewModel with the customers list
/// loaded from the API (not persisted to the form, loaded fresh on each GET/POST error).
/// </summary>
public class ReceptionistBookingViewModelWithCustomers : ReceptionistBookingViewModel
{
    public List<CustomerViewModel> CustomerList { get; set; } = new();

    // Convenience method used by the controller to set ViewBag
    public List<CustomerViewModel> Customers() => CustomerList;
}
