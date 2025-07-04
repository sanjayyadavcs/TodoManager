using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TodoManager.DAL.EF;
using TodoManager.DAL.Entities;

namespace TodoManager.DAL.Seed;

public class DataSeeder
{
    public static async Task SeedDefaultDataAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataSeeder>>();

        try
        {
            // Ensure required roles exist
            var roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new Role(role));
                    if (!roleResult.Succeeded)
                    {
                        logger.LogError("[DataSeeder] Failed to create role: {Role}. Errors: {Errors}", role, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                    else
                    {
                        logger.LogInformation("[DataSeeder] Created role: {Role}", role);
                    }
                }
            }

            // Seed default admin user if not exists
            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = "admin",
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true
                };

                var userResult = await userManager.CreateAsync(adminUser, "Admin@123");

                if (userResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    await userManager.AddToRoleAsync(adminUser, "User");
                    logger.LogInformation("[DataSeeder] Admin user created and assigned roles.");
                }
                else
                {
                    logger.LogError("[DataSeeder] Failed to create admin user. Errors: {Errors}", string.Join(", ", userResult.Errors.Select(e => e.Description)));
                    throw new Exception("Admin user creation failed.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "[DataSeeder] Seeding failed with exception.");
            throw;
        }
    }
}
