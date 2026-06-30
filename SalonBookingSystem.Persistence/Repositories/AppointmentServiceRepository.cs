using Microsoft.EntityFrameworkCore;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Domain.Entities;
using SalonBookingSystem.Persistence.Context;

namespace SalonBookingSystem.Persistence.Repositories;

public class AppointmentServiceRepository : IAppointmentServiceRepository
{
    private readonly ApplicationDbContext _context;

    public AppointmentServiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AppointmentService?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.AppointmentServices
            .AsNoTracking()
            .FirstOrDefaultAsync(as_ => as_.Id == id, cancellationToken);
    }

    public async Task<List<AppointmentService>> GetByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        return await _context.AppointmentServices
            .AsNoTracking()
            .Where(as_ => as_.AppointmentId == appointmentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<AppointmentService> CreateAsync(AppointmentService appointmentService, CancellationToken cancellationToken = default)
    {
        _context.AppointmentServices.Add(appointmentService);
        await _context.SaveChangesAsync(cancellationToken);
        return appointmentService;
    }

    public async Task DeleteAsync(AppointmentService appointmentService, CancellationToken cancellationToken = default)
    {
        appointmentService.IsDeleted = true;
        _context.AppointmentServices.Update(appointmentService);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
