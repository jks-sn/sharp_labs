// HackathonHostedService.cs

using System;
using System.Threading;
using System.Threading.Tasks;
using Hackathon.Model;
using Hackathon.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Hackathon;
public class HackathonHostedService(HRDirector hrDirector, HRManager hrManager, IOptions<HackathonOptions> hackathonOptions) : BackgroundService
{
    private readonly HRDirector _hrDirector = hrDirector;
    private readonly HRManager _hrManager = hrManager;
    private readonly int _hackathonCount = hackathonOptions.Value.HackathonCount;
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(RunHackathons, stoppingToken);
    }

    private void RunHackathons()
    {
        double totalHarmonic = 0;

        for (int i = 0; i < _hackathonCount; i++)
        {
            var hackathon = new Hackathon.Model.Hackathon(_hrManager, _hrDirector);
            double harmonic = hackathon.Run();
            totalHarmonic += harmonic;
            Console.WriteLine($"Хакатон {i + 1}: Гармоничность = {harmonic:F2}");
        }

        double averageHarmonic = totalHarmonic / _hackathonCount;
        Console.WriteLine($"\nСредняя гармоничность по {_hackathonCount} хакатонам: {averageHarmonic:F2}");
    }
    
}
