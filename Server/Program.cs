using Core.Server.Database;
using Core.Server.Managers;
using Core.Server.Providers;
using Core.Server.Repositories;
using Core.Server.Services;
using Core.Server.Services.Interfaces;
using Core.Settings;
using Core.Utils;
using Infrastructure.Managers;
using Infrastructure.Providers.Auth;
using Infrastructure.Providers.MessageBus;
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

builder.Services.AddScoped<AuthController>();
builder.Services.AddScoped<UsersController>();
builder.Services.AddScoped<ImagesController>();
builder.Services.AddScoped<ConfirmController>();
builder.Services.AddScoped<PasswordController>();

#endregion

#region Провайдеры

builder.Services.AddScoped<IAuthProvider, JwtAuthProvider>();

#endregion

#region Сервисы

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IEmailTemplatingService, SubstitutionEmailTemplatingService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<FileDataService>();

#endregion

#region Репозитории

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailSendingQueueRepository, EmailSendingQueueRepository>();
builder.Services.AddScoped<IFileDataRepository, FileDataRepository>();

#endregion

#region Сервисы для приложения

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp",
        policy => policy
            .WithOrigins(Environment.GetEnvironmentVariable("CLIENT_URL")!) // URL вашего Blazor-приложения
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

#endregion

#region Синглтоны

builder.Services.AddSingleton<EmailSettings>();
builder.Services.AddSingleton<IMessageBusProvider, RedisMessageBusProvider>();
builder.Services.AddSingleton<IMessageBusManager, MessageBusManager>();

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazorApp");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#region Старт сервера

try
{
    logger.Debug($"Main method execute. Environment is {env}");

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