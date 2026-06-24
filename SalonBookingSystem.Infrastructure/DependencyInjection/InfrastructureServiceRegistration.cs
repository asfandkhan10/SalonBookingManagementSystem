using Microsoft.Extensions.DependencyInjection;
using SalonBookingSystem.Application.Interfaces;
using SalonBookingSystem.Infrastructure.Services;

namespace SalonBookingSystem.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
