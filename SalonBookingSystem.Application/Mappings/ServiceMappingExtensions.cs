using SalonBookingSystem.Application.DTOs.Service;
using SalonBookingSystem.Domain.Entities;

namespace SalonBookingSystem.Application.Mappings;

public static class ServiceMappingExtensions
{
    public static ServiceResponse ToResponse(this Service service)
    {
        return new ServiceResponse
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            Price = service.Price,
            DurationMinutes = service.DurationMinutes,
            CreatedAt = service.CreatedAt,
            CreatedBy = service.CreatedBy,
            UpdatedAt = service.UpdatedAt,
            UpdatedBy = service.UpdatedBy
        };
    }

    public static Service ToEntity(this CreateServiceRequest request)
    {
        return new Service
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Price = request.Price,
            DurationMinutes = request.DurationMinutes
        };
    }

    public static void ApplyUpdate(this Service service, UpdateServiceRequest request)
    {
        service.Name = request.Name.Trim();
        service.Description = request.Description.Trim();
        service.Price = request.Price;
        service.DurationMinutes = request.DurationMinutes;
    }
}
