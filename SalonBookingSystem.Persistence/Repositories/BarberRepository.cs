using Microsoft.EntityFrameworkCore;
using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Domain.Entities;
using SalonBookingSystem.Persistence.Context;

namespace SalonBookingSystem.Persistence.Repositories;

public class BarberRepository : IBarberRepository
{
    private readonly ApplicationDbContext _context;

    public BarberRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Barber?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Barbers
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Barber> Items, int TotalCount)> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var barbers = _context.Barbers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            barbers = barbers.Where(b =>
                EF.Functions.Like(b.FirstName, $"%{search}%") ||
                EF.Functions.Like(b.LastName, $"%{search}%") ||
                EF.Functions.Like(b.PhoneNumber, $"%{search}%"));
        }

        var totalCount = await barbers.CountAsync(cancellationToken);

        var items = await barbers
            .OrderBy(b => b.LastName)
            .ThenBy(b => b.FirstName)
            .Skip((query.NormalizedPageNumber - 1) * query.NormalizedPageSize)
            .Take(query.NormalizedPageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Barber> CreateAsync(Barber barber, CancellationToken cancellationToken = default)
    {
        _context.Barbers.Add(barber);
        await _context.SaveChangesAsync(cancellationToken);
        return barber;
    }

    public async Task UpdateAsync(Barber barber, CancellationToken cancellationToken = default)
    {
        _context.Barbers.Update(barber);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
