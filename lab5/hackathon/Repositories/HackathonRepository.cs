using Entities;
using Repositories.Interface;

namespace Repositories;

public class HackathonRepository : IHackathonRepository
{
    private readonly AppDbContext _context;

    public HackathonRepository(AppDbContext context)
    {
        _context = context;
    }

    public Hackathon GetHackathonById(int id)
    {
        return _context.Hackathons.Find(id);
    }

    public IEnumerable<Hackathon> GetAllHackathons()
    {
        return _context.Hackathons.ToList();
    }

    public void AddHackathon(Hackathon hackathon)
    {
        _context.Hackathons.Add(hackathon);
        _context.SaveChanges();
    }

    public void UpdateHackathon(Hackathon hackathon)
    {
        _context.Hackathons.Update(hackathon);
        _context.SaveChanges();
    }
}