//Entities/Team.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ParticipantTitle = HRDirectorService.Entities.Consts.ParticipantTitle;

namespace Entities;

public class Team
{
    public int Id { get; set; }
    public int HackathonId { get; set; }
    public HRDirectorService.Entities.Hackathon Hackathon { get; set; }
    
    
    public int TeamLeadId { get; set; }
    public ParticipantTitle TeamLeadTitle { get; set; }
    public HRDirectorService.Entities.Participant TeamLead { get; set; }
    
    public int JuniorId { get; set; }
    public ParticipantTitle JuniorTitle { get; set; }
    public HRDirectorService.Entities.Participant Junior { get; set; }
    
    public Team() {}
    
    public Team(HRDirectorService.Entities.Participant teamLead, HRDirectorService.Entities.Participant junior)
    {
        TeamLead = teamLead;
        Junior = junior;
        TeamLeadId = teamLead.Id;
        JuniorId = junior.Id;
    }
}