using System.ComponentModel.DataAnnotations;

namespace SalonBookingSystem.Web.Models;

/// <summary>
/// Flat view model used by the Receptionist to create appointments directly.
/// Separate from BookingViewModel (which drives the customer 3-step wizard).
/// </summary>
public class ReceptionistBookingViewModel
{
    public List<BarberViewModel> Barbers { get; set; } = new();
    public List<ServiceViewModel> Services { get; set; } = new();

    [Required(ErrorMessage = "Customer is required")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Barber is required")]
    public int BarberId { get; set; }

    [Required(ErrorMessage = "Appointment date is required")]
    public DateTime AppointmentDate { get; set; } = DateTime.Today.AddDays(1);

    [Required(ErrorMessage = "Start time is required")]
    public string StartTime { get; set; } = "09:00";

    [Required(ErrorMessage = "At least one service is required")]
    public List<int> SelectedServiceIds { get; set; } = new();
}
