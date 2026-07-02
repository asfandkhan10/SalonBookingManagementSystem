using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SalonBookingSystem.Persistence.Context;

namespace SalonBookingSystem.Persistence.Context;

public static class DbInitializer
{
    public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Apply pending migrations
        await context.Database.MigrateAsync();

        // Seed roles
        if (!await roleManager.RoleExistsAsync("Administrator"))
        {
            await roleManager.CreateAsync(new IdentityRole("Administrator"));
        }

        if (!await roleManager.RoleExistsAsync("Receptionist"))
        {
            await roleManager.CreateAsync(new IdentityRole("Receptionist"));
        }

        if (!await roleManager.RoleExistsAsync("Customer"))
        {
            await roleManager.CreateAsync(new IdentityRole("Customer"));
        }
    }
}
