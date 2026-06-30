using SalonBookingSystem.Application.DTOs.AppointmentService;

namespace SalonBookingSystem.Application.Interfaces;

public interface IAppointmentServiceService
{
    Task<AppointmentServiceResponse> AddServiceToAppointmentAsync(
        CreateAppointmentServiceRequest request,
        CancellationToken cancellationToken = default);

    Task<List<AppointmentServiceResponse>> GetAppointmentServicesAsync(
        int appointmentId,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveServiceFromAppointmentAsync(
        int id,
        CancellationToken cancellationToken = default);
}
