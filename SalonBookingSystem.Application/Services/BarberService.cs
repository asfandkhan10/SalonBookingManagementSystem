using FluentValidation;
using Microsoft.Extensions.Logging;
using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Application.DTOs.Barber;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Application.Mappings;

namespace SalonBookingSystem.Application.Services;

public class BarberService : IBarberService
{
    private readonly IBarberRepository _barberRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<CreateBarberRequest> _createValidator;
    private readonly IValidator<UpdateBarberRequest> _updateValidator;
    private readonly ILogger<BarberService> _logger;

    public BarberService(
        IBarberRepository barberRepository,
        ICurrentUserService currentUserService,
        IValidator<CreateBarberRequest> createValidator,
        IValidator<UpdateBarberRequest> updateValidator,
        ILogger<BarberService> logger)
    {
        _barberRepository = barberRepository;
        _currentUserService = currentUserService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<BarberResponse> CreateBarberAsync(
        CreateBarberRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, request, cancellationToken);

        var barber = request.ToEntity();
        var userId = _currentUserService.GetUserId();
        var now = DateTime.UtcNow;

        barber.CreatedAt = now;
        barber.CreatedBy = userId;
        barber.IsDeleted = false;

        var created = await _barberRepository.CreateAsync(barber, cancellationToken);

        _logger.LogInformation("Barber {BarberId} created by {UserId}", created.Id, userId);

        return created.ToResponse();
    }

    public async Task<BarberResponse?> UpdateBarberAsync(
        int id,
        UpdateBarberRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidator, request, cancellationToken);

        var barber = await _barberRepository.GetByIdAsync(id, cancellationToken);
        if (barber is null)
        {
            return null;
        }

        barber.ApplyUpdate(request);

        var userId = _currentUserService.GetUserId();
        barber.UpdatedAt = DateTime.UtcNow;
        barber.UpdatedBy = userId;

        await _barberRepository.UpdateAsync(barber, cancellationToken);

        _logger.LogInformation("Barber {BarberId} updated by {UserId}", id, userId);

        return barber.ToResponse();
    }

    public async Task<BarberResponse?> GetBarberByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var barber = await _barberRepository.GetByIdAsync(id, cancellationToken);
        return barber?.ToResponse();
    }

    public async Task<PagedResult<BarberResponse>> GetBarbersAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var normalizedQuery = new PagedQuery
        {
            PageNumber = query.NormalizedPageNumber,
            PageSize = query.NormalizedPageSize,
            Search = query.Search
        };

        var (items, totalCount) = await _barberRepository.GetPagedAsync(normalizedQuery, cancellationToken);
        var pageSize = normalizedQuery.NormalizedPageSize;
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<BarberResponse>
        {
            PageNumber = normalizedQuery.NormalizedPageNumber,
            PageSize = pageSize,
            TotalRecords = totalCount,
            TotalPages = totalPages,
            Data = items.Select(b => b.ToResponse()).ToList()
        };
    }

    public async Task<bool> ActivateBarberAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var barber = await _barberRepository.GetByIdAsync(id, cancellationToken);
        if (barber is null)
        {
            return false;
        }

        barber.IsActive = true;

        var userId = _currentUserService.GetUserId();
        barber.UpdatedAt = DateTime.UtcNow;
        barber.UpdatedBy = userId;

        await _barberRepository.UpdateAsync(barber, cancellationToken);

        _logger.LogInformation("Barber {BarberId} activated by {UserId}", id, userId);

        return true;
    }

    public async Task<bool> DeactivateBarberAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var barber = await _barberRepository.GetByIdAsync(id, cancellationToken);
        if (barber is null)
        {
            return false;
        }

        barber.IsActive = false;

        var userId = _currentUserService.GetUserId();
        barber.UpdatedAt = DateTime.UtcNow;
        barber.UpdatedBy = userId;

        await _barberRepository.UpdateAsync(barber, cancellationToken);

        _logger.LogInformation("Barber {BarberId} deactivated by {UserId}", id, userId);

        return true;
    }

    public async Task<bool> SoftDeleteBarberAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var barber = await _barberRepository.GetByIdAsync(id, cancellationToken);
        if (barber is null)
        {
            return false;
        }

        var userId = _currentUserService.GetUserId();
        barber.IsDeleted = true;
        barber.UpdatedAt = DateTime.UtcNow;
        barber.UpdatedBy = userId;

        await _barberRepository.UpdateAsync(barber, cancellationToken);

        _logger.LogInformation("Barber {BarberId} soft deleted by {UserId}", id, userId);

        return true;
    }

    private static async Task ValidateAsync<T>(
        IValidator<T> validator,
        T instance,
        CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(instance, cancellationToken);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }
}
