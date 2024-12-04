using HRDirectorService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Repositories;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(configure => configure.AddConsole());

        services.AddControllers();

        // Configure database connection
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.AddSingleton<HRDirectorService.HRDirectorService>();

        services.AddHostedService<HRDirectorBackgroundService>();

        services.AddHttpClient();

        services.AddRouting();

        services.AddEndpointsApiExplorer();
    });

var host = builder.Build();

host.Services.GetRequiredService<ILogger<Program>>().LogInformation("Starting HRDirectorService...");

await host.RunAsync();