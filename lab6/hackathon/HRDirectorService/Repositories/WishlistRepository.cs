//HRDirectorService/Repositories/WishlistRepository.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using HRDirectorService.Data;
using HRDirectorService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRDirectorService.Repositories;

public class WishlistRepository(HRDirectorDbContext context) : IWishlistRepository
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
            .Where(w => w.Participant.HackathonId == hackathonId)
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
            .Where(w => w.Participant.HackathonId == hackathonId)
            .ToListAsync();
    }
}