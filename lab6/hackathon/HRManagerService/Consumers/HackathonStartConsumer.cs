using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;
using Microsoft.Extensions.Logging;
using HRManagerService.Interfaces;
using HRManagerService.Services;
using Messages;

namespace HRManagerService.Consumers;

[NotMapped]
public class HackathonStartConsumer(
    ILogger<HackathonStartConsumer> logger,
    TeamBuildingOrchestrationService orchestration)
    : IConsumer<IHackathonStarted>
{
    public Task Consume(ConsumeContext<IHackathonStarted> context)
    {
        var hackathonId = context.Message.HackathonId;
        var participantsNumber = context.Message.ParticipantsNumber;
        logger.LogInformation("Received HackathonStarted for HackathonId={HackathonId}, ExpectedCount={ExpectedCount}",
            hackathonId, participantsNumber);
        
        orchestration.OnHackathonStart(hackathonId, participantsNumber);

        return Task.CompletedTask;
    }
}