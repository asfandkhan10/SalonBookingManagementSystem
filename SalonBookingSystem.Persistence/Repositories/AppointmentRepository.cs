using Microsoft.EntityFrameworkCore;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Domain.Entities;
using SalonBookingSystem.Domain.Enums;
using SalonBookingSystem.Persistence.Context;

namespace SalonBookingSystem.Persistence.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly ApplicationDbContext _context;

    public AppointmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Appointment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Appointment>> GetByBarberIdAsync(int barberId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Where(a => a.BarberId == barberId)
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Appointment>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Where(a => a.CustomerId == customerId)
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Appointment>> GetByBarberIdAndDateAsync(
        int barberId,
        DateTime appointmentDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Where(a => a.BarberId == barberId
                && a.AppointmentDate.Date == appointmentDate.Date
                && a.Status != Domain.Enums.AppointmentStatus.Cancelled)
            .OrderBy(a => a.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<Appointment> CreateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync(cancellationToken);
        return appointment;
    }

    public async Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        appointment.IsDeleted = true;
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
