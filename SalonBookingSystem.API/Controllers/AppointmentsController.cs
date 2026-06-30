using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SalonBookingSystem.Application.Common;
using SalonBookingSystem.Application.DTOs.Appointment;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Application.Validators;

namespace SalonBookingSystem.API.Controllers;

[Route("api/v1/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    private readonly IValidator<CreateAppointmentRequest> _createValidator;
    private readonly IValidator<UpdateAppointmentRequest> _updateValidator;

    public AppointmentsController(
        IAppointmentService appointmentService,
        IValidator<CreateAppointmentRequest> createValidator,
        IValidator<UpdateAppointmentRequest> updateValidator)
    {
        _appointmentService = appointmentService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAppointments(CancellationToken cancellationToken)
    {
        var appointments = await _appointmentService.GetAppointmentsAsync(cancellationToken);
        return Ok(ApiResponse<List<AppointmentResponse>>.Ok(appointments));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentById(int id, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentService.GetAppointmentByIdAsync(id, cancellationToken);
        if (appointment is null)
        {
            return NotFound(ApiResponse<AppointmentResponse>.Fail("Appointment not found"));
        }
        return Ok(ApiResponse<AppointmentResponse>.Ok(appointment));
    }

    [HttpGet("barber/{barberId}")]
    public async Task<IActionResult> GetAppointmentsByBarberId(int barberId, CancellationToken cancellationToken)
    {
        var appointments = await _appointmentService.GetAppointmentsByBarberIdAsync(barberId, cancellationToken);
        return Ok(ApiResponse<List<AppointmentResponse>>.Ok(appointments));
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetAppointmentsByCustomerId(int customerId, CancellationToken cancellationToken)
    {
        var appointments = await _appointmentService.GetAppointmentsByCustomerIdAsync(customerId, cancellationToken);
        return Ok(ApiResponse<List<AppointmentResponse>>.Ok(appointments));
    }

    [HttpGet("available-slots")]
    public async Task<IActionResult> GetAvailableSlots(
        [FromQuery] int barberId,
        [FromQuery] DateTime appointmentDate,
        [FromQuery] int durationMinutes,
        CancellationToken cancellationToken)
    {
        var availableSlots = await _appointmentService.GetAvailableSlotsAsync(barberId, appointmentDate, durationMinutes, cancellationToken);
        return Ok(ApiResponse<List<string>>.Ok(availableSlots));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest(ApiResponse<object>.Fail("Request body is required."));
        }

        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Validation failed.", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
        }

        var appointment = await _appointmentService.CreateAppointmentAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, ApiResponse<AppointmentResponse>.Ok(appointment));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment(int id, [FromBody] UpdateAppointmentRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Validation failed.", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
        }

        var appointment = await _appointmentService.UpdateAppointmentAsync(id, request, cancellationToken);
        if (appointment is null)
        {
            return NotFound(ApiResponse<AppointmentResponse>.Fail("Appointment not found"));
        }
        return Ok(ApiResponse<AppointmentResponse>.Ok(appointment));
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(int id, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.CancelAppointmentAsync(id, cancellationToken);
        if (!result)
        {
            return NotFound(ApiResponse<bool>.Fail("Appointment not found"));
        }
        return Ok(ApiResponse<bool>.Ok(true, "Appointment cancelled successfully"));
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteAppointment(int id, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.CompleteAppointmentAsync(id, cancellationToken);
        if (!result)
        {
            return NotFound(ApiResponse<bool>.Fail("Appointment not found"));
        }
        return Ok(ApiResponse<bool>.Ok(true, "Appointment completed successfully"));
    }
}
