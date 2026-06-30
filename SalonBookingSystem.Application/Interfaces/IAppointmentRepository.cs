using SalonBookingSystem.Domain.Entities;

namespace SalonBookingSystem.Application.Interfaces;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<List<Appointment>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<List<Appointment>> GetByBarberIdAsync(int barberId, CancellationToken cancellationToken = default);

    Task<List<Appointment>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);

    Task<List<Appointment>> GetByBarberIdAndDateAsync(
        int barberId,
        DateTime appointmentDate,
        CancellationToken cancellationToken = default);

    Task<Appointment> CreateAsync(Appointment appointment, CancellationToken cancellationToken = default);

    Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default);

    Task DeleteAsync(Appointment appointment, CancellationToken cancellationToken = default);
}
