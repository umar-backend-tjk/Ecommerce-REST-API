using Application.Mapping;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

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

//Swagger
builder.Services.RegisterSwagger();

//Identity
builder.Services.RegisterIdentity();

//Automapper
builder.Services.AddAutoMapper(typeof(ApplicationProfile));

//Services
builder.Services.RegisterServices();

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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();