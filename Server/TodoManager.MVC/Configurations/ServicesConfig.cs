using TodoManager.DAL.Interfaces;
using TodoManager.DAL.Repositories;
using TodoManager.Services.Interfaces;
using TodoManager.Services.ServiceImplementation;

namespace TodoManager.MVC.Configurations;

public static class ServicesConfig
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        services.AddScoped<ITodoRepository, TodoRepository>();
        services.AddScoped<ITodoService, TodoService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
