using SalonBookingSystem.Application.DTOs.BarberSchedule;
using SalonBookingSystem.Domain.Entities;

namespace SalonBookingSystem.Application.Mappings;

public static class BarberScheduleMappingExtensions
{
    public static BarberScheduleResponse ToResponse(this BarberSchedule schedule)
    {
        return new BarberScheduleResponse
        {
            Id = schedule.Id,
            BarberId = schedule.BarberId,
            DayOfWeek = schedule.DayOfWeek,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime,
            CreatedAt = schedule.CreatedAt,
            CreatedBy = schedule.CreatedBy,
            UpdatedAt = schedule.UpdatedAt,
            UpdatedBy = schedule.UpdatedBy
        };
    }

    public static BarberSchedule ToEntity(this CreateBarberScheduleRequest request)
    {
        return new BarberSchedule
        {
            BarberId = request.BarberId,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };
    }

    public static void ApplyUpdate(this BarberSchedule schedule, UpdateBarberScheduleRequest request)
    {
        schedule.DayOfWeek = request.DayOfWeek;
        schedule.StartTime = request.StartTime;
        schedule.EndTime = request.EndTime;
    }
}
