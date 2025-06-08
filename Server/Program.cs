using Core.Database;
using Core.Providers;
using Core.Repositories;
using Core.Services;
using Core.Services.Interfaces;
using Core.Utils;
using Infrastructure.Providers.Auth;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Targets;
using NLog.Web;
using Server.Controllers;

var builder = WebApplication.CreateBuilder(args);

DotEnvUtil.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

#region Настройка логгера

// Nlog
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

// Настройка NLog с использованием выбранного конфигурационного файла
LogManager.Setup()
    .LoadConfigurationFromFile(!string.IsNullOrEmpty(env) && env != Environments.Production
        ? Path.Combine(AppContext.BaseDirectory, $"nlog.{env}.config")
        : Path.Combine(AppContext.BaseDirectory, "nlog.config"))
    .GetCurrentClassLogger();

var logger = LogManager.GetCurrentClassLogger();

// Create NLog Directory
var fileTarget = (FileTarget)LogManager.Configuration.FindTargetByName("file");
Directory.CreateDirectory(Path.GetDirectoryName(
    fileTarget.FileName.Render(
        new LogEventInfo
        {
            TimeStamp = DateTime.Now
        }))!);
builder.Logging.ClearProviders().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
builder.WebHost.UseNLog();

#endregion

#region Настройка БД

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(
        Environment.GetEnvironmentVariable("DBConnectionString"),
        b => b.MigrationsAssembly("Migrations"));
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});

#endregion

#region Контроллеры

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins(Environment.GetEnvironmentVariable("CLIENT_URL")!)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
builder.Services.AddScoped<AuthController>();

#endregion

#region Провайдеры

builder.Services.AddScoped<IAuthProvider, JwtAuthProvider>();

#endregion

#region Сервисы

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

#endregion

#region Репозитории

builder.Services.AddScoped<IUserRepository, UserRepository>();

#endregion

#region Сервисы для приложения

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#region Старт сервера

try
{
    logger.Debug($"Main method execute. Environment is {env}");

    // Start Jobs.
    // scheduler.Start();

    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex.Message, "Stopped program caz of exception.");
    throw;
}
finally
{
    LogManager.Shutdown();
}

#endregion