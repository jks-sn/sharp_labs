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
public class WishlistConsumer(
    ILogger<WishlistConsumer> logger,
    IParticipantRepository participantRepo,
    IWishlistRepository wishlistRepo,
    ITeamBuildingOrchestrationService orchestration)
    : IConsumer<IWishlistInfo>
{

    public async Task Consume(ConsumeContext<IWishlistInfo> context)
    {
        var msg = context.Message;
        logger.LogInformation("Received wishlist from ParticipantId={ParticipantId}, Title={Title}, HackathonId={HackathonId}",
            msg.ParticipantId, msg.ParticipantTitle, msg.HackathonId);

        var participant = await participantRepo.GetByIdAsync(msg.ParticipantId, ParticipantTitleExtensions.FromString(msg.ParticipantTitle));
        if (participant == null)
        {
            throw new InvalidOperationException(
                $"Participant ({msg.ParticipantId}, {msg.ParticipantTitle}) not found in DB. Retrying...");
        }
        
        var wishlist = new Wishlist
        {
            ParticipantId = msg.ParticipantId,
            ParticipantTitle = ParticipantTitleExtensions.FromString(msg.ParticipantTitle),
            DesiredParticipants = msg.DesiredParticipants,
            HackathonId = msg.HackathonId
        };

        await wishlistRepo.AddWishlistAsync(wishlist);

        orchestration.OnDataReceived(wishlist.HackathonId);
    }
}