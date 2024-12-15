using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;

namespace HRDirectorService.Interfaces;

public interface IWishlistRepository
{
    Task AddWishlistAsync(Wishlist wishlist);
    
    Task<int> CountAsync();

    Task<int> GetWishlistCountForHackathonAsync(int hackathonId);
    
    Task<List<Wishlist>> GetAllAsync();

    Task<List<Wishlist>> GetWishlistsForHackathonAsync(int hackathonId);
}