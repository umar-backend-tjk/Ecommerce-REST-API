using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Extensions;

public static class IdentityRegister
{
    public static void RegisterIdentity(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();
    }
}