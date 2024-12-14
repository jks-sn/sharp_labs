using System.Threading.Tasks;
using Entities;
using Hackathon = HRDirectorService.Entities.Hackathon;

namespace HRDirectorService.Interfaces;

public interface IHackathonRepository
{
    Task<Hackathon> CreateHackathonAsync(Hackathon hackathon);
    Task<Hackathon> GetByIdAsync(int hackathonId);
    Task UpdateHackathonAsync(Hackathon hackathon);
    Task<Hackathon> GetIdByHackathonIdAsync(int hackathonId);
}