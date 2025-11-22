using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Extensions;

public static class DataContextRegister
{
    public static void RegisterDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    }
}