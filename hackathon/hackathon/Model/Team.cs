// Model/Team.cs
using Hackathon.Model;

namespace Hackathon.Model
{
    public class Team
    {
        public Junior Junior { get; private set; }
        public TeamLead TeamLead { get; private set; }

        public Team(Junior junior, TeamLead teamLead)
        {
            Junior = junior;
            TeamLead = teamLead;

            Junior.AssignedPartner = TeamLead.Name;
            TeamLead.AssignedPartner = Junior.Name;
        }
    }
}