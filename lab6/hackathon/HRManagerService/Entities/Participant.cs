//Entities/Participant.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRManagerService.Entities.Consts;

namespace HRManagerService.Entities;

public class Participant
{
    public int Id { get; set; }
    public int ParticipantId { get; set; }
    public ParticipantTitle Title { get; set; }
    public string Name { get; set; }
    
    public int HackathonId { get; set; }
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

    public Wishlist MakeWishlist(IEnumerable<Participant> probableTeammates)
    {
        var desiredParticipants = probableTeammates
            .Select(e => e.ParticipantId)
            .OrderBy(_ => Guid.NewGuid())
            .ToArray();
        return new Wishlist
        {
            ParticipantId = ParticipantId,
            DesiredParticipants = desiredParticipants
        };
    }
}