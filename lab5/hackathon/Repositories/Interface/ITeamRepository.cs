using Entities;

namespace Repositories.Interface;

public interface ITeamRepository
{
    Team GetTeamById(int id);
    IEnumerable<Team> GetAllTeams();
    void AddTeam(Team team);
    void AddTeams(IEnumerable<Team> teams);
    void UpdateTeam(Team team);
}