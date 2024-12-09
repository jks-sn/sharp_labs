//Entities/Wishlist.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Consts;

namespace Entities;

public class Wishlist
{
    public int Id { get; set; }
    
    public int ParticipantId { get; set; }
    
    public ParticipantTitle ParticipantTitle { get; set; }
    public Participant Participant { get; set; }
    public List<int> DesiredParticipants { get; set; } = new List<int>();

    public int? HackathonId { get; set; }
    public Hackathon Hackathon { get; set; }
    public Wishlist() {}
    
    public Wishlist( int participantId, ParticipantTitle participantTitle, List<int> desiredParticipants)
    {
        ParticipantId = participantId;
        ParticipantTitle = participantTitle;
        DesiredParticipants = desiredParticipants;
    }
}