using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Persistence.Context;
using SalonBookingSystem.Persistence.Repositories;

namespace SalonBookingSystem.Persistence.DependencyInjection;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IBarberRepository, BarberRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();

        return services;
    }
}
