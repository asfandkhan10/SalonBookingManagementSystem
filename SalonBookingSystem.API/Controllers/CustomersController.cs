using Microsoft.AspNetCore.Mvc;
using SalonBookingSystem.API.Authorization;
using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Application.DTOs.Customer;
using SalonBookingSystem.Application.Interfaces;

namespace SalonBookingSystem.API.Controllers;

[Route("api/v1/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [AdministratorAuthorization]
    public async Task<ActionResult<ApiResponse<PagedResult<CustomerResponse>>>> GetCustomers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = new PagedQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Search = search
        };

        var result = await _customerService.GetCustomersAsync(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<CustomerResponse>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [ReceptionistAuthorization]
    public async Task<ActionResult<ApiResponse<CustomerResponse>>> GetCustomerById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            return NotFound(ApiResponse<CustomerResponse>.Fail($"Customer with id {id} was not found."));
        }

        return Ok(ApiResponse<CustomerResponse>.Ok(customer));
    }

    [HttpPost]
    [ReceptionistAuthorization]
    public async Task<ActionResult<ApiResponse<CustomerResponse>>> CreateCustomer(
        [FromBody] CreateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerService.CreateCustomerAsync(request, cancellationToken);
        var response = ApiResponse<CustomerResponse>.Ok(customer, "Customer created successfully.");

        return CreatedAtAction(
            nameof(GetCustomerById),
            new { id = customer.Id },
            response);
    }

    [HttpPut("{id:int}")]
    [ReceptionistAuthorization]
    public async Task<ActionResult<ApiResponse<CustomerResponse>>> UpdateCustomer(
        int id,
        [FromBody] UpdateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerService.UpdateCustomerAsync(id, request, cancellationToken);
        if (customer is null)
        {
            return NotFound(ApiResponse<CustomerResponse>.Fail($"Customer with id {id} was not found."));
        }

        return Ok(ApiResponse<CustomerResponse>.Ok(customer, "Customer updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [ReceptionistAuthorization]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCustomer(
        int id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await _customerService.SoftDeleteCustomerAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail($"Customer with id {id} was not found."));
        }

        return Ok(ApiResponse<object>.Ok(null!, "Customer deleted successfully."));
    }
}
