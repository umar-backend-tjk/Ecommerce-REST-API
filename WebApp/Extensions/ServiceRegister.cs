using Application.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;

namespace WebApp.Extensions;

public static class ServiceRegister
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryService, CategoryService>();
    }
}