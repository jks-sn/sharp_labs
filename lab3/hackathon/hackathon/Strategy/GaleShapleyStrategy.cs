// GaleShapleyStrategy.cs

using Hackathon.Model;

namespace Hackathon.Strategy;

public class GaleShapleyStrategy : IAssignmentStrategy
{
    public List<Team> AssignPairs(List<Junior> juniors, List<TeamLead> teamLeads)
    {
        var freeJuniors = new Queue<Junior>(juniors);
        var teamLeadEngagements = new Dictionary<string, string>(); // TeamLeadName -> JuniorName

        // словари для быстрого доступа
        var teamLeadDict = teamLeads.ToDictionary(tl => tl.Name, tl => tl);
        var juniorDict = juniors.ToDictionary(j => j.Name, j => j);

        // Индексы предложений для каждого джуна
        var proposalIndex = juniors.ToDictionary(j => j.Name, j => 0);

        while (freeJuniors.Count > 0)
        {
            var junior = freeJuniors.Dequeue();
            var teamLeadName = junior.WishList[proposalIndex[junior.Name]];
            proposalIndex[junior.Name]++;

            if (!teamLeadEngagements.ContainsKey(teamLeadName))
            {
                // Тимлид свободен
                teamLeadEngagements[teamLeadName] = junior.Name;
            }
            else
            {
                var currentJuniorName = teamLeadEngagements[teamLeadName];
                var teamLead = teamLeadDict[teamLeadName];

                // Сравнение предпочтений тимлида
                if (teamLead.WishList.IndexOf(junior.Name) < teamLead.WishList.IndexOf(currentJuniorName))
                {
                    // Тимлид предпочитает нового джуна
                    freeJuniors.Enqueue(juniorDict[currentJuniorName]);
                    teamLeadEngagements[teamLeadName] = junior.Name;
                }
                else
                {
                    // Тимлид оставляет текущего джуна
                    freeJuniors.Enqueue(junior);
                }
            }
        }

        // Формирование списка пар
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
