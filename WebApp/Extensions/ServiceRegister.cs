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
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}