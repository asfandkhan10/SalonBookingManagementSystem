using Microsoft.AspNetCore.Mvc;
using SalonBookingSystem.API.Authorization;
using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Application.DTOs.Barber;
using SalonBookingSystem.Application.Interfaces;

namespace SalonBookingSystem.API.Controllers;

[Route("api/v1/barbers")]
public class BarbersController : ControllerBase
{
    private readonly IBarberService _barberService;

    public BarbersController(IBarberService barberService)
    {
        _barberService = barberService;
    }

    [HttpGet]
    [AdministratorAuthorization]
    public async Task<ActionResult<ApiResponse<PagedResult<BarberResponse>>>> GetBarbers(
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

        var result = await _barberService.GetBarbersAsync(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<BarberResponse>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [ReceptionistAuthorization]
    public async Task<ActionResult<ApiResponse<BarberResponse>>> GetBarberById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var barber = await _barberService.GetBarberByIdAsync(id, cancellationToken);
        if (barber is null)
        {
            return NotFound(ApiResponse<BarberResponse>.Fail($"Barber with id {id} was not found."));
        }

        return Ok(ApiResponse<BarberResponse>.Ok(barber));
    }

    [HttpPost]
    [AdministratorAuthorization]
    public async Task<ActionResult<ApiResponse<BarberResponse>>> CreateBarber(
        [FromBody] CreateBarberRequest request,
        CancellationToken cancellationToken = default)
    {
        var barber = await _barberService.CreateBarberAsync(request, cancellationToken);
        var response = ApiResponse<BarberResponse>.Ok(barber, "Barber created successfully.");

        return CreatedAtAction(
            nameof(GetBarberById),
            new { id = barber.Id },
            response);
    }

    [HttpPut("{id:int}")]
    [AdministratorAuthorization]
    public async Task<ActionResult<ApiResponse<BarberResponse>>> UpdateBarber(
        int id,
        [FromBody] UpdateBarberRequest request,
        CancellationToken cancellationToken = default)
    {
        var barber = await _barberService.UpdateBarberAsync(id, request, cancellationToken);
        if (barber is null)
        {
            return NotFound(ApiResponse<BarberResponse>.Fail($"Barber with id {id} was not found."));
        }

        return Ok(ApiResponse<BarberResponse>.Ok(barber, "Barber updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [AdministratorAuthorization]
    public async Task<ActionResult<ApiResponse<object>>> DeleteBarber(
        int id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await _barberService.SoftDeleteBarberAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail($"Barber with id {id} was not found."));
        }

        return Ok(ApiResponse<object>.Ok(null!, "Barber deleted successfully."));
    }

    [HttpPost("{id:int}/activate")]
    [AdministratorAuthorization]
    public async Task<ActionResult<ApiResponse<BarberResponse>>> ActivateBarber(
        int id,
        CancellationToken cancellationToken = default)
    {
        var activated = await _barberService.ActivateBarberAsync(id, cancellationToken);
        if (!activated)
        {
            return NotFound(ApiResponse<BarberResponse>.Fail($"Barber with id {id} was not found."));
        }

        var barber = await _barberService.GetBarberByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<BarberResponse>.Ok(barber!, "Barber activated successfully."));
    }

    [HttpPost("{id:int}/deactivate")]
    [AdministratorAuthorization]
    public async Task<ActionResult<ApiResponse<BarberResponse>>> DeactivateBarber(
        int id,
        CancellationToken cancellationToken = default)
    {
        var deactivated = await _barberService.DeactivateBarberAsync(id, cancellationToken);
        if (!deactivated)
        {
            return NotFound(ApiResponse<BarberResponse>.Fail($"Barber with id {id} was not found."));
        }

        var barber = await _barberService.GetBarberByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<BarberResponse>.Ok(barber!, "Barber deactivated successfully."));
    }
}
