using SalonBookingSystem.Domain.Entities;

namespace SalonBookingSystem.Application.Interfaces;

public interface IBarberScheduleRepository
{
    Task<BarberSchedule?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<List<BarberSchedule>> GetByBarberIdAsync(int barberId, CancellationToken cancellationToken = default);

    Task<List<BarberSchedule>> GetByBarberIdAndDayOfWeekAsync(
        int barberId,
        Domain.Enums.DayOfWeek dayOfWeek,
        CancellationToken cancellationToken = default);

    Task<BarberSchedule> CreateAsync(BarberSchedule schedule, CancellationToken cancellationToken = default);

    Task UpdateAsync(BarberSchedule schedule, CancellationToken cancellationToken = default);

    Task DeleteAsync(BarberSchedule schedule, CancellationToken cancellationToken = default);
}
