using Microsoft.EntityFrameworkCore;
using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Domain.Entities;
using SalonBookingSystem.Persistence.Context;

namespace SalonBookingSystem.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Customer> Items, int TotalCount)> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var customers = _context.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            customers = customers.Where(c =>
                EF.Functions.Like(c.FirstName, $"%{search}%") ||
                EF.Functions.Like(c.LastName, $"%{search}%") ||
                EF.Functions.Like(c.Email, $"%{search}%") ||
                EF.Functions.Like(c.PhoneNumber, $"%{search}%"));
        }

        var totalCount = await customers.CountAsync(cancellationToken);

        var items = await customers
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Skip((query.NormalizedPageNumber - 1) * query.NormalizedPageSize)
            .Take(query.NormalizedPageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);
        return customer;
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<Customer?> GetByApplicationUserIdAsync(string applicationUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ApplicationUserId == applicationUserId, cancellationToken);
    }
}
