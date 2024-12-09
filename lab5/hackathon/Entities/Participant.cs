//Entities/Participant.cs

using System.ComponentModel.DataAnnotations;
using Entities.Consts;

namespace Entities;

public class Participant
{
    public int Id { get; set; }
    public ParticipantTitle Title { get; set; }
    public string Name { get; set; }

    
    public int? HackathonId { get; set; }
    public Hackathon Hackathon { get; set; }
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    
    public Participant()
    {
    }

    public Participant(int id, ParticipantTitle title, string name)
    {
        Id = id;
        Title = title;
        Name = name;
    }

    public Wishlist MakeWishlist(IEnumerable<Participant> probableTeammates)
    {
        var desiredParticipants = probableTeammates
            .Select(e => e.Id)
            .OrderBy(_ => Guid.NewGuid())
            .ToList();
        return new Wishlist
        {
            ParticipantId = Id,
            ParticipantTitle = Title,
            DesiredParticipants = desiredParticipants
        };
    }
}