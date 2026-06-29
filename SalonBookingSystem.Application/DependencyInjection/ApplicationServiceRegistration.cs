using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Application.Services;
using SalonBookingSystem.Application.Validators;

namespace SalonBookingSystem.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateCustomerValidator>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IBarberService, BarberService>();
        services.AddScoped<IBarberScheduleService, BarberScheduleService>();

        return services;
    }
}
