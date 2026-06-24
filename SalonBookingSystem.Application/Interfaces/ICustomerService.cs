using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Application.DTOs.Customer;

namespace SalonBookingSystem.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerResponse> CreateCustomerAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken = default);

    Task<CustomerResponse?> UpdateCustomerAsync(
        int id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken = default);

    Task<CustomerResponse?> GetCustomerByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<PagedResult<CustomerResponse>> GetCustomersAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default);

    Task<bool> SoftDeleteCustomerAsync(
        int id,
        CancellationToken cancellationToken = default);
}
