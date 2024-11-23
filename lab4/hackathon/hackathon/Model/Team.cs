// Model/Team.cs

using Hackathon.Model;

namespace Hackathon.Model;
public class Team
{
    public int Id { get; set; }
    
    
    public int HackathonEventId { get; set; }
    public HackathonEvent HackathonEvent { get; set; }
    
    
    public int JuniorId { get; set; }
    public Junior Junior { get; set; }
    
    
    public int TeamLeadId { get; set; }
    public TeamLead TeamLead { get; set; }

    public Team(Junior junior, TeamLead teamLead)
    {
        Junior = junior;
        JuniorId = junior.Id;
        TeamLead = teamLead;
        TeamLeadId = teamLead.Id;

        Junior.AssignedPartner = TeamLead.Name;
        TeamLead.AssignedPartner = Junior.Name;
    }
    
    private Team() { }
}