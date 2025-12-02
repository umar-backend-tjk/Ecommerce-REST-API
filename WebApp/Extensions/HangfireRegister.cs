using Hangfire;
using Hangfire.Redis.StackExchange;

namespace WebApp.Extensions;

public static class HangfireRegister
{
    public static void RegisterHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
        {
            config.UseRedisStorage(configuration.GetConnectionString("RedisConnection"),
                new RedisStorageOptions
                {
                    Db = 1
                });
        });
        
        services.AddHangfireServer();
    }
}