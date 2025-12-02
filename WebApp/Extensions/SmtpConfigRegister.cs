using Application.DTOs.SMTPConfig;

namespace WebApp.Extensions;

public static class SmtpConfigRegister
{
    public static void ConfigureSmtp(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection("SMTPConfig");
        services.Configure<SmtpConfigurationModel>(config);
    }
}