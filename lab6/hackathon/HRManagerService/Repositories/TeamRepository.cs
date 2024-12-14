//HRManagerService/Repositories/TeamRepository.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRManagerService.Data;
using HRManagerService.Entities;
using HRManagerService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRManagerService.Repositories;

public class TeamRepository(HRManagerDbContext context) : ITeamRepository
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
}