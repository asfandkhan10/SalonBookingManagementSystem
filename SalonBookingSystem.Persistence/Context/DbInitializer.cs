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

        // Seed default admin user
        var adminEmail = "admin@salon.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Administrator");
            }
        }
    }
}
