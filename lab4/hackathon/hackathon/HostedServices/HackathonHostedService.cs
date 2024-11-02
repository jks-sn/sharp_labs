// HostedServices/HackathonHostedService.cs

using System;
using System.Threading;
using System.Threading.Tasks;
using Hackathon.Interface;
using Hackathon.Model;
using Hackathon.Options;
using Hackathon.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection; 

namespace Hackathon.HostedServices;
public class HackathonHostedService(
    IOptions<HackathonOptions> hackathonOptions,
    IServiceScopeFactory serviceScopeFactory)
    : BackgroundService
{
    private readonly int _hackathonCount = hackathonOptions.Value.HackathonCount;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => RunHackathons(stoppingToken), stoppingToken);
    }

    private void RunHackathons(CancellationToken stoppingToken)
    {
        using (var scope = serviceScopeFactory.CreateScope())
        {
            var hackathon = scope.ServiceProvider.GetRequiredService<IHackathon>();
            var hackathonPrinter = scope.ServiceProvider.GetRequiredService<HackathonPrinter>();

            var totalHarmonic = 0.0;

            for (int i = 0; i < _hackathonCount; ++i)
            {
                var harmonic = hackathon.Run();
                totalHarmonic += harmonic;

                Console.WriteLine($"Хакатон {i + 1}: Гармоничность = {harmonic:F2}");
            }

            Console.WriteLine("Введите ID хакатона для отображения информации:");

            if (int.TryParse(Console.ReadLine(), out int hackathonId))
            {
                hackathonPrinter.PrintHackathonById(hackathonId);
            }

            hackathonPrinter.PrintAverageHarmonic();
        }
    }
}
