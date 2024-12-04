using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public class Team
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Hackathon")]
    public int HackathonId { get; set; }
    public Hackathon Hackathon { get; set; }
    
    [NotMapped]
    public Participant TeamLead { get; set; }

    [NotMapped]
    public Participant Junior { get; set; }

    // Навигационные свойства
    public ICollection<Participant> Participants { get; set; }
    
    public Team() {}

    public Team(Participant teamLead, Participant junior)
    {
        TeamLead = teamLead;
        Junior = junior;
    }
}