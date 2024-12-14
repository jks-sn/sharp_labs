// HRManagerService/Consumers/ParticipantWithWishlistConsumer.cs

using System.ComponentModel.DataAnnotations.Schema;
using Entities;
using HRDirectorService.Entities;
using HRDirectorService.Entities.Consts;
using HRDirectorService.Interfaces;
using HRDirectorService.Services;
using MassTransit;
using Messages;

namespace HRDirectorService.Consumers;

[NotMapped]
public class ParticipantWithWishlistConsumer(
    ILogger<ParticipantWithWishlistConsumer> logger,
    IParticipantRepository participantRepo,
    IWishlistRepository wishlistRepo,
    HRDirectorOrchestrationService orchestration)
    : IConsumer<IParticipantWithWishlist>
{
    public async Task Consume(ConsumeContext<IParticipantWithWishlist> context)
    {
        var msg = context.Message;
        logger.LogInformation("HRManager received participant+wishlist: ParticipantId={ParticipantId}, ParticipantTitle={ParticipantTitle}, HackathonId={HackathonId}", 
                               msg.ParticipantId, msg.ParticipantTitle, msg.HackathonId);
        
        var participant = new Participant
        {
            Id = msg.ParticipantId,
            Title = ParticipantTitleExtensions.FromString(msg.ParticipantTitle),
            Name = msg.ParticipantName,
            HackathonId = msg.HackathonId
        };
        await participantRepo.AddParticipantAsync(participant);
        
        var wishlist = new Wishlist
        {
            ParticipantId = participant.Id,
            ParticipantTitle = participant.Title,
            DesiredParticipants = msg.DesiredParticipants,
            HackathonId = msg.HackathonId
        };
        await wishlistRepo.AddWishlistAsync(wishlist);
        
        logger.LogInformation("ParticipantWithWishlist saved: participantId={ParticipantId}, wishlistCount={Count}", 
                               msg.ParticipantId, msg.DesiredParticipants.Length);
        
        orchestration.OnDataReceived(msg.HackathonId);
    }
}
