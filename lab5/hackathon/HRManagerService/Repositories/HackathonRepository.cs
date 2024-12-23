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

    public async Task<Hackathon> GetByIdAsync(int hackathonId)
    {
        return await context.Hackathons
            .Include(h => h.Participants)
            .ThenInclude(p => p.Wishlists)
            .Include(h => h.Wishlists)
            .Include(h => h.Teams)
            .ThenInclude(t => t.TeamLead)
            .Include(h => h.Teams)
            .ThenInclude(t => t.Junior)
            .FirstOrDefaultAsync(h => h.Id == hackathonId);
    }

    public async Task UpdateHackathonAsync(Hackathon hackathon)
    {
        context.Hackathons.Update(hackathon);
        await context.SaveChangesAsync();
    }
}