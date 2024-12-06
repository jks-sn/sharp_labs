//Entities/Participant.cs

using System.ComponentModel.DataAnnotations;
using Entities.Consts;

namespace Entities;

public class Participant
{
    [Key] public int Id { get; set; }
    [Required] public ParticipantTitle Title { get; set; }
    [Required] public string Name { get; set; }

    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

    public int? HackathonId { get; set; }
    public Hackathon Hackathon { get; set; }

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
        var desiredParticipants = new List<int>(probableTeammates.Select(e => e.Id).OrderBy(_ => Random.Shared.Next()));
        return new Wishlist(participantId: Id, participantTitle: Title, desiredParticipants);
    }
}