//Entities/Hackathon.cs

using Entities;

namespace HRDirectorService.Entities;

public class Hackathon
{
    public int Id { get; set; }
    public double MeanSatisfactionIndex { get; set; }
    
    public ICollection<Participant> Participants { get; set; } = new List<Participant>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public ICollection<Team> Teams { get; set; } = new List<Team>();

    public Hackathon() {}
}