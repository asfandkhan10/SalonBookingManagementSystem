using SalonBookingSystem.Domain.Common;

namespace SalonBookingSystem.Domain.Entities;

public class BarberSchedule : BaseEntity
{
    public int BarberId { get; set; }

    public Enums.DayOfWeek DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }
}
