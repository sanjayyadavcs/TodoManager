using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TodoManager.DAL.EF;
using TodoManager.DAL.Entities;
using TodoManager.DAL.Seed;
using TodoManager.MVC.Configurations;
using TodoManager.MVC.Middlewares;

namespace TodoManager.MVC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Logging
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });

            // DbContext
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging();
            });

            // Identity
            builder.Services.AddIdentity<User, Role>().AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders();

            // Add Services
            builder.Services
                .AddProjectServices()
                .AddJwtAuthentication(builder.Configuration)
                .AddSwaggerWithJwt()
                .AddCustomCors(builder.Configuration);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            // Seed data
            await DataSeeder.SeedDefaultDataAsync(app.Services);

            app.UseCors("CustomCorsPolicy");
            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
