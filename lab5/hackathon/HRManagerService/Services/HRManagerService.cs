//HRManagerService/Services/HRManagerService.cs

using System.Linq;
using System.Threading.Tasks;
using Entities;
using Entities.Consts;
using HRManagerService.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading;
using Microsoft.Extensions.Options;

namespace HRManagerService.Services;

public class HRManagerService(
    IParticipantRepository participantRepo,
    IWishlistRepository wishlistRepo,
    IOptions<Options.ControllerOptions> controllerOptions,
    ILogger<HRManagerService> logger)
{
    private readonly int _expectedParticipantCount = controllerOptions.Value.ParticipantsNumber;

    public int GetExpectedCount() => _expectedParticipantCount;
    
    public async Task<int> GetParticipantCountAsync() => await participantRepo.CountAsync();
    public async Task<int> GetWishlistCountAsync() => await wishlistRepo.CountAsync();

    public async Task<Participant[]> GetParticipantsAsync()
    {
        return (await participantRepo.GetAllAsync()).ToArray();
    }

    public async Task<Wishlist[]> GetWishlistsAsync()
    {
        return (await wishlistRepo.GetAllAsync()).ToArray();
    }
}
