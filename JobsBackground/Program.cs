using BackgroundJobs.Jobs;
using Core.Database;
using Core.Managers;
using Core.Providers;
using Core.Repositories;
using Core.Services;
using Core.Services.Interfaces;
using Core.Settings;
using Core.Utils;
using Infrastructure.Managers;
using Infrastructure.Providers.MessageBus;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Targets;
using NLog.Web;
using Quartz;

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

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IEmailSendingQueueRepository, EmailSendingQueueRepository>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

builder.Services.AddSingleton<IMessageBusProvider, RedisMessageBusProvider>();
builder.Services.AddSingleton<IMessageBusManager, MessageBusManager>();
builder.Services.AddSingleton<EmailSettings>();

builder.Services.AddQuartz(q =>
{
    // Регистрируем ваши джобы
    q.AddJob<EmailSendingJob>(j => j
        .WithIdentity(nameof(EmailSendingJob))
        .StoreDurably()
    );

    // Настройка триггера
    q.AddTrigger(t => t
        .ForJob(nameof(EmailSendingJob))
        .WithIdentity(nameof(EmailSendingJob))
        .WithSimpleSchedule(s => s
            .WithIntervalInSeconds(15) // Интервал выполнения
            .RepeatForever()
        )
        .StartNow()
    );
});
builder.Services.AddQuartzHostedService(q => 
{
    q.WaitForJobsToComplete = true;
    q.StartDelay = TimeSpan.FromSeconds(5); // Задержка перед стартом
});

var app = builder.Build();

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