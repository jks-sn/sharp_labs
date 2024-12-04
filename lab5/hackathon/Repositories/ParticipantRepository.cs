using Entities;
using Repositories.Interface;

namespace Repositories;

public class ParticipantRepository(AppDbContext context) : IParticipantRepository
{
    private readonly AppDbContext _context = context;

    public Participant GetParticipantById(int participantId)
    {
        return _context.Participants.Find(participantId);
    }

    public IEnumerable<Participant> GetAllParticipants()
    {
        return _context.Participants.ToList();
    }

    public void AddParticipant(Participant participant)
    {
        _context.Participants.Add(participant);
        _context.SaveChanges();
    }

    public void AddParticipants(IEnumerable<Participant> participants)
    {
        _context.Participants.AddRange(participants);
        _context.SaveChanges();
    }

    public void UpdateParticipant(Participant participant)
    {
        _context.Participants.Update(participant);
        _context.SaveChanges();
    }
}