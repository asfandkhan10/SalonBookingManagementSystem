using FluentValidation;
using Microsoft.Extensions.Logging;
using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Application.DTOs.Customer;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Application.Mappings;

namespace SalonBookingSystem.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<CreateCustomerRequest> _createValidator;
    private readonly IValidator<UpdateCustomerRequest> _updateValidator;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository customerRepository,
        ICurrentUserService currentUserService,
        IValidator<CreateCustomerRequest> createValidator,
        IValidator<UpdateCustomerRequest> updateValidator,
        ILogger<CustomerService> logger)
    {
        _customerRepository = customerRepository;
        _currentUserService = currentUserService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<CustomerResponse> CreateCustomerAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, request, cancellationToken);

        var customer = request.ToEntity();
        var userId = _currentUserService.GetUserId();
        var now = DateTime.UtcNow;

        customer.CreatedAt = now;
        customer.CreatedBy = userId;
        customer.IsDeleted = false;

        var created = await _customerRepository.CreateAsync(customer, cancellationToken);

        _logger.LogInformation("Customer {CustomerId} created by {UserId}", created.Id, userId);

        return created.ToResponse();
    }

    public async Task<CustomerResponse?> UpdateCustomerAsync(
        int id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidator, request, cancellationToken);

        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            return null;
        }

        customer.ApplyUpdate(request);

        var userId = _currentUserService.GetUserId();
        customer.UpdatedAt = DateTime.UtcNow;
        customer.UpdatedBy = userId;

        await _customerRepository.UpdateAsync(customer, cancellationToken);

        _logger.LogInformation("Customer {CustomerId} updated by {UserId}", id, userId);

        return customer.ToResponse();
    }

    public async Task<CustomerResponse?> GetCustomerByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        return customer?.ToResponse();
    }

    public async Task<PagedResult<CustomerResponse>> GetCustomersAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var normalizedQuery = new PagedQuery
        {
            PageNumber = query.NormalizedPageNumber,
            PageSize = query.NormalizedPageSize,
            Search = query.Search
        };

        var (items, totalCount) = await _customerRepository.GetPagedAsync(normalizedQuery, cancellationToken);
        var pageSize = normalizedQuery.NormalizedPageSize;
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<CustomerResponse>
        {
            PageNumber = normalizedQuery.NormalizedPageNumber,
            PageSize = pageSize,
            TotalRecords = totalCount,
            TotalPages = totalPages,
            Data = items.Select(c => c.ToResponse()).ToList()
        };
    }

    public async Task<bool> SoftDeleteCustomerAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            return false;
        }

        var userId = _currentUserService.GetUserId();
        customer.IsDeleted = true;
        customer.UpdatedAt = DateTime.UtcNow;
        customer.UpdatedBy = userId;

        await _customerRepository.UpdateAsync(customer, cancellationToken);

        _logger.LogInformation("Customer {CustomerId} soft deleted by {UserId}", id, userId);

        return true;
    }

    public async Task<CustomerResponse?> GetCustomerByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByEmailAsync(email, cancellationToken);
        return customer?.ToResponse();
    }

    public async Task<CustomerResponse?> GetCustomerByApplicationUserIdAsync(
        string applicationUserId,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByApplicationUserIdAsync(applicationUserId, cancellationToken);
        return customer?.ToResponse();
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
