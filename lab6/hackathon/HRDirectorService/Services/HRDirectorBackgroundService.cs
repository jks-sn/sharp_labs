//HRDirectorService/Services/HRDirectorBackgroundService.cs

using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;
using Entities;
using HRDirectorService.Interfaces;
using HRDirectorService.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MassTransit;
using Messages;
using Microsoft.Extensions.Options;
using Hackathon = HRDirectorService.Entities.Hackathon;

namespace HRDirectorService;

[NotMapped]
public class HRDirectorBackgroundService(IBus bus, ILogger<HRDirectorBackgroundService> logger,  IServiceProvider serviceProvider, IOptions<HackathonOptions> options) : BackgroundService
{
    private readonly HackathonOptions _options = options.Value;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var hackathonRepo = scope.ServiceProvider.GetRequiredService<IHackathonRepository>();
        int hackathonId = 1;
        for (var i = 1; i <= _options.HackathonsNumber; i++)
        {
            await hackathonRepo.CreateHackathonAsync(new Hackathon
            {
                Id = hackathonId,
                MeanSatisfactionIndex = 0.0
            });
            
            await bus.Publish<IHackathonStarted>(new { HackathonId = i, ParticipantsNumber = _options.ParticipantsNumber }, stoppingToken);
            logger.LogWarning("Announced start of Hackathon {HackathonId}", i);
        }
    }
}