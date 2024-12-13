//Entities/Team.cs

using HRManagerService.Entities.Consts;

namespace HRManagerService.Entities;

public class Team
{
    public int Id { get; set; }
    public int HackathonId { get; set; }
    
    
    public int TeamLeadId { get; set; }
    public ParticipantTitle TeamLeadTitle { get; set; }
    public Participant TeamLead { get; set; }
    
    public int JuniorId { get; set; }
    public ParticipantTitle JuniorTitle { get; set; }
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