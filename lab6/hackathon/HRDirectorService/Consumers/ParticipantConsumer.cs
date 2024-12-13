//HRDirectorService/Consumers/ParticipantConsumer.cs

using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Entities;
using HRDirectorService.Repositories;
using HRDirectorService.Interfaces;
using HRDirectorService.Services;
using Messages;
using Hackathon = HRDirectorService.Entities.Hackathon;
using Participant = HRDirectorService.Entities.Participant;
using ParticipantTitleExtensions = HRDirectorService.Entities.Consts.ParticipantTitleExtensions;

namespace HRDirectorService.Consumers;

[NotMapped]
public class ParticipantConsumer(
    ILogger<ParticipantConsumer> logger,
    IParticipantRepository participantRepo,
    IHackathonRepository hackathonRepo,
    HRDirectorOrchestrationService orchestration)
    : IConsumer<IParticipantInfo>
{
    public async Task Consume(ConsumeContext<IParticipantInfo> context)
    {
        var msg = context.Message;
        logger.LogInformation("HRDirector received participant: Id={Id}, Title={Title}, Name={Name}, HackathonId={HackathonId}",
            msg.Id, msg.Title, msg.Name, msg.HackathonId);
        
        var title = ParticipantTitleExtensions.FromString(msg.Title);
        var participant = new Participant(msg.Id, title, msg.Name)
        {
            HackathonId = msg.HackathonId
        };
        await participantRepo.AddParticipantAsync(participant);

        // Создадим Hackathon, если его нет
        var hackathon = await hackathonRepo.GetByIdAsync(msg.HackathonId);
        if (hackathon == null)
        {
            hackathon = new Hackathon { Id = msg.HackathonId };
            await hackathonRepo.CreateHackathonAsync(hackathon);
        }

        orchestration.OnDataReceived(msg.HackathonId);
    }
}