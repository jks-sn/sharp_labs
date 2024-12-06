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
        await hrManagerService.AddParticipantAsync(participant);
    }

    public async Task AddWishlistAsync(WishlistInputModel input)
    {
        var title = ParticipantTitleExtensions.FromString(input.ParticipantTitle);
        var wishlist = new Wishlist(input.ParticipantId, title, input.DesiredParticipants);
        await hrManagerService.AddWishlistAsync(wishlist);
    }
}