using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Application.DTOs.Service;

namespace SalonBookingSystem.Application.Interfaces;

public interface IServiceService
{
    Task<ServiceResponse> CreateServiceAsync(
        CreateServiceRequest request,
        CancellationToken cancellationToken = default);

    Task<ServiceResponse?> UpdateServiceAsync(
        int id,
        UpdateServiceRequest request,
        CancellationToken cancellationToken = default);

    Task<ServiceResponse?> GetServiceByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ServiceResponse>> GetServicesAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default);

    Task<bool> SoftDeleteServiceAsync(
        int id,
        CancellationToken cancellationToken = default);
}
