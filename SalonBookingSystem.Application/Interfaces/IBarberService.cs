using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Application.DTOs.Barber;

namespace SalonBookingSystem.Application.Interfaces;

public interface IBarberService
{
    Task<BarberResponse> CreateBarberAsync(
        CreateBarberRequest request,
        CancellationToken cancellationToken = default);

    Task<BarberResponse?> UpdateBarberAsync(
        int id,
        UpdateBarberRequest request,
        CancellationToken cancellationToken = default);

    Task<BarberResponse?> GetBarberByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<PagedResult<BarberResponse>> GetBarbersAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default);

    Task<bool> ActivateBarberAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateBarberAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> SoftDeleteBarberAsync(
        int id,
        CancellationToken cancellationToken = default);
}
