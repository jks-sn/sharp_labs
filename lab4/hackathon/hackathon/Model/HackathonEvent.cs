// Model/HackathonEvent.cs

using System.Collections.Generic;

namespace Hackathon.Model;
    
public class HackathonEvent
{
    public int Id { get; set; }
    
    public double Harmonic { get; set; }
    
    public ICollection<Participant> Participants { get; set; }

    public ICollection<Team> Teams { get; set; } = new List<Team>();
    
}