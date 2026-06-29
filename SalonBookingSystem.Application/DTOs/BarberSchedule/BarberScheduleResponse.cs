namespace SalonBookingSystem.Application.DTOs.BarberSchedule;

public class BarberScheduleResponse
{
    public int Id { get; set; }

    public int BarberId { get; set; }

    public Domain.Enums.DayOfWeek DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }
}
