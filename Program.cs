// Program.cs

using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Hackathon.HostedServices;
using Hackathon.Interface;
using Hackathon.Strategy;
using Hackathon.Options;
using Hackathon.Preferences;
using Hackathon.Services;
using Microsoft.Extensions.Configuration;

namespace Hackathon;
class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<DataLoaderOptions>(context.Configuration.GetSection("DataLoaderOptions"));
                services.Configure<HackathonOptions>(context.Configuration.GetSection("HackathonOptions"));
                services.Configure<HRManagerOptions>(context.Configuration.GetSection("HRManagerOptions"));
                
                services.AddSingleton<IDataLoader, DataLoader>();
                services.AddSingleton<IHRDirector, HRDirector>();
                services.AddSingleton<IHRManager, HRManager>();
                services.AddSingleton<IHackathon, Services.Hackathon>();
                services.AddTransient<IAssignmentStrategy, GaleShapleyStrategy>();
                services.AddTransient<IAssignmentStrategy, RandomAssignmentStrategy>();
                services.AddSingleton<IAssignmentStrategyFactory, StrategyFactory>();
                
                services.AddSingleton<IPreferenceGenerator, RandomPreferenceGenerator>();
                
                services.AddHostedService<HackathonHostedService>();
            })
            .Build();

        await host.RunAsync();
    }
}