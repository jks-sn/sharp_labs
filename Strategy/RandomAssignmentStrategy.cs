// RandomAssignmentStrategy.cs

using Hackathon.Model;

namespace Hackathon.Strategy
{
    public class RandomAssignmentStrategy : IAssignmentStrategy
    {
        public List<Team> AssignPairs(List<Junior> juniors, List<TeamLead> teamLeads)
        {
            Random rnd = new Random();
            var shuffledTeamLeads = teamLeads.OrderBy(x => rnd.Next()).ToList();

            var teams = new List<Team>();

            for (int i = 0; i < juniors.Count; i++)
            {
                //Console.WriteLine($"Количество предпочтений для джуна {juniors[i].Name}: {juniors[i].WishList.Count}");
                //Console.WriteLine($"Количество предпочтений для тимлида {shuffledTeamLeads[i].Name}: {shuffledTeamLeads[i].WishList.Count}");
                teams.Add(new Team(juniors[i], shuffledTeamLeads[i]));
            }

            return teams;
        }
    }
}
