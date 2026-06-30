namespace SalonBookingSystem.Application.DTOs.AppointmentService;

public class AppointmentServiceResponse
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public int ServiceId { get; set; }

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }
}
