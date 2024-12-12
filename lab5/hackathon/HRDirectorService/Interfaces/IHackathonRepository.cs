//HRDirectorService/Interfaces/IHackathonRepository.cs

using System.Threading.Tasks;
using Entities;

namespace HRDirectorService.Interfaces;

public interface IHackathonRepository
{
    Task AddHackathonAsync(Hackathon hackathon);
}