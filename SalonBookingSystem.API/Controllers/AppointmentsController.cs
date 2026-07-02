using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SalonBookingSystem.API.Authorization;
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
    private readonly IValidator<RescheduleAppointmentRequest> _rescheduleValidator;

    public AppointmentsController(
        IAppointmentService appointmentService,
        IValidator<CreateAppointmentRequest> createValidator,
        IValidator<UpdateAppointmentRequest> updateValidator,
        IValidator<RescheduleAppointmentRequest> rescheduleValidator)
    {
        _appointmentService = appointmentService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _rescheduleValidator = rescheduleValidator;
    }

    [HttpGet]
    [AdministratorAuthorization]
    public async Task<IActionResult> GetAppointments(CancellationToken cancellationToken)
    {
        var appointments = await _appointmentService.GetAppointmentsAsync(cancellationToken);
        return Ok(ApiResponse<List<AppointmentResponse>>.Ok(appointments));
    }

    [HttpGet("{id}")]
    [CustomerAuthorization]
    public async Task<IActionResult> GetAppointmentById(int id, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentService.GetAppointmentByIdAsync(id, cancellationToken);
        if (appointment is null)
        {
            return NotFound(ApiResponse<AppointmentResponse>.Fail("Appointment not found"));
        }

        var currentCustomerId = (int)HttpContext.Items["CustomerId"]!;
        if (appointment.CustomerId != currentCustomerId)
        {
            return Forbid();
        }

        return Ok(ApiResponse<AppointmentResponse>.Ok(appointment));
    }

    [HttpGet("barber/{barberId}")]
    [ReceptionistAuthorization]
    public async Task<IActionResult> GetAppointmentsByBarberId(int barberId, CancellationToken cancellationToken)
    {
        var appointments = await _appointmentService.GetAppointmentsByBarberIdAsync(barberId, cancellationToken);
        return Ok(ApiResponse<List<AppointmentResponse>>.Ok(appointments));
    }

    [HttpGet("customer/{customerId}")]
    [CustomerAuthorization]
    public async Task<IActionResult> GetAppointmentsByCustomerId(int customerId, CancellationToken cancellationToken)
    {
        var currentCustomerId = (int)HttpContext.Items["CustomerId"]!;
        if (currentCustomerId != customerId)
        {
            return Forbid();
        }

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
    [CustomerAuthorization]
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

        // Set CustomerId from authenticated user
        var currentCustomerId = (int)HttpContext.Items["CustomerId"]!;
        request.CustomerId = currentCustomerId;

        var appointment = await _appointmentService.CreateAppointmentAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, ApiResponse<AppointmentResponse>.Ok(appointment));
    }

    [HttpPut("{id}")]
    [CustomerAuthorization]
    public async Task<IActionResult> UpdateAppointment(int id, [FromBody] UpdateAppointmentRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Validation failed.", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
        }

        var appointment = await _appointmentService.GetAppointmentByIdAsync(id, cancellationToken);
        if (appointment is null)
        {
            return NotFound(ApiResponse<AppointmentResponse>.Fail("Appointment not found"));
        }

        var currentCustomerId = (int)HttpContext.Items["CustomerId"]!;
        if (appointment.CustomerId != currentCustomerId)
        {
            return Forbid();
        }

        var updated = await _appointmentService.UpdateAppointmentAsync(id, request, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<AppointmentResponse>.Fail("Failed to update appointment"));
        }
        return Ok(ApiResponse<AppointmentResponse>.Ok(updated));
    }

    [HttpPost("{id}/cancel")]
    [CustomerAuthorization]
    public async Task<IActionResult> CancelAppointment(int id, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentService.GetAppointmentByIdAsync(id, cancellationToken);
        if (appointment is null)
        {
            return NotFound(ApiResponse<bool>.Fail("Appointment not found"));
        }

        var currentCustomerId = (int)HttpContext.Items["CustomerId"]!;
        if (appointment.CustomerId != currentCustomerId)
        {
            return Forbid();
        }

        var result = await _appointmentService.CancelAppointmentAsync(id, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(true, "Appointment cancelled successfully"));
    }

    [HttpPost("{id}/reschedule")]
    [CustomerAuthorization]
    public async Task<IActionResult> RescheduleAppointment(int id, [FromBody] RescheduleAppointmentRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest(ApiResponse<object>.Fail("Request body is required."));
        }

        var validationResult = await _rescheduleValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Validation failed.", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
        }

        var appointment = await _appointmentService.GetAppointmentByIdAsync(id, cancellationToken);
        if (appointment is null)
        {
            return NotFound(ApiResponse<AppointmentResponse>.Fail("Appointment not found"));
        }

        var currentCustomerId = (int)HttpContext.Items["CustomerId"]!;
        if (appointment.CustomerId != currentCustomerId)
        {
            return Forbid();
        }

        var rescheduled = await _appointmentService.RescheduleAppointmentAsync(id, request, cancellationToken);
        if (rescheduled is null)
        {
            return NotFound(ApiResponse<AppointmentResponse>.Fail("Failed to reschedule appointment"));
        }
        return Ok(ApiResponse<AppointmentResponse>.Ok(rescheduled, "Appointment rescheduled successfully"));
    }

    [HttpPost("{id}/complete")]
    [ReceptionistAuthorization]
    public async Task<IActionResult> CompleteAppointment(int id, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.CompleteAppointmentAsync(id, cancellationToken);
        if (!result)
        {
            return NotFound(ApiResponse<bool>.Fail("Appointment not found"));
        }
        return Ok(ApiResponse<bool>.Ok(true, "Appointment completed successfully"));
    }

    // Receptionist/Administrator appointment management endpoints

    [HttpPost("admin/create")]
    [ReceptionistAuthorization]
    public async Task<IActionResult> CreateAppointmentForCustomer([FromBody] CreateAppointmentRequest request, CancellationToken cancellationToken)
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

    [HttpPut("admin/{id}")]
    [ReceptionistAuthorization]
    public async Task<IActionResult> UpdateAppointmentForCustomer(int id, [FromBody] UpdateAppointmentRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Validation failed.", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
        }

        var updated = await _appointmentService.UpdateAppointmentAsync(id, request, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<AppointmentResponse>.Fail("Appointment not found"));
        }
        return Ok(ApiResponse<AppointmentResponse>.Ok(updated));
    }

    [HttpPost("admin/{id}/cancel")]
    [ReceptionistAuthorization]
    public async Task<IActionResult> CancelAppointmentForCustomer(int id, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.CancelAppointmentAsync(id, cancellationToken);
        if (!result)
        {
            return NotFound(ApiResponse<bool>.Fail("Appointment not found"));
        }
        return Ok(ApiResponse<bool>.Ok(true, "Appointment cancelled successfully"));
    }

    [HttpPost("admin/{id}/reschedule")]
    [ReceptionistAuthorization]
    public async Task<IActionResult> RescheduleAppointmentForCustomer(int id, [FromBody] RescheduleAppointmentRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest(ApiResponse<object>.Fail("Request body is required."));
        }

        var validationResult = await _rescheduleValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Validation failed.", validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
        }

        var rescheduled = await _appointmentService.RescheduleAppointmentAsync(id, request, cancellationToken);
        if (rescheduled is null)
        {
            return NotFound(ApiResponse<AppointmentResponse>.Fail("Appointment not found"));
        }
        return Ok(ApiResponse<AppointmentResponse>.Ok(rescheduled, "Appointment rescheduled successfully"));
    }
}
