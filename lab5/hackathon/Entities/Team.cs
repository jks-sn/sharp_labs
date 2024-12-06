//Entities/Team.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public class Team
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int HackathonId { get; set; }
    
    [ForeignKey("HackathonId")]
    public Hackathon Hackathon { get; set; }
    
    [Required]
    public int TeamLeadId { get; set; }
    
    public Participant TeamLead { get; set; }
    
    [Required]
    public int JuniorId { get; set; }
    public Participant Junior { get; set; }
    
    public Team() {}
    
    public Team(Participant teamLead, Participant junior)
    {
        TeamLead = teamLead;
        Junior = junior;
        TeamLeadId = teamLead.Id;
        JuniorId = junior.Id;
    }
}