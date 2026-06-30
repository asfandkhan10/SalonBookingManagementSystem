using SalonBookingSystem.Domain.Enums;

namespace SalonBookingSystem.Application.DTOs.Appointment;

public class UpdateAppointmentRequest
{
    public DateTime AppointmentDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public AppointmentStatus Status { get; set; }

    public List<int> ServiceIds { get; set; } = new();
}
