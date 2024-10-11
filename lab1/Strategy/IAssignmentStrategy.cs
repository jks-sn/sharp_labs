// IAssignmentStrategy.cs
using Hackathon.Model;

namespace Hackathon.Strategy
{
    public interface IAssignmentStrategy
    {
        List<Team> AssignPairs(List<Junior> juniors, List<TeamLead> teamLeads);
    }
}
