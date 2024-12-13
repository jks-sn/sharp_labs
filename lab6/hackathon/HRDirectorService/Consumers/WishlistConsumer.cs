using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;
using Microsoft.Extensions.Logging;
using Messages;
using System.Threading.Tasks;
using Entities;
using HRDirectorService.Entities.Consts;
using HRDirectorService.Interfaces;
using HRDirectorService.Repositories;
using HRDirectorService.Services;
using Hackathon = HRDirectorService.Entities.Hackathon;
using ParticipantTitleExtensions = HRDirectorService.Entities.Consts.ParticipantTitleExtensions;

namespace HRDirectorService.Consumers;

[NotMapped]
public class WishlistConsumer(
    ILogger<WishlistConsumer> logger,
    IWishlistRepository wishlistRepo,
    IParticipantRepository participantRepo,
    IHackathonRepository hackathonRepo,
    HRDirectorOrchestrationService orchestration)
    : IConsumer<IWishlistInfo>
{
    public async Task Consume(ConsumeContext<IWishlistInfo> context)
    {
        var msg = context.Message;
        logger.LogInformation("HRDirector received wishlist from ParticipantId={ParticipantId}, HackathonId={HackathonId}",
            msg.ParticipantId, msg.HackathonId);

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
        
        var hackathon = await hackathonRepo.GetByIdAsync(msg.HackathonId);
        if (hackathon == null)
        {
            hackathon = new Hackathon { Id = msg.HackathonId };
            await hackathonRepo.CreateHackathonAsync(hackathon);
        }

        orchestration.OnDataReceived(msg.HackathonId);
    }
}