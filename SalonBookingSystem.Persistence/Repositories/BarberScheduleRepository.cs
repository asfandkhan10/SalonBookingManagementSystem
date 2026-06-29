using Microsoft.EntityFrameworkCore;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Domain.Entities;
using SalonBookingSystem.Persistence.Context;

namespace SalonBookingSystem.Persistence.Repositories;

public class BarberScheduleRepository : IBarberScheduleRepository
{
    private readonly ApplicationDbContext _context;

    public BarberScheduleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BarberSchedule?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.BarberSchedules
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<BarberSchedule>> GetByBarberIdAsync(int barberId, CancellationToken cancellationToken = default)
    {
        return await _context.BarberSchedules
            .AsNoTracking()
            .Where(s => s.BarberId == barberId)
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<BarberSchedule>> GetByBarberIdAndDayOfWeekAsync(
        int barberId,
        Domain.Enums.DayOfWeek dayOfWeek,
        CancellationToken cancellationToken = default)
    {
        return await _context.BarberSchedules
            .AsNoTracking()
            .Where(s => s.BarberId == barberId && s.DayOfWeek == dayOfWeek)
            .ToListAsync(cancellationToken);
    }

    public async Task<BarberSchedule> CreateAsync(BarberSchedule schedule, CancellationToken cancellationToken = default)
    {
        _context.BarberSchedules.Add(schedule);
        await _context.SaveChangesAsync(cancellationToken);
        return schedule;
    }

    public async Task UpdateAsync(BarberSchedule schedule, CancellationToken cancellationToken = default)
    {
        _context.BarberSchedules.Update(schedule);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(BarberSchedule schedule, CancellationToken cancellationToken = default)
    {
        schedule.IsDeleted = true;
        _context.BarberSchedules.Update(schedule);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
