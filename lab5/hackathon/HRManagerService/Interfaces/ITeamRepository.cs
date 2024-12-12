using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;

namespace HRManagerService.Interfaces;

public interface ITeamRepository
{
    Task AddTeamsAsync(IEnumerable<Team> teams);
    Task<List<Team>> GetAllAsync();
}