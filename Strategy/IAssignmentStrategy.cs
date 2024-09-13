// IAssignmentStrategy.cs
using Hackathon.Model;

namespace Hackathon.Strategy
{
    public interface IAssignmentStrategy
    {
        List<(Junior, TeamLead)> AssignPairs(List<Junior> juniors, List<TeamLead> teamLeads);
    }
}
