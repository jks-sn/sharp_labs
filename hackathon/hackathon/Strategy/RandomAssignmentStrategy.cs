// RandomAssignmentStrategy.cs

using Hackathon.Model;

namespace Hackathon.Strategy;
public class RandomAssignmentStrategy : IAssignmentStrategy
{
    public List<Team> AssignPairs(List<Junior> juniors, List<TeamLead> teamLeads)
    {
        Random rnd = new Random();
        var shuffledTeamLeads = teamLeads.OrderBy(x => rnd.Next()).ToList();

        var teams = new List<Team>();

        for (int i = 0; i < juniors.Count; i++)
        {
            teams.Add(new Team(juniors[i], shuffledTeamLeads[i]));
        }

        return teams;
    }
}
