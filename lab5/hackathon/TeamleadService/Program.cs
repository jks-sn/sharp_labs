using Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Options;
using Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TeamleadService;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(configure => configure.AddConsole());

        services.Configure<DataLoaderOptions>(context.Configuration.GetSection("DataLoaderOptions"));
        services.AddSingleton<DataLoader>();

        // Retrieve Participant configuration
        var id = int.Parse(context.Configuration["Participant__Id"] ?? "1");
        var title = context.Configuration["Participant__Title"] ?? "Teamlead";
        var name = context.Configuration["Participant__Name"] ?? "Unknown";

        // Create Participant instance
        var participant = new Participant(id, title, name);

        // Configure ServiceSettings
        services.Configure<ServiceSettings>(settings =>
        {
            settings.Participant = participant;
        });

        // Register services
        services.AddSingleton<TeamleadService.TeamleadService>();
        services.AddHostedService<TeamleadBackgroundService>();

        // Configure HttpClient for HRManager
        services.AddHttpClient("HrManagerClient", client =>
        {
            client.BaseAddress = new Uri(context.Configuration["HrManagerUri"] ?? "http://hr_manager:8081/");
        });
    });

var host = builder.Build();

await host.RunAsync();