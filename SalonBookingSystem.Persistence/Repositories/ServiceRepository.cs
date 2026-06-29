using Microsoft.EntityFrameworkCore;
using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Domain.Entities;
using SalonBookingSystem.Persistence.Context;

namespace SalonBookingSystem.Persistence.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly ApplicationDbContext _context;

    public ServiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Service?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Services
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Service> Items, int TotalCount)> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var services = _context.Services.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            services = services.Where(s =>
                EF.Functions.Like(s.Name, $"%{search}%") ||
                EF.Functions.Like(s.Description, $"%{search}%"));
        }

        var totalCount = await services.CountAsync(cancellationToken);

        var items = await services
            .OrderBy(s => s.Name)
            .Skip((query.NormalizedPageNumber - 1) * query.NormalizedPageSize)
            .Take(query.NormalizedPageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Service> CreateAsync(Service service, CancellationToken cancellationToken = default)
    {
        _context.Services.Add(service);
        await _context.SaveChangesAsync(cancellationToken);
        return service;
    }

    public async Task UpdateAsync(Service service, CancellationToken cancellationToken = default)
    {
        _context.Services.Update(service);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
