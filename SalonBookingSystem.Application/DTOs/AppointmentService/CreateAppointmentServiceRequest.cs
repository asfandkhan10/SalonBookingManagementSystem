namespace SalonBookingSystem.Application.DTOs.AppointmentService;

public class CreateAppointmentServiceRequest
{
    public int AppointmentId { get; set; }

    public int ServiceId { get; set; }

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }
}
