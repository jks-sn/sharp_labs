//HRDirectorService/Repositories/WishlistRepository.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using HRManagerService.Data;
using HRManagerService.Entities;
using HRManagerService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRManagerService.Repositories;

public class WishlistRepository(HRManagerDbContext context) : IWishlistRepository
{
    public async Task AddWishlistAsync(Wishlist wishlist)
    {
        context.Wishlists.Add(wishlist);
        await context.SaveChangesAsync();
    }

    public async Task<int> CountAsync()
    {
        return await context.Wishlists.CountAsync();
    }
    
    public async Task<int> GetWishlistCountForHackathonAsync(int hackathonId)
    {
        return await context.Wishlists
            .Where(w => w.HackathonId == hackathonId)
            .CountAsync();
    }
    
    public async Task<List<Wishlist>> GetAllAsync()
    {
        return await context.Wishlists.ToListAsync();
    }
    
    public async Task<List<Wishlist>> GetWishlistsForHackathonAsync(int hackathonId)
    {
        return await context.Wishlists
            .Include(w => w.Participant)
            .Where(w => w.HackathonId == hackathonId)
            .ToListAsync();
    }
}