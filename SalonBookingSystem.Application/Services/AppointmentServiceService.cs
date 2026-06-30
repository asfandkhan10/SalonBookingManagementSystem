using Microsoft.Extensions.Logging;
using SalonBookingSystem.Application.DTOs.AppointmentService;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Application.Mappings;

namespace SalonBookingSystem.Application.Services;

public class AppointmentServiceService : IAppointmentServiceService
{
    private readonly IAppointmentServiceRepository _appointmentServiceRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AppointmentServiceService> _logger;

    public AppointmentServiceService(
        IAppointmentServiceRepository appointmentServiceRepository,
        ICurrentUserService currentUserService,
        ILogger<AppointmentServiceService> logger)
    {
        _appointmentServiceRepository = appointmentServiceRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<AppointmentServiceResponse> AddServiceToAppointmentAsync(
        CreateAppointmentServiceRequest request,
        CancellationToken cancellationToken = default)
    {
        var appointmentService = request.ToEntity();
        var userId = _currentUserService.GetUserId();
        var now = DateTime.UtcNow;

        appointmentService.CreatedAt = now;
        appointmentService.CreatedBy = userId;
        appointmentService.IsDeleted = false;

        var created = await _appointmentServiceRepository.CreateAsync(appointmentService, cancellationToken);

        _logger.LogInformation("AppointmentService {Id} added to Appointment {AppointmentId} by {UserId}", created.Id, created.AppointmentId, userId);

        return created.ToResponse();
    }

    public async Task<List<AppointmentServiceResponse>> GetAppointmentServicesAsync(
        int appointmentId,
        CancellationToken cancellationToken = default)
    {
        var appointmentServices = await _appointmentServiceRepository.GetByAppointmentIdAsync(appointmentId, cancellationToken);
        return appointmentServices.Select(as_ => as_.ToResponse()).ToList();
    }

    public async Task<bool> RemoveServiceFromAppointmentAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var appointmentService = await _appointmentServiceRepository.GetByIdAsync(id, cancellationToken);
        if (appointmentService is null)
        {
            return false;
        }

        var userId = _currentUserService.GetUserId();
        appointmentService.UpdatedAt = DateTime.UtcNow;
        appointmentService.UpdatedBy = userId;

        await _appointmentServiceRepository.DeleteAsync(appointmentService, cancellationToken);

        _logger.LogInformation("AppointmentService {Id} removed by {UserId}", id, userId);

        return true;
    }
}
