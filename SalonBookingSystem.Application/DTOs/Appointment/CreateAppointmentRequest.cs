using SalonBookingSystem.Domain.Enums;

namespace SalonBookingSystem.Application.DTOs.Appointment;

public class CreateAppointmentRequest
{
    public int CustomerId { get; set; }

    public int BarberId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public string StartTime { get; set; } = string.Empty;

    public List<int> ServiceIds { get; set; } = new();
}
