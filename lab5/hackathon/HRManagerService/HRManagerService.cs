using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;

namespace HRManagerService
{
    public class HRManagerService
    {
        public bool AllWishlistsReceived => Wishlists.Count >= _expectedParticipantCount;
        public bool AllParticipantsReceived => Participants.Count >= _expectedParticipantCount;
        public List<Participant>? Participants { get; private set; } = new List<Participant>();
        public Dictionary<int, Wishlist> Wishlists { get; private set; } = new Dictionary<int, Wishlist>();
        public HRManager HrManager { get; }

        private readonly int _expectedParticipantCount;

        private readonly object _participanteLock = new object();
        private readonly object _wishlistLock = new object();

        public TaskCompletionSource<bool> ParticipantsReceivedTcs { get; } = new TaskCompletionSource<bool>();
        public TaskCompletionSource<bool> WishlistsReceivedTcs { get; } = new TaskCompletionSource<bool>();

        private readonly ILogger<HRManagerService> _logger;

        public HRManagerService(HRManager hrManager, int expectedParticipantCount, ILogger<HRManagerService> logger)
        {
            HrManager = hrManager;
            _expectedParticipantCount = expectedParticipantCount;
            _logger = logger;
        }

        public void AddParticipant(Participant participant)
        {
            lock (_participanteLock)
            {
                Participants.Add(participant);
                _logger.LogInformation($"Participant added: ID={participant.Id}, Name={participant.Name}, Title={participant.Title}");
                
                if (Participants.Count >= _expectedParticipantCount)
                {
                    _logger.LogInformation("All participants received.");
                    ParticipantsReceivedTcs.TrySetResult(true);
                }
            }
        }

        public void AddWishlist(Wishlist wishlist)
        {
            lock (_wishlistLock)
            {
                Wishlists[wishlist.ParticipantId] = wishlist;
                _logger.LogInformation($"Wishlist added for Participant ID {wishlist.ParticipantId}");
                
                if (Wishlists.Count >= _expectedParticipantCount)
                {
                    _logger.LogInformation("All wishlists received.");
                    WishlistsReceivedTcs.TrySetResult(true);
                }
            }
        }

        public Task WaitAllParticipantsReceivedAsync(CancellationToken cancellationToken)
        {
            return ParticipantsReceivedTcs.Task.WaitAsync(cancellationToken);
        }

        public Task WaitAllWishlistsReceivedAsync(CancellationToken cancellationToken)
        {
            return WishlistsReceivedTcs.Task.WaitAsync(cancellationToken);
        }
    }
}