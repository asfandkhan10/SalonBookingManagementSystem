using System.ComponentModel.DataAnnotations;

namespace SalonBookingSystem.Web.Models;

/// <summary>
/// Drives the 3-step customer self-service booking wizard.
/// Step 1: Choose services
/// Step 2: Choose barber, date, time slot
/// Step 3: Confirm
/// </summary>
public class BookingViewModel
{
    // ── Available data loaded from API ──────────────────────────────────────
    public List<BarberViewModel> Barbers { get; set; } = new();
    public List<ServiceViewModel> Services { get; set; } = new();
    public List<string> AvailableSlots { get; set; } = new();

    // ── Wizard state ────────────────────────────────────────────────────────
    public int Step { get; set; } = 1;

    // Step 1
    public List<int> SelectedServiceIds { get; set; } = new();

    // Step 2
    public int? SelectedBarberId { get; set; }
    public string? SelectedDate { get; set; }
    public string? SelectedTime { get; set; }

    // Step 3 — customer is already logged in; CustomerId comes from session
    public int CustomerId { get; set; }

    // ── Computed helpers ────────────────────────────────────────────────────
    public int TotalDurationMinutes =>
        Services
            .Where(s => SelectedServiceIds.Contains(s.Id))
            .Sum(s => s.DurationMinutes);

    public decimal TotalAmount =>
        Services
            .Where(s => SelectedServiceIds.Contains(s.Id))
            .Sum(s => s.Price);

    public string? SelectedBarberName =>
        Barbers.FirstOrDefault(b => b.Id == SelectedBarberId)?.FullName;

    public List<ServiceViewModel> SelectedServices =>
        Services.Where(s => SelectedServiceIds.Contains(s.Id)).ToList();
}
