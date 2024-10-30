// HackathonHostedService.cs

using System;
using System.Threading;
using System.Threading.Tasks;
using Hackathon.Interface;
using Hackathon.Model;
using Hackathon.Options;
using Hackathon.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Hackathon.HostedServices;
public class HackathonHostedService(
    IHackathon hackathon,
    IOptions<HackathonOptions> hackathonOptions)
    : BackgroundService
{
    private readonly int _hackathonCount = hackathonOptions.Value.HackathonCount;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => RunHackathons(stoppingToken), stoppingToken);
    }

    private void RunHackathons(CancellationToken stoppingToken)
    {
        double totalHarmonic = 0;

        for (int i = 0; i < _hackathonCount; ++i)
        {
            double harmonic = hackathon.Run();
            totalHarmonic += harmonic;

            Console.WriteLine($"Хакатон {i + 1}: Гармоничность = {harmonic:F2}");
        }

        double averageHarmonic = totalHarmonic / _hackathonCount;
        Console.WriteLine($"\nСредняя гармоничность по {_hackathonCount} хакатонам: {averageHarmonic:F2}");
    }
}
