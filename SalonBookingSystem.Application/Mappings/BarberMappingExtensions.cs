using SalonBookingSystem.Application.DTOs.Barber;
using SalonBookingSystem.Domain.Entities;

namespace SalonBookingSystem.Application.Mappings;

public static class BarberMappingExtensions
{
    public static BarberResponse ToResponse(this Barber barber)
    {
        return new BarberResponse
        {
            Id = barber.Id,
            FirstName = barber.FirstName,
            LastName = barber.LastName,
            PhoneNumber = barber.PhoneNumber,
            IsActive = barber.IsActive,
            CreatedAt = barber.CreatedAt,
            CreatedBy = barber.CreatedBy,
            UpdatedAt = barber.UpdatedAt,
            UpdatedBy = barber.UpdatedBy
        };
    }

    public static Barber ToEntity(this CreateBarberRequest request)
    {
        return new Barber
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            IsActive = true
        };
    }

    public static void ApplyUpdate(this Barber barber, UpdateBarberRequest request)
    {
        barber.FirstName = request.FirstName.Trim();
        barber.LastName = request.LastName.Trim();
        barber.PhoneNumber = request.PhoneNumber.Trim();
    }
}
