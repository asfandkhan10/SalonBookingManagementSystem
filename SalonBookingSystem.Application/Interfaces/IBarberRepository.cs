using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Domain.Entities;

namespace SalonBookingSystem.Application.Interfaces;

public interface IBarberRepository
{
    Task<Barber?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Barber> Items, int TotalCount)> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default);

    Task<Barber> CreateAsync(Barber barber, CancellationToken cancellationToken = default);

    Task UpdateAsync(Barber barber, CancellationToken cancellationToken = default);
}
