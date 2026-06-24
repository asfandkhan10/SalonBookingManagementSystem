using SalonBookingSystem.Application.DTOs.Customer;
using SalonBookingSystem.Domain.Entities;

namespace SalonBookingSystem.Application.Mappings;

public static class CustomerMappingExtensions
{
    public static CustomerResponse ToResponse(this Customer customer)
    {
        return new CustomerResponse
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            ApplicationUserId = customer.ApplicationUserId,
            CreatedAt = customer.CreatedAt,
            CreatedBy = customer.CreatedBy,
            UpdatedAt = customer.UpdatedAt,
            UpdatedBy = customer.UpdatedBy
        };
    }

    public static Customer ToEntity(this CreateCustomerRequest request)
    {
        return new Customer
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            PhoneNumber = request.PhoneNumber.Trim()
        };
    }

    public static void ApplyUpdate(this Customer customer, UpdateCustomerRequest request)
    {
        customer.FirstName = request.FirstName.Trim();
        customer.LastName = request.LastName.Trim();
        customer.Email = request.Email.Trim();
        customer.PhoneNumber = request.PhoneNumber.Trim();
    }
}
