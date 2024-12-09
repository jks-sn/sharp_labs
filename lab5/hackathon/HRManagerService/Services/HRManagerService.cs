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

    private readonly TaskCompletionSource<bool> _participantsTcs = new();
    private readonly TaskCompletionSource<bool> _wishlistsTcs = new();

    public async Task AddParticipantAsync(Participant participant)
    {
        await participantRepo.AddParticipantAsync(participant);
        var count = await participantRepo.CountAsync();
        logger.LogInformation("AAAAAAAAAAAAAAAAAAAAParticipant added. Total: {Count}/{Expected}", count, _expectedParticipantCount);

        if (count >= _expectedParticipantCount)
        {
            logger.LogInformation("СТАРТУУУУУУУУУУУУУУУУУУУУУУУУУУУЕМ");
            _participantsTcs.TrySetResult(true);
        }
    }

    public async Task AddWishlistAsync(Wishlist wishlist)
    {
        await wishlistRepo.AddWishlistAsync(wishlist);
        var count = await wishlistRepo.CountAsync();
        logger.LogInformation("AAAAAAAAAAAAAAAAAAAAAAAWishlist added. Total: {Count}/{Expected}", count, _expectedParticipantCount);

        if (count >= _expectedParticipantCount)
        {
            _wishlistsTcs.TrySetResult(true);
        }
    }

    public async Task WaitAllParticipantsReceivedAsync(CancellationToken cancellationToken)
    {
        await using (cancellationToken.Register(() => _participantsTcs.TrySetCanceled()))
        {
            await _participantsTcs.Task;
        }
    }

    public async Task WaitAllWishlistsReceivedAsync(CancellationToken cancellationToken)
    {
        await using (cancellationToken.Register(() => _wishlistsTcs.TrySetCanceled()))
        {
            await _wishlistsTcs.Task;
        }
    }

    public async Task<Participant[]> GetParticipantsAsync()
    {
        return (await participantRepo.GetAllAsync()).ToArray();
    }

    public async Task<Wishlist[]> GetWishlistsAsync()
    {
        return (await wishlistRepo.GetAllAsync()).ToArray();
    }
}
