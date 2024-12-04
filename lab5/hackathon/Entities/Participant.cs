using System.ComponentModel.DataAnnotations;
using Entities.Consts;

namespace Entities;

public class Participant
{
    [Key]
    public int Id { get; set; }
    public ParticipantTitle Title { get; set; }
    [Required]
    public string Name { get; set; }
    
    // Навигационные свойства
    public List<Team> Teams { get; set; } 
    public List<Wishlist> Wishlists { get; set; }
    public List<Hackathon> Hackathons { get; set; }

    public Participant() {}

    public Participant(int id, ParticipantTitle title, string name)
    {
        Id = id;
        Title = title;
        Name = name;
    }
    public Wishlist MakeWishlist(IEnumerable<Participant> participants)
    {
        var desiredParticipants = participants
            .Select(e => e.Id)
            .OrderBy(_ => Random.Shared.Next()).ToList();

        return new Wishlist(Id, Title, desiredParticipants);
    }
}