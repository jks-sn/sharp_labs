// Program.cs

using System.IO.Abstractions;
using System.Threading.Tasks;
using Hackathon.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Hackathon.HostedServices;
using Hackathon.Interface;
using Hackathon.Strategy;
using Hackathon.Options;
using Hackathon.Preferences;
using Hackathon.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hackathon;
class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Warning);
                logging.AddFilter("Microsoft", LogLevel.Warning);
                logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<DataLoaderOptions>(context.Configuration.GetSection("DataLoaderOptions"));
                services.Configure<HackathonOptions>(context.Configuration.GetSection("HackathonOptions"));
                services.Configure<HRManagerOptions>(context.Configuration.GetSection("HRManagerOptions"));
                services.Configure<ConnectionOptions>(context.Configuration.GetSection("ConnectionOptions"));
                
                services.AddSingleton<IFileSystem, FileSystem>();
                
                services.AddSingleton<IDataLoader, DataLoader>();
                services.AddSingleton<IHRDirector, HRDirector>();
                services.AddSingleton<IHRManager, HRManager>();
                services.AddScoped<IHackathon, Services.Hackathon>();
                services.AddTransient<IAssignmentStrategy, GaleShapleyStrategy>();
                services.AddTransient<IAssignmentStrategy, RandomAssignmentStrategy>();
                services.AddSingleton<IAssignmentStrategyFactory, StrategyFactory>();

                services.AddSingleton<IPreferenceGenerator, RandomPreferenceGenerator>();
                
                services.AddScoped<HackathonPrinter>();
                
                services.AddDbContext<HackathonDbContext>((serviceProvider, optionsBuilder) =>
                {
                    var connectionOptions = serviceProvider.GetRequiredService<IOptions<ConnectionOptions>>().Value;
                    optionsBuilder.UseNpgsql(connectionOptions.DefaultConnection)
                        .EnableDetailedErrors(false)
                        .EnableSensitiveDataLogging(false);
                });
                
                services.AddHostedService<HackathonHostedService>();
            })
            .Build();
        
        await host.RunAsync();
    }
}