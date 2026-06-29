namespace SalonBookingSystem.Application.DTOs.BarberSchedule;

public class UpdateBarberScheduleRequest
{
    public Domain.Enums.DayOfWeek DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }
}
