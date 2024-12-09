//HRManagerService/Repositories/HackathonRepository.cs

using System.Threading.Tasks;
using Entities;
using HRManagerService.Data;
using HRManagerService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRManagerService.Repositories;

public class HackathonRepository(HRManagerDbContext context) : IHackathonRepository
{
    public async Task<Hackathon> CreateHackathonAsync(Hackathon hackathon)
    {
        context.Hackathons.Add(hackathon);
        await context.SaveChangesAsync();
        return hackathon;
    }

    public async Task<Hackathon> GetByIdAsync(int id)
    {
        return await context.Hackathons
            .Include(h => h.Participants)
            .Include(h => h.Wishlists)
            .Include(h => h.Teams)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task UpdateHackathonAsync(Hackathon hackathon)
    {
        context.Hackathons.Update(hackathon);
        await context.SaveChangesAsync();
    }
}