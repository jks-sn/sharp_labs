using Entities.Consts;

namespace Entities;

public class ParticipantGroup
{
    public ParticipantTitle Title { get; set; }
    public List<Participant> Participants { get; set; } = new List<Participant>();
    public List<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}