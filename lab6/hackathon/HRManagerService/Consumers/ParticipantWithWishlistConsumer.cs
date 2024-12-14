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
        logger.LogInformation("HRManager received participant+wishlist: Id={Id}, Title={Title}, HackathonId={HackathonId}", 
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
        
        logger.LogInformation("ParticipantWithWishlist saved: participantId={Id}, wishlistCount={Count}", 
                               msg.ParticipantId, msg.DesiredParticipants.Length);
        
        orchestration.OnDataReceived(msg.HackathonId); 
    }
}
