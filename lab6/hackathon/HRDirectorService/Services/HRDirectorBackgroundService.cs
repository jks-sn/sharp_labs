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
public class HRDirectorBackgroundService(IBus bus, ILogger<HRDirectorBackgroundService> logger, IHackathonRepository hackathonRepo, IOptions<HackathonOptions> options) : BackgroundService
{
    private readonly HackathonOptions _options = options.Value;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        for (var hackathonId = 1; hackathonId <= _options.HackathonsNumber; hackathonId++)
        {
            await hackathonRepo.CreateHackathonAsync(new Hackathon
            {
                HackathonId = hackathonId,
                MeanSatisfactionIndex = 0.0
            });
            
            await bus.Publish<IHackathonStarted>(new { HackathonId = hackathonId, ParticipantsNumber = _options.ParticipantsNumber }, stoppingToken);
            logger.LogWarning("Announced start of Hackathon {HackathonId}", hackathonId);
        }
    }
}