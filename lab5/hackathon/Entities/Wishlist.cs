using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Consts;

namespace Entities;

public class Wishlist
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("Participant")]
    public int ParticipantId { get; set; }
    // Навигационные свойства
    public Participant? Participant { get; set; }
    [ForeignKey("Hackathon")]
    public int? HackathonId { get; set; }
    public Hackathon? Hackathon { get; set; }
    public ParticipantTitle ParticipantTitle { get; set; }
    public List<int> DesiredParticipants { get; set; }
    public Wishlist() {}

    public Wishlist(int participantId, ParticipantTitle title, List<int>desiredParticipants)
    {
        ParticipantId = participantId;
        ParticipantTitle = title;
        DesiredParticipants = desiredParticipants;
    }
}