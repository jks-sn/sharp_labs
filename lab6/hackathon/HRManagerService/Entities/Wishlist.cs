//Entities/Wishlist.cs

using HRManagerService.Entities.Consts;

namespace HRManagerService.Entities;

public class Wishlist
{
    public int Id { get; set; }
    
    public int ParticipantId { get; set; }
    
    public ParticipantTitle ParticipantTitle { get; set; }
    public Participant Participant { get; set; }
    public int[] DesiredParticipants { get; set; }
    public int HackathonId { get; set; }
    public Wishlist() {}
    
    public Wishlist( int participantId, ParticipantTitle participantTitle,int[] desiredParticipants)
    {
        ParticipantId = participantId;
        ParticipantTitle = participantTitle;
        DesiredParticipants = desiredParticipants;
    }
}