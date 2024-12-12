// HRManagerService/Interfaces/IParticipantService.cs

using System.Threading.Tasks;
using Dto;
using Entities;

namespace HRManagerService.Interfaces;

public interface IParticipantService
{
    Task AddParticipantAsync(ParticipantDto ParticipantDto);
    Task AddWishlistAsync(WishlistDto WishlistDto);
}