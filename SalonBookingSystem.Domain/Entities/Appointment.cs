using SalonBookingSystem.Domain.Common;
using SalonBookingSystem.Domain.Enums;

namespace SalonBookingSystem.Domain.Entities;

public class Appointment : BaseEntity
{
    public int CustomerId { get; set; }

    public int BarberId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public AppointmentStatus Status { get; set; }

    public decimal TotalAmount { get; set; }

    public int TotalDurationMinutes { get; set; }
}
