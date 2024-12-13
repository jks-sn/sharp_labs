//HRDirectorService/Services/HRDirectorBackgroundService.cs

using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;
using Entities;
using HRDirectorService.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MassTransit;
using Messages;
using Hackathon = HRDirectorService.Entities.Hackathon;

namespace HRDirectorService;

[NotMapped]
public class HRDirectorBackgroundService(IBus bus, ILogger<HRDirectorBackgroundService> logger,  IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var hackathonRepo = scope.ServiceProvider.GetRequiredService<IHackathonRepository>();
        int hackathonId = 1;
        for (var i = 1; i <= 1; i++)
        {
            await hackathonRepo.CreateHackathonAsync(new Hackathon
            {
                Id = hackathonId,
                MeanSatisfactionIndex = 0.0
            });
            
            await bus.Publish<IHackathonStarted>(new { HackathonId = i }, stoppingToken);
            logger.LogInformation("Announced start of Hackathon {HackathonId}", i);
        }
    }
}