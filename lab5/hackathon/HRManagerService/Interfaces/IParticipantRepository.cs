using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using Entities.Consts;

namespace HRManagerService.Interfaces;

public interface IParticipantRepository
{
    Task AddParticipantAsync(Participant participant);
    Task<List<Participant>> GetAllAsync();
    Task<Participant> GetByIdAsync(int id, ParticipantTitle title);
    Task<int> CountAsync();
}