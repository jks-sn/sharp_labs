// HRManagerService/Interfaces/IParticipantService.cs

using System.Threading.Tasks;
using Dto;

namespace HRManagerService.Interfaces;

public interface IParticipantService
{
    Task AddParticipantAsync(ParticipantDto ParticipantDto);
    Task AddWishlistAsync(WishlistDto WishlistDto);
}