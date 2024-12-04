using Entities;

namespace Repositories.Interface;

public interface IHackathonRepository
{
    Hackathon GetHackathonById(int id);
    IEnumerable<Hackathon> GetAllHackathons();
    void AddHackathon(Hackathon hackathon);
    void UpdateHackathon(Hackathon hackathon);
}