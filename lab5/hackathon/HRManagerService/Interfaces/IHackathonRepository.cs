using System.Threading.Tasks;
using Entities;

namespace HRManagerService.Interfaces;

public interface IHackathonRepository
{
    Task<Hackathon> CreateHackathonAsync(Hackathon hackathon);
    Task<Hackathon> GetByIdAsync(int hackathonId);
    Task UpdateHackathonAsync(Hackathon hackathon);
}