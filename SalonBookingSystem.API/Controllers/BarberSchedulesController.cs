using Microsoft.AspNetCore.Mvc;
using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Application.DTOs.BarberSchedule;
using SalonBookingSystem.Application.Interfaces;

namespace SalonBookingSystem.API.Controllers;

[ApiController]
[Route("api/v1/barbers/{barberId}/schedules")]
public class BarberSchedulesController : ControllerBase
{
    private readonly IBarberScheduleService _scheduleService;

    public BarberSchedulesController(IBarberScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<BarberScheduleResponse>>>> GetBarberSchedules(
        int barberId,
        CancellationToken cancellationToken = default)
    {
        var schedules = await _scheduleService.GetBarberSchedulesAsync(barberId, cancellationToken);
        return Ok(ApiResponse<List<BarberScheduleResponse>>.Ok(schedules));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<BarberScheduleResponse>>> GetScheduleById(
        int barberId,
        int id,
        CancellationToken cancellationToken = default)
    {
        var schedule = await _scheduleService.GetScheduleByIdAsync(id, cancellationToken);
        if (schedule is null)
        {
            return NotFound(ApiResponse<BarberScheduleResponse>.Fail($"Schedule with id {id} was not found."));
        }

        return Ok(ApiResponse<BarberScheduleResponse>.Ok(schedule));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BarberScheduleResponse>>> CreateSchedule(
        int barberId,
        [FromBody] CreateBarberScheduleRequest request,
        CancellationToken cancellationToken = default)
    {
        request.BarberId = barberId;

        var schedule = await _scheduleService.CreateScheduleAsync(request, cancellationToken);
        var response = ApiResponse<BarberScheduleResponse>.Ok(schedule, "Schedule created successfully.");

        return CreatedAtAction(
            nameof(GetScheduleById),
            new { barberId, id = schedule.Id },
            response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<BarberScheduleResponse>>> UpdateSchedule(
        int barberId,
        int id,
        [FromBody] UpdateBarberScheduleRequest request,
        CancellationToken cancellationToken = default)
    {
        var schedule = await _scheduleService.UpdateScheduleAsync(id, request, cancellationToken);
        if (schedule is null)
        {
            return NotFound(ApiResponse<BarberScheduleResponse>.Fail($"Schedule with id {id} was not found."));
        }

        return Ok(ApiResponse<BarberScheduleResponse>.Ok(schedule, "Schedule updated successfully."));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSchedule(
        int barberId,
        int id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await _scheduleService.DeleteScheduleAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail($"Schedule with id {id} was not found."));
        }

        return Ok(ApiResponse<object>.Ok(null!, "Schedule deleted successfully."));
    }
}
