using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Domain.Entities;

namespace SalonBookingSystem.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Customer> Items, int TotalCount)> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default);

    Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken = default);

    Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);

    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Customer?> GetByApplicationUserIdAsync(string applicationUserId, CancellationToken cancellationToken = default);
}
