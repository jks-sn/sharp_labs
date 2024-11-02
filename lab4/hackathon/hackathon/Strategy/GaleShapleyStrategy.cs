// Strategy/GaleShapleyStrategy.cs

using Hackathon.Model;

namespace Hackathon.Strategy;

public class GaleShapleyStrategy : IAssignmentStrategy
{
    public List<Team> AssignPairs(List<Junior> juniors, List<TeamLead> teamLeads)
    {
        var freeJuniors = new Queue<Junior>(juniors);
        var teamLeadEngagements = new Dictionary<string, string>();

        var teamLeadDict = teamLeads.ToDictionary(tl => tl.Name, tl => tl);
        var juniorDict = juniors.ToDictionary(j => j.Name, j => j);

        var proposalIndex = juniors.ToDictionary(j => j.Name, j => 0);

        // Создаем словари предпочтений
        var juniorPreferences = juniors.ToDictionary(
            j => j.Name,
            j => j.Preferences.OrderBy(p => p.Rank).Select(p => p.PreferredName).ToList()
        );

        var teamLeadPreferences = teamLeads.ToDictionary(
            tl => tl.Name,
            tl => tl.Preferences.OrderBy(p => p.Rank).Select(p => p.PreferredName).ToList()
        );

        while (freeJuniors.Count > 0)
        {
            var junior = freeJuniors.Dequeue();
            var juniorPrefList = juniorPreferences[junior.Name];

            if (proposalIndex[junior.Name] >= juniorPrefList.Count)
            {
                continue;
            }

            var teamLeadName = juniorPrefList[proposalIndex[junior.Name]];
            proposalIndex[junior.Name]++;

            if (!teamLeadEngagements.ContainsKey(teamLeadName))
            {
                teamLeadEngagements[teamLeadName] = junior.Name;
            }
            else
            {
                var currentJuniorName = teamLeadEngagements[teamLeadName];
                var teamLeadPrefList = teamLeadPreferences[teamLeadName];

                var currentJuniorRank = teamLeadPrefList.IndexOf(currentJuniorName);
                var proposingJuniorRank = teamLeadPrefList.IndexOf(junior.Name);

                if (proposingJuniorRank < currentJuniorRank)
                {
                    freeJuniors.Enqueue(juniorDict[currentJuniorName]);
                    teamLeadEngagements[teamLeadName] = junior.Name;
                }
                else
                {
                    freeJuniors.Enqueue(junior);
                }
            }
        }

        var teams = new List<Team>();
        foreach (var engagement in teamLeadEngagements)
        {
            var teamLead = teamLeadDict[engagement.Key];
            var junior = juniorDict[engagement.Value];
            teams.Add(new Team(junior, teamLead));
        }

        return teams;
    }
}
