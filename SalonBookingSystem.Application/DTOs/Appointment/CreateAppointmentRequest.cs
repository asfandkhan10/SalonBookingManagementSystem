using SalonBookingSystem.Domain.Enums;

namespace SalonBookingSystem.Application.DTOs.Appointment;

public class CreateAppointmentRequest
{
    public int CustomerId { get; set; }

    public int BarberId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public List<int> ServiceIds { get; set; } = new();
}
