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
        
        var participant = new Participant
        {
            ParticipantId = msg.ParticipantId,
            Title = ParticipantTitleExtensions.FromString(msg.ParticipantTitle),
            Name = msg.ParticipantName,
            HackathonId = msg.HackathonId
        };
        await participantRepo.AddParticipantAsync(participant);
        
        var wishlist = new Wishlist
        {
            ParticipantId = participant.Id,
            DesiredParticipants = msg.DesiredParticipants,
        };
        await wishlistRepo.AddWishlistAsync(wishlist);
        
        orchestration.OnDataReceived(msg.HackathonId);
    }
}
