using SalonBookingSystem.Domain.Entities;

namespace SalonBookingSystem.Application.Interfaces;

public interface IAppointmentServiceRepository
{
    Task<AppointmentService?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<List<AppointmentService>> GetByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default);

    Task<AppointmentService> CreateAsync(AppointmentService appointmentService, CancellationToken cancellationToken = default);

    Task DeleteAsync(AppointmentService appointmentService, CancellationToken cancellationToken = default);
}
