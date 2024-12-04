using Entities;

namespace Repositories.Interface;

public interface IParticipantRepository
{
    Participant GetParticipantById(int id);
    IEnumerable<Participant> GetAllParticipants();
    void AddParticipant(Participant participant);
    void AddParticipants(IEnumerable<Participant> participants);
    void UpdateParticipant(Participant participant);
}