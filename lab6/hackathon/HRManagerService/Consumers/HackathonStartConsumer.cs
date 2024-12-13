using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;
using Microsoft.Extensions.Logging;
using HRManagerService.Interfaces;
using Messages;

namespace HRManagerService.Consumers;

[NotMapped]
public class HackathonStartConsumer(
    ILogger<HackathonStartConsumer> logger,
    ITeamBuildingOrchestrationService orchestration)
    : IConsumer<IHackathonStarted>
{
    public Task Consume(ConsumeContext<IHackathonStarted> context)
    {
        var hackathonId = context.Message.HackathonId;
        var expectedCount = context.Message.ExpectedCount;
        logger.LogInformation("Received HackathonStarted for HackathonId={HackathonId}, ExpectedCount={ExpectedCount}",
            hackathonId, expectedCount);
        
        orchestration.OnHackathonStart(hackathonId, expectedCount);

        return Task.CompletedTask;
    }
}