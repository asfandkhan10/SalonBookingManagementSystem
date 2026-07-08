using Microsoft.AspNetCore.Mvc;
using SalonBookingSystem.API.Authorization;
using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Application.DTOs.Service;
using SalonBookingSystem.Application.Interfaces;

namespace SalonBookingSystem.API.Controllers;

[Route("api/v1/services")]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _serviceService;

    public ServicesController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ServiceResponse>>>> GetServices(
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

        var result = await _serviceService.GetServicesAsync(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<ServiceResponse>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [ReceptionistAuthorization]
    public async Task<ActionResult<ApiResponse<ServiceResponse>>> GetServiceById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var service = await _serviceService.GetServiceByIdAsync(id, cancellationToken);
        if (service is null)
        {
            return NotFound(ApiResponse<ServiceResponse>.Fail($"Service with id {id} was not found."));
        }

        return Ok(ApiResponse<ServiceResponse>.Ok(service));
    }

    [HttpPost]
    [AdministratorAuthorization]
    public async Task<ActionResult<ApiResponse<ServiceResponse>>> CreateService(
        [FromBody] CreateServiceRequest request,
        CancellationToken cancellationToken = default)
    {
        var service = await _serviceService.CreateServiceAsync(request, cancellationToken);
        var response = ApiResponse<ServiceResponse>.Ok(service, "Service created successfully.");

        return CreatedAtAction(
            nameof(GetServiceById),
            new { id = service.Id },
            response);
    }

    [HttpPut("{id:int}")]
    [AdministratorAuthorization]
    public async Task<ActionResult<ApiResponse<ServiceResponse>>> UpdateService(
        int id,
        [FromBody] UpdateServiceRequest request,
        CancellationToken cancellationToken = default)
    {
        var service = await _serviceService.UpdateServiceAsync(id, request, cancellationToken);
        if (service is null)
        {
            return NotFound(ApiResponse<ServiceResponse>.Fail($"Service with id {id} was not found."));
        }

        return Ok(ApiResponse<ServiceResponse>.Ok(service, "Service updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [AdministratorAuthorization]
    public async Task<ActionResult<ApiResponse<object>>> DeleteService(
        int id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await _serviceService.SoftDeleteServiceAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail($"Service with id {id} was not found."));
        }

        return Ok(ApiResponse<object>.Ok(null!, "Service deleted successfully."));
    }
}
