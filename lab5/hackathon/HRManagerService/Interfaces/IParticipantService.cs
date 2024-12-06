// HRManagerService/Interfaces/IParticipantService.cs

using System.Threading.Tasks;
using Entities;

namespace HRManagerService.Interfaces;

public interface IParticipantService
{
    Task AddParticipantAsync(ParticipantInputModel participantInputModel);
    Task AddWishlistAsync(WishlistInputModel wishlistInputModel);
}