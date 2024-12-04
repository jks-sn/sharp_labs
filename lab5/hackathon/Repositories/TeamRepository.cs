using Entities;
using Repositories.Interface;

namespace Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly AppDbContext _context;

    public TeamRepository(AppDbContext context)
    {
        _context = context;
    }

    public Team GetTeamById(int id)
    {
        return _context.Teams.Find(id);
    }

    public IEnumerable<Team> GetAllTeams()
    {
        return _context.Teams.ToList();
    }

    public void AddTeam(Team team)
    {
        _context.Teams.Add(team);
        _context.SaveChanges();
    }

    public void AddTeams(IEnumerable<Team> teams)
    {
        _context.Teams.AddRange(teams);
        _context.SaveChanges();
    }

    public void UpdateTeam(Team team)
    {
        _context.Teams.Update(team);
        _context.SaveChanges();
    }
}