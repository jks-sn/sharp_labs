// HRManagerService/Consumers/ParticipantWithWishlistConsumer.cs

using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;
using Messages;
using HRManagerService.Repositories;
using HRManagerService.Data;
using HRManagerService.Entities;
using HRManagerService.Entities.Consts;
using HRManagerService.Interfaces;
using HRManagerService.Services;

namespace HRManagerService.Consumers;

[NotMapped]
public class ParticipantWithWishlistConsumer(
    ILogger<ParticipantWithWishlistConsumer> logger,
    IParticipantRepository participantRepo,
    IWishlistRepository wishlistRepo,
    TeamBuildingOrchestrationService orchestration)
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
