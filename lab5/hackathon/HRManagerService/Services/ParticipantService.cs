//HRManagerService/Services/ParticipantService.cs

using System.Threading.Tasks;
using Entities;
using Entities.Consts;
using Microsoft.Extensions.Logging;
using System.Linq;
using HRManagerService.Interfaces;

namespace HRManagerService.Services;

public class ParticipantService(HRManagerService hrManagerService, ILogger<ParticipantService> logger)
    : IParticipantService
{
    public async Task AddParticipantAsync(ParticipantInputModel input)
    {
        var title = ParticipantTitleExtensions.FromString(input.Title);
        var participant = new Participant(input.Id, title, input.Name);
        logger.LogInformation("Adding participant: Id={Id}, Title={Title}, Name={Name}", input.Id, input.Title, input.Name);
        await hrManagerService.AddParticipantAsync(participant);
    }

    public async Task AddWishlistAsync(WishlistInputModel input)
    {
        var wishlist = new Wishlist
        {
            ParticipantId = input.ParticipantId,
            ParticipantTitle = ParticipantTitleExtensions.FromString(input.ParticipantTitle),
            DesiredParticipants = input.DesiredParticipants
        };

        logger.LogInformation("Adding wishlist for participant {ParticipantId}, {ParticipantTitle}, with {Count} desired participants",
            input.ParticipantId, input.ParticipantTitle.ToString(), input.DesiredParticipants.Count);

        await hrManagerService.AddWishlistAsync(wishlist);
    }
}