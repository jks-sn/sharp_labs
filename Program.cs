// Program.cs
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Hackathon.Model;
using Hackathon.Strategy;

namespace HackathonSimulation
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<DataLoader>();
                    services.AddSingleton<HRDirector>();
                    services.AddSingleton<IAssignmentStrategy, GaleShapleyStrategy>();
                    services.AddSingleton<HRManager>();

                    services.AddHostedService<HackathonHostedService>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}