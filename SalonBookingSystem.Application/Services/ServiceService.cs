using FluentValidation;
using Microsoft.Extensions.Logging;
using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Application.DTOs.Service;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Application.Mappings;

namespace SalonBookingSystem.Application.Services;

public class ServiceService : IServiceService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<CreateServiceRequest> _createValidator;
    private readonly IValidator<UpdateServiceRequest> _updateValidator;
    private readonly ILogger<ServiceService> _logger;

    public ServiceService(
        IServiceRepository serviceRepository,
        ICurrentUserService currentUserService,
        IValidator<CreateServiceRequest> createValidator,
        IValidator<UpdateServiceRequest> updateValidator,
        ILogger<ServiceService> logger)
    {
        _serviceRepository = serviceRepository;
        _currentUserService = currentUserService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<ServiceResponse> CreateServiceAsync(
        CreateServiceRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, request, cancellationToken);

        var service = request.ToEntity();
        var userId = _currentUserService.GetUserId();
        var now = DateTime.UtcNow;

        service.CreatedAt = now;
        service.CreatedBy = userId;
        service.IsDeleted = false;

        var created = await _serviceRepository.CreateAsync(service, cancellationToken);

        _logger.LogInformation("Service {ServiceId} created by {UserId}", created.Id, userId);

        return created.ToResponse();
    }

    public async Task<ServiceResponse?> UpdateServiceAsync(
        int id,
        UpdateServiceRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidator, request, cancellationToken);

        var service = await _serviceRepository.GetByIdAsync(id, cancellationToken);
        if (service is null)
        {
            return null;
        }

        service.ApplyUpdate(request);

        var userId = _currentUserService.GetUserId();
        service.UpdatedAt = DateTime.UtcNow;
        service.UpdatedBy = userId;

        await _serviceRepository.UpdateAsync(service, cancellationToken);

        _logger.LogInformation("Service {ServiceId} updated by {UserId}", id, userId);

        return service.ToResponse();
    }

    public async Task<ServiceResponse?> GetServiceByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var service = await _serviceRepository.GetByIdAsync(id, cancellationToken);
        return service?.ToResponse();
    }

    public async Task<PagedResult<ServiceResponse>> GetServicesAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var normalizedQuery = new PagedQuery
        {
            PageNumber = query.NormalizedPageNumber,
            PageSize = query.NormalizedPageSize,
            Search = query.Search
        };

        var (items, totalCount) = await _serviceRepository.GetPagedAsync(normalizedQuery, cancellationToken);
        var pageSize = normalizedQuery.NormalizedPageSize;
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<ServiceResponse>
        {
            PageNumber = normalizedQuery.NormalizedPageNumber,
            PageSize = pageSize,
            TotalRecords = totalCount,
            TotalPages = totalPages,
            Data = items.Select(s => s.ToResponse()).ToList()
        };
    }

    public async Task<bool> SoftDeleteServiceAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var service = await _serviceRepository.GetByIdAsync(id, cancellationToken);
        if (service is null)
        {
            return false;
        }

        var userId = _currentUserService.GetUserId();
        service.IsDeleted = true;
        service.UpdatedAt = DateTime.UtcNow;
        service.UpdatedBy = userId;

        await _serviceRepository.UpdateAsync(service, cancellationToken);

        _logger.LogInformation("Service {ServiceId} soft deleted by {UserId}", id, userId);

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
