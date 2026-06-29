using FluentValidation;
using Microsoft.Extensions.Logging;
using SalonBookingSystem.Application.DTOs.BarberSchedule;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Application.Mappings;

namespace SalonBookingSystem.Application.Services;

public class BarberScheduleService : IBarberScheduleService
{
    private readonly IBarberScheduleRepository _scheduleRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<CreateBarberScheduleRequest> _createValidator;
    private readonly IValidator<UpdateBarberScheduleRequest> _updateValidator;
    private readonly ILogger<BarberScheduleService> _logger;

    public BarberScheduleService(
        IBarberScheduleRepository scheduleRepository,
        ICurrentUserService currentUserService,
        IValidator<CreateBarberScheduleRequest> createValidator,
        IValidator<UpdateBarberScheduleRequest> updateValidator,
        ILogger<BarberScheduleService> logger)
    {
        _scheduleRepository = scheduleRepository;
        _currentUserService = currentUserService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<BarberScheduleResponse> CreateScheduleAsync(
        CreateBarberScheduleRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, request, cancellationToken);

        var schedule = request.ToEntity();
        var userId = _currentUserService.GetUserId();
        var now = DateTime.UtcNow;

        schedule.CreatedAt = now;
        schedule.CreatedBy = userId;
        schedule.IsDeleted = false;

        await ValidateNoOverlapAsync(schedule, null, cancellationToken);

        var created = await _scheduleRepository.CreateAsync(schedule, cancellationToken);

        _logger.LogInformation("BarberSchedule {ScheduleId} created for Barber {BarberId} by {UserId}", created.Id, created.BarberId, userId);

        return created.ToResponse();
    }

    public async Task<BarberScheduleResponse?> UpdateScheduleAsync(
        int id,
        UpdateBarberScheduleRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidator, request, cancellationToken);

        var schedule = await _scheduleRepository.GetByIdAsync(id, cancellationToken);
        if (schedule is null)
        {
            return null;
        }

        schedule.ApplyUpdate(request);

        await ValidateNoOverlapAsync(schedule, id, cancellationToken);

        var userId = _currentUserService.GetUserId();
        schedule.UpdatedAt = DateTime.UtcNow;
        schedule.UpdatedBy = userId;

        await _scheduleRepository.UpdateAsync(schedule, cancellationToken);

        _logger.LogInformation("BarberSchedule {ScheduleId} updated by {UserId}", id, userId);

        return schedule.ToResponse();
    }

    public async Task<BarberScheduleResponse?> GetScheduleByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(id, cancellationToken);
        return schedule?.ToResponse();
    }

    public async Task<List<BarberScheduleResponse>> GetBarberSchedulesAsync(
        int barberId,
        CancellationToken cancellationToken = default)
    {
        var schedules = await _scheduleRepository.GetByBarberIdAsync(barberId, cancellationToken);
        return schedules.Select(s => s.ToResponse()).ToList();
    }

    public async Task<bool> DeleteScheduleAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(id, cancellationToken);
        if (schedule is null)
        {
            return false;
        }

        var userId = _currentUserService.GetUserId();
        schedule.UpdatedAt = DateTime.UtcNow;
        schedule.UpdatedBy = userId;

        await _scheduleRepository.DeleteAsync(schedule, cancellationToken);

        _logger.LogInformation("BarberSchedule {ScheduleId} soft deleted by {UserId}", id, userId);

        return true;
    }

    private async Task ValidateNoOverlapAsync(
        Domain.Entities.BarberSchedule schedule,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var existingSchedules = await _scheduleRepository.GetByBarberIdAndDayOfWeekAsync(
            schedule.BarberId,
            schedule.DayOfWeek,
            cancellationToken);

        foreach (var existing in existingSchedules)
        {
            if (excludeId.HasValue && existing.Id == excludeId.Value)
            {
                continue;
            }

            if (SchedulesOverlap(existing.StartTime, existing.EndTime, schedule.StartTime, schedule.EndTime))
            {
                throw new ValidationException("Schedule overlaps with an existing schedule for the same day.");
            }
        }
    }

    private static bool SchedulesOverlap(TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
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
