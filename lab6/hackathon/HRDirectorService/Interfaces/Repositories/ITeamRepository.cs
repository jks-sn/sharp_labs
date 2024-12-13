using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;

namespace HRDirectorService.Interfaces;

public interface ITeamRepository
{
    Task AddTeamsAsync(IEnumerable<Team> teams);

    Task<List<Team>> GetAllAsync();
    
    Task<List<Team>> GetTeamsForHackathonAsync(int hackathonId);
}