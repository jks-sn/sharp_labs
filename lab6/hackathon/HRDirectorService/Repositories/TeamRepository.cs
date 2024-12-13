//HRDirectorService/Repositories/TeamRepository.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using HRDirectorService.Data;
using HRDirectorService.Data;
using HRDirectorService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRDirectorService.Repositories;

public class TeamRepository(HRDirectorDbContext context) : ITeamRepository
{
    public async Task AddTeamsAsync(IEnumerable<Team> teams)
    {
        context.Teams.AddRange(teams);
        await context.SaveChangesAsync();
    }

    public async Task<List<Team>> GetAllAsync()
    {
        return await context.Teams.ToListAsync();
    }
    
    public async Task<List<Team>> GetTeamsForHackathonAsync(int hackathonId)
    {
        return await context.Teams
            .Where(t => t.HackathonId == hackathonId)
            .ToListAsync();
    }
}