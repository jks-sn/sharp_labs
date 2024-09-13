// HRManager.cs
using Hackathon.Model;

using Hackathon.Strategy;

namespace Hackathon.Model
{
    public class HRManager(IAssignmentStrategy strategy)
    {
        private IAssignmentStrategy _strategy = strategy;

        public List<(Junior, TeamLead)> AssignTeams(List<Junior> juniors, List<TeamLead> teamLeads)
        {
            return _strategy.AssignPairs(juniors, teamLeads);
        }
    }
}
