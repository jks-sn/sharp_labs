using System.Linq;
using System.Threading.Tasks;
using Entities;
using Entities.Consts;
using HRManagerService.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading;
using Microsoft.Extensions.Options;

namespace HRManagerService.Services;

public class HRManagerService
{
    private readonly IParticipantRepository _participantRepo;
    private readonly IWishlistRepository _wishlistRepo;
    private readonly ILogger<HRManagerService> _logger;
    private readonly int _expectedParticipantCount;

    private readonly TaskCompletionSource<bool> _participantsTcs = new();
    private readonly TaskCompletionSource<bool> _wishlistsTcs = new();

    public HRManagerService(
        IParticipantRepository participantRepo,
        IWishlistRepository wishlistRepo,
        IOptions<Options.ControllerOptions> controllerOptions,
        ILogger<HRManagerService> logger)
    {
        _participantRepo = participantRepo;
        _wishlistRepo = wishlistRepo;
        _logger = logger;
        _expectedParticipantCount = controllerOptions.Value.ParticipantsNumber;
    }

    public async Task AddParticipantAsync(Participant participant)
    {
        await _participantRepo.AddParticipantAsync(participant);
        var count = await _participantRepo.CountAsync();
        _logger.LogInformation("Participant added. Total: {Count}/{Expected}", count, _expectedParticipantCount);

        if (count >= _expectedParticipantCount)
        {
            _participantsTcs.TrySetResult(true);
        }
    }

    public async Task AddWishlistAsync(Wishlist wishlist)
    {
        await _wishlistRepo.AddWishlistAsync(wishlist);
        var count = await _wishlistRepo.CountAsync();
        _logger.LogInformation("Wishlist added. Total: {Count}/{Expected}", count, _expectedParticipantCount);

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
        return (await _participantRepo.GetAllAsync()).ToArray();
    }

    public async Task<Wishlist[]> GetWishlistsAsync()
    {
        return (await _wishlistRepo.GetAllAsync()).ToArray();
    }
}
