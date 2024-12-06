// Entities/GaleShapleyStrategy.cs

using HRManagerService.Interface;

namespace Entities;
public class GaleShapleyStrategy : ITeamBuildingStrategy
{
    public IEnumerable<Team> BuildTeams(
        IEnumerable<Participant> teamLeads,
        IEnumerable<Participant> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists)
    {
        var teamLeadsList = teamLeads.ToList();
        var juniorsList = juniors.ToList();

        // Создаем словарь предпочтений тимлидов
        var teamLeadPreferences = teamLeadsWishlists.ToDictionary(
            w => w.ParticipantId,
            w => w.DesiredParticipants.ToList());

        // Создаем словарь предпочтений джунов
        var juniorPreferences = juniorsWishlists.ToDictionary(
            w => w.ParticipantId,
            w => w.DesiredParticipants.ToList());

        // Инициализируем словарь предложений тимлидов
        var teamLeadProposals = teamLeadsList.ToDictionary(
            tl => tl.Id,
            tl => new Queue<int>(teamLeadPreferences.ContainsKey(tl.Id) ? teamLeadPreferences[tl.Id] : new List<int>()));

        // Словарь пар: джун -> тимлид
        var engagements = new Dictionary<int, int>();

        // Пока есть свободные тимлиды с непросмотренными предпочтениями
        while (teamLeadProposals.Any(kvp => kvp.Value.Count > 0))
        {
            foreach (var teamLead in teamLeadsList)
            {
                // Если тимлид уже в паре или у него закончились предпочтения, пропускаем
                if (engagements.ContainsValue(teamLead.Id) || !teamLeadProposals[teamLead.Id].Any())
                    continue;

                // Тимлид делает предложение джуну
                var juniorId = teamLeadProposals[teamLead.Id].Dequeue();

                // Если джун свободен
                if (!engagements.ContainsKey(juniorId))
                {
                    engagements[juniorId] = teamLead.Id;
                }
                else
                {
                    var currentTeamLeadId = engagements[juniorId];

                    // Сравниваем предпочтения джуна между текущим тимлидом и новым предложением
                    var juniorPreferenceList = juniorPreferences.ContainsKey(juniorId) ? juniorPreferences[juniorId] : new List<int>();
                    if (juniorPreferenceList.IndexOf(teamLead.Id) < juniorPreferenceList.IndexOf(currentTeamLeadId))
                    {
                        // Джун предпочитает нового тимлида
                        engagements[juniorId] = teamLead.Id;
                    }
                    // Если джун предпочитает текущего тимлида, ничего не делаем
                }
            }
        }

        // Формируем команды
        var teams = engagements.Select(e =>
        {
            var junior = juniorsList.FirstOrDefault(j => j.Id == e.Key);
            var teamLead = teamLeadsList.FirstOrDefault(tl => tl.Id == e.Value);
            return new Team(teamLead, junior);
        })
        .Where(t => t.TeamLead != null && t.Junior != null)
        .ToList();

        return teams;
    }
}
