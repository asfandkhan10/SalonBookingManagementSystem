using SalonBookingSystem.Application.DTOs.Appointment;

namespace SalonBookingSystem.Application.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentResponse> CreateAppointmentAsync(
        CreateAppointmentRequest request,
        CancellationToken cancellationToken = default);

    Task<AppointmentResponse?> UpdateAppointmentAsync(
        int id,
        UpdateAppointmentRequest request,
        CancellationToken cancellationToken = default);

    Task<AppointmentResponse?> GetAppointmentByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<List<AppointmentResponse>> GetAppointmentsAsync(
        CancellationToken cancellationToken = default);

    Task<List<AppointmentResponse>> GetAppointmentsByBarberIdAsync(
        int barberId,
        CancellationToken cancellationToken = default);

    Task<List<AppointmentResponse>> GetAppointmentsByCustomerIdAsync(
        int customerId,
        CancellationToken cancellationToken = default);

    Task<bool> CancelAppointmentAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<AppointmentResponse?> RescheduleAppointmentAsync(
        int id,
        RescheduleAppointmentRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> CompleteAppointmentAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<List<string>> GetAvailableSlotsAsync(
        int barberId,
        DateTime appointmentDate,
        int durationMinutes,
        CancellationToken cancellationToken = default);
}
