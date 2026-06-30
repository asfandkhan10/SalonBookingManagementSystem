using FluentValidation;
using Microsoft.Extensions.Logging;
using SalonBookingSystem.Application.DTOs.Appointment;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Application.Mappings;
using SalonBookingSystem.Domain.Entities;

namespace SalonBookingSystem.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentServiceRepository _appointmentServiceRepository;
    private readonly IBarberScheduleRepository _barberScheduleRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<CreateAppointmentRequest> _createValidator;
    private readonly IValidator<UpdateAppointmentRequest> _updateValidator;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IAppointmentServiceRepository appointmentServiceRepository,
        IBarberScheduleRepository barberScheduleRepository,
        IServiceRepository serviceRepository,
        ICurrentUserService currentUserService,
        IValidator<CreateAppointmentRequest> createValidator,
        IValidator<UpdateAppointmentRequest> updateValidator,
        ILogger<AppointmentService> logger)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentServiceRepository = appointmentServiceRepository;
        _barberScheduleRepository = barberScheduleRepository;
        _serviceRepository = serviceRepository;
        _currentUserService = currentUserService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<AppointmentResponse> CreateAppointmentAsync(
        CreateAppointmentRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, request, cancellationToken);

        var appointment = request.ToEntity();
        var userId = _currentUserService.GetUserId();
        var now = DateTime.UtcNow;

        appointment.CreatedAt = now;
        appointment.CreatedBy = userId;
        appointment.IsDeleted = false;

        await ValidateNoOverlapAsync(appointment, null, cancellationToken);
        await ValidateBarberScheduleAsync(appointment, cancellationToken);
        await CalculateTotalsAsync(appointment, request.ServiceIds, cancellationToken);

        var created = await _appointmentRepository.CreateAsync(appointment, cancellationToken);

        await AddAppointmentServicesAsync(created.Id, request.ServiceIds, cancellationToken);

        _logger.LogInformation("Appointment {AppointmentId} created for Customer {CustomerId} and Barber {BarberId} by {UserId}", created.Id, created.CustomerId, created.BarberId, userId);

        return created.ToResponse();
    }

    public async Task<AppointmentResponse?> UpdateAppointmentAsync(
        int id,
        UpdateAppointmentRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidator, request, cancellationToken);

        var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
        if (appointment is null)
        {
            return null;
        }

        appointment.ApplyUpdate(request);

        await ValidateNoOverlapAsync(appointment, id, cancellationToken);
        await ValidateBarberScheduleAsync(appointment, cancellationToken);

        var userId = _currentUserService.GetUserId();
        appointment.UpdatedAt = DateTime.UtcNow;
        appointment.UpdatedBy = userId;

        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

        await UpdateAppointmentServicesAsync(appointment.Id, request.ServiceIds, cancellationToken);

        _logger.LogInformation("Appointment {AppointmentId} updated by {UserId}", id, userId);

        return appointment.ToResponse();
    }

    public async Task<AppointmentResponse?> GetAppointmentByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
        return appointment?.ToResponse();
    }

    public async Task<List<AppointmentResponse>> GetAppointmentsAsync(
        CancellationToken cancellationToken = default)
    {
        var appointments = await _appointmentRepository.GetAllAsync(cancellationToken);
        return appointments.Select(a => a.ToResponse()).ToList();
    }

    public async Task<List<AppointmentResponse>> GetAppointmentsByBarberIdAsync(
        int barberId,
        CancellationToken cancellationToken = default)
    {
        var appointments = await _appointmentRepository.GetByBarberIdAsync(barberId, cancellationToken);
        return appointments.Select(a => a.ToResponse()).ToList();
    }

    public async Task<List<AppointmentResponse>> GetAppointmentsByCustomerIdAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        var appointments = await _appointmentRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        return appointments.Select(a => a.ToResponse()).ToList();
    }

    public async Task<bool> CancelAppointmentAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
        if (appointment is null)
        {
            return false;
        }

        appointment.Status = Domain.Enums.AppointmentStatus.Cancelled;
        var userId = _currentUserService.GetUserId();
        appointment.UpdatedAt = DateTime.UtcNow;
        appointment.UpdatedBy = userId;

        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

        _logger.LogInformation("Appointment {AppointmentId} cancelled by {UserId}", id, userId);

        return true;
    }

    public async Task<bool> CompleteAppointmentAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
        if (appointment is null)
        {
            return false;
        }

        appointment.Status = Domain.Enums.AppointmentStatus.Completed;
        var userId = _currentUserService.GetUserId();
        appointment.UpdatedAt = DateTime.UtcNow;
        appointment.UpdatedBy = userId;

        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

        _logger.LogInformation("Appointment {AppointmentId} completed by {UserId}", id, userId);

        return true;
    }

    public async Task<List<string>> GetAvailableSlotsAsync(
        int barberId,
        DateTime appointmentDate,
        int durationMinutes,
        CancellationToken cancellationToken = default)
    {
        var dayOfWeek = (Domain.Enums.DayOfWeek)appointmentDate.DayOfWeek;
        var barberSchedule = await _barberScheduleRepository.GetByBarberIdAndDayOfWeekAsync(barberId, dayOfWeek, cancellationToken);

        if (barberSchedule == null || barberSchedule.Count == 0)
        {
            return new List<string>();
        }

        var existingAppointments = await _appointmentRepository.GetByBarberIdAndDateAsync(barberId, appointmentDate, cancellationToken);
        var availableSlots = new List<string>();

        foreach (var schedule in barberSchedule)
        {
            var currentTime = schedule.StartTime;
            while (currentTime.Add(TimeSpan.FromMinutes(durationMinutes)) <= schedule.EndTime)
            {
                var slotEndTime = currentTime.Add(TimeSpan.FromMinutes(durationMinutes));
                var isAvailable = true;

                foreach (var existing in existingAppointments)
                {
                    if (AppointmentsOverlap(currentTime, slotEndTime, existing.StartTime, existing.EndTime))
                    {
                        isAvailable = false;
                        break;
                    }
                }

                if (isAvailable)
                {
                    availableSlots.Add(currentTime.ToString(@"hh\:mm"));
                }

                currentTime = currentTime.Add(TimeSpan.FromMinutes(15));
            }
        }

        return availableSlots;
    }

    private async Task ValidateNoOverlapAsync(
        Appointment appointment,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var existingAppointments = await _appointmentRepository.GetByBarberIdAndDateAsync(
            appointment.BarberId,
            appointment.AppointmentDate,
            cancellationToken);

        foreach (var existing in existingAppointments)
        {
            if (excludeId.HasValue && existing.Id == excludeId.Value)
            {
                continue;
            }

            if (AppointmentsOverlap(appointment.StartTime, appointment.EndTime, existing.StartTime, existing.EndTime))
            {
                throw new ValidationException("Appointment overlaps with an existing appointment for the same barber.");
            }
        }
    }

    private async Task ValidateBarberScheduleAsync(
        Appointment appointment,
        CancellationToken cancellationToken = default)
    {
        var dayOfWeek = (Domain.Enums.DayOfWeek)appointment.AppointmentDate.DayOfWeek;
        var barberSchedule = await _barberScheduleRepository.GetByBarberIdAndDayOfWeekAsync(
            appointment.BarberId,
            dayOfWeek,
            cancellationToken);

        if (barberSchedule == null || barberSchedule.Count == 0)
        {
            throw new ValidationException("Barber is not scheduled to work on this day.");
        }

        var isWithinSchedule = barberSchedule.Any(s =>
            appointment.StartTime >= s.StartTime &&
            appointment.EndTime <= s.EndTime);

        if (!isWithinSchedule)
        {
            throw new ValidationException("Appointment time is outside the barber's working schedule.");
        }
    }

    private async Task CalculateTotalsAsync(
        Appointment appointment,
        List<int> serviceIds,
        CancellationToken cancellationToken = default)
    {
        decimal totalAmount = 0;
        int totalDuration = 0;

        foreach (var serviceId in serviceIds)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId, cancellationToken);
            if (service != null)
            {
                totalAmount += service.Price;
                totalDuration += service.DurationMinutes;
            }
        }

        appointment.TotalAmount = totalAmount;
        appointment.TotalDurationMinutes = totalDuration;
        appointment.EndTime = appointment.StartTime.Add(TimeSpan.FromMinutes(totalDuration));
    }

    private async Task AddAppointmentServicesAsync(
        int appointmentId,
        List<int> serviceIds,
        CancellationToken cancellationToken = default)
    {
        foreach (var serviceId in serviceIds)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId, cancellationToken);
            if (service != null)
            {
                var appointmentService = new Domain.Entities.AppointmentService
                {
                    AppointmentId = appointmentId,
                    ServiceId = serviceId,
                    Price = service.Price,
                    DurationMinutes = service.DurationMinutes,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = _currentUserService.GetUserId(),
                    IsDeleted = false
                };

                await _appointmentServiceRepository.CreateAsync(appointmentService, cancellationToken);
            }
        }
    }

    private async Task UpdateAppointmentServicesAsync(
        int appointmentId,
        List<int> serviceIds,
        CancellationToken cancellationToken = default)
    {
        var existingServices = await _appointmentServiceRepository.GetByAppointmentIdAsync(appointmentId, cancellationToken);

        foreach (var existing in existingServices)
        {
            await _appointmentServiceRepository.DeleteAsync(existing, cancellationToken);
        }

        await AddAppointmentServicesAsync(appointmentId, serviceIds, cancellationToken);
    }

    private static bool AppointmentsOverlap(TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
    {
        return start1 < end2 && start2 < end1;
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
