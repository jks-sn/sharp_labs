using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using Participant = HRDirectorService.Entities.Participant;
using ParticipantTitle = HRDirectorService.Entities.Consts.ParticipantTitle;

namespace HRDirectorService.Interfaces;

public interface IParticipantRepository
{
    Task AddParticipantAsync(Participant participant);
    
    Task<int> CountAsync();

    Task<int> GetParticipantCountForHackathonAsync(int hackathonId);
    
    Task<List<Participant>> GetAllAsync();

    Task<List<Participant>> GetParticipantsForHackathonAsync(int hackathonId);
    
    Task<Participant> GetByIdAsync(int id, ParticipantTitle title);
}