using SalonBookingSystem.Domain.Enums;

namespace SalonBookingSystem.Application.DTOs.Appointment;

public class RescheduleAppointmentRequest
{
    public DateTime NewAppointmentDate { get; set; }

    public string NewStartTime { get; set; } = string.Empty;
}
