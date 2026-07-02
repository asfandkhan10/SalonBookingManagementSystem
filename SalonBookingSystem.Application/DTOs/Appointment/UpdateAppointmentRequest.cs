using SalonBookingSystem.Domain.Enums;

namespace SalonBookingSystem.Application.DTOs.Appointment;

public class UpdateAppointmentRequest
{
    public DateTime AppointmentDate { get; set; }

    public string StartTime { get; set; } = string.Empty;

    public AppointmentStatus Status { get; set; }

    public List<int> ServiceIds { get; set; } = new();
}
