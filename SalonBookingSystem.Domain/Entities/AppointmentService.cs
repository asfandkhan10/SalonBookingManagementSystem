using SalonBookingSystem.Domain.Common;

namespace SalonBookingSystem.Domain.Entities;

public class AppointmentService : BaseEntity
{
    public int AppointmentId { get; set; }

    public int ServiceId { get; set; }

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }
}
