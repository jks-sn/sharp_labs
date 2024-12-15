//ParticipantService/Consumers/HackathonStartedConsumer.cs

using MassTransit;
using Messages;
using ParticipantService.Entities.Consts;

public class HackathonStartedConsumer(
    ILogger<HackathonStartedConsumer> logger,
    ParticipantService.Services.ParticipantService participantService,
    IBus bus)
    : IConsumer<IHackathonStarted>
{
    public async Task Consume(ConsumeContext<IHackathonStarted> context)
    {
        var hackathonId = context.Message.HackathonId;
        logger.LogWarning("HackathonStartedConsumer запускается.");

        var participant = participantService.GetParticipant();
        var wishlist = participant.MakeWishlist(participantService.GetProbableTeammates());
        
        await bus.Publish<IParticipantWithWishlist>(new
        {
            ParticipantId = participant.Id,
            ParticipantTitle = ParticipantTitleExtensions.ToString(participant.Title),
            ParticipantName = participant.Name,
            HackathonId = hackathonId,
            DesiredParticipants = wishlist.DesiredParticipants
        });

        logger.LogInformation("Participant {ParticipantId} data and wishlist for hackathon {HackathonId} published.", participant.Id, hackathonId);
    }
}