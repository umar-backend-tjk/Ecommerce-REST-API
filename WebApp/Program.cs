using Application.Interfaces;
using Application.Mapping;
using Hangfire;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

//Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

//Database
builder.Services.RegisterDatabase(builder.Configuration);

builder.Services.AddScoped<Seeder>();

//Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "Redis cache";
});

//File
builder.Services.AddScoped<IFileStorageService>(_ => new FileStorageService(builder.Environment.ContentRootPath));

//Swagger
builder.Services.RegisterSwagger();

//Identity
builder.Services.RegisterIdentity();

//Automapper
builder.Services.AddAutoMapper(typeof(ApplicationProfile));

//Hangfire
builder.Services.RegisterHangfire(builder.Configuration);

//Services
builder.Services.RegisterServices();

//Configure Smtp
builder.Services.ConfigureSmtp(builder.Configuration);

//Authentication
builder.Services.RegisterAuthentication(builder.Configuration);

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var context = serviceProvider.GetRequiredService<DataContext>();
    var seed = serviceProvider.GetRequiredService<Seeder>();
    await context.Database.MigrateAsync();
    await seed.SeedRoles();
    await seed.SeedAdmin();
}

app.UseHangfireDashboard();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();