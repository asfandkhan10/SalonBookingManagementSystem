using SalonBookingSystem.Application.DTOs.BarberSchedule;

namespace SalonBookingSystem.Application.Interfaces;

public interface IBarberScheduleService
{
    Task<BarberScheduleResponse> CreateScheduleAsync(
        CreateBarberScheduleRequest request,
        CancellationToken cancellationToken = default);

    Task<BarberScheduleResponse?> UpdateScheduleAsync(
        int id,
        UpdateBarberScheduleRequest request,
        CancellationToken cancellationToken = default);

    Task<BarberScheduleResponse?> GetScheduleByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<List<BarberScheduleResponse>> GetBarberSchedulesAsync(
        int barberId,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteScheduleAsync(
        int id,
        CancellationToken cancellationToken = default);
}
