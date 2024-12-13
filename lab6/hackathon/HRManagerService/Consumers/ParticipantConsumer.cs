using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;
using Microsoft.Extensions.Logging;
using Messages;
using HRManagerService.Interfaces;
using Entities;
using HRManagerService.Entities;
using HRManagerService.Entities.Consts;

namespace HRManagerService.Consumers;

[NotMapped]
public class ParticipantConsumer(
    ILogger<ParticipantConsumer> logger,
    IParticipantRepository participantRepo,
    ITeamBuildingOrchestrationService orchestration)
    : IConsumer<IParticipantInfo>
{
    public async Task Consume(ConsumeContext<IParticipantInfo> context)
    {
        var msg = context.Message;
        logger.LogInformation("Received participant: Id={Id}, Title={Title}, Name={Name}, HackathonId={HackathonId}",
            msg.Id, msg.Title, msg.Name, msg.HackathonId);

        var title = ParticipantTitleExtensions.FromString(msg.Title);
        var participant = new Participant(msg.Id, title, msg.Name) { HackathonId = msg.HackathonId };
        await participantRepo.AddParticipantAsync(participant);

        orchestration.OnDataReceived(participant.HackathonId);
    }
}