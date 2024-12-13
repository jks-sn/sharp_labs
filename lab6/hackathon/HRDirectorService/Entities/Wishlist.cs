//Entities/Wishlist.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ParticipantTitle = HRDirectorService.Entities.Consts.ParticipantTitle;

namespace Entities;

public class Wishlist
{
    public int Id { get; set; }
    
    public int ParticipantId { get; set; }
    
    public ParticipantTitle ParticipantTitle { get; set; }
    public HRDirectorService.Entities.Participant Participant { get; set; }
    public int[] DesiredParticipants { get; set; }
    public int? HackathonId { get; set; }
    public HRDirectorService.Entities.Hackathon Hackathon { get; set; }
    public Wishlist() {}
    
    public Wishlist( int participantId, ParticipantTitle participantTitle,int[] desiredParticipants)
    {
        ParticipantId = participantId;
        ParticipantTitle = participantTitle;
        DesiredParticipants = desiredParticipants;
    }
}