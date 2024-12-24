//HRManagerService/Services/ParticipantService.cs

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using Dto;
using HRManagerService.Entities;
using HRManagerService.Entities.Consts;
using HRManagerService.Interfaces;

namespace HRManagerService.Services;

public class ParticipantService(IParticipantRepository participantRepo, IWishlistRepository wishlistRepo, ILogger<ParticipantService> logger)
    : IParticipantService
{
    public async Task AddParticipantAsync(ParticipantDto input)
    {
        var title = ParticipantTitleExtensions.FromString(input.Title);
        var participant = new Participant(input.ParticipantId, title, input.Name);
        logger.LogWarning("Adding participant: HackathonId={HackathonId}, ParticipantTitle={ParticipantTitle}, ParticipantName={ParticipantName}", input.ParticipantId, input.Title, input.Name);
        await participantRepo.AddParticipantAsync(participant);
    }

    public async Task AddWishlistAsync(WishlistDto input)
    {
        var wishlist = new Wishlist
        {
            ParticipantId = input.ParticipantId,
            DesiredParticipants = input.DesiredParticipants
        };

        logger.LogWarning("Adding wishlist for participant {ParticipantId}, {ParticipantTitle}, with {Count} desired participants",
            input.ParticipantId, input.ParticipantTitle.ToString(), input.DesiredParticipants.Length);

        await wishlistRepo.AddWishlistAsync(wishlist);
    }
}