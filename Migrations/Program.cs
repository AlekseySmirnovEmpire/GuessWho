using Core.Database;
using Core.Server.Database;
using Core.Utils;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

DotEnvUtil.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(
        Environment.GetEnvironmentVariable("DBConnectionString"),
        b => b.MigrationsAssembly("Migrations"));
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});

var app = builder.Build();

try
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetService<ApplicationDbContext>()!.Database.Migrate();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}
