using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;

namespace HRManagerService.Interfaces;

public interface IWishlistRepository
{
    Task AddWishlistAsync(Wishlist wishlist);
    Task<List<Wishlist>> GetAllAsync();
    Task<int> CountAsync();
}