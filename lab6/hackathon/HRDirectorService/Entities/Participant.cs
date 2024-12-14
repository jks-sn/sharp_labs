//Entities/Participant.cs

using Entities;
using ParticipantTitle = HRDirectorService.Entities.Consts.ParticipantTitle;

namespace HRDirectorService.Entities;

public class Participant
{
    public int Id { get; set; }
    
    public int ParticipantId { get; set; }
    public ParticipantTitle Title { get; set; }
    public string Name { get; set; }
    
    public int HackathonId { get; set; }
    
   public Hackathon Hackathon { get; set; } 
    
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    
    public Participant()
    {
    }

    public Participant(int participantId, ParticipantTitle title, string name)
    {
        ParticipantId = participantId;
        Title = title;
        Name = name;
    }
}