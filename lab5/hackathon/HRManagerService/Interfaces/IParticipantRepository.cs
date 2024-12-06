using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;

namespace HRManagerService.Interfaces;

public interface IParticipantRepository
{
    Task AddParticipantAsync(Participant participant);
    Task<List<Participant>> GetAllAsync();
    Task<Participant> GetByIdAsync(int id);
    Task<int> CountAsync();
}