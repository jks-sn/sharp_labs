// HRManagerService/Strategies/RandomStrategy.cs

using HRManagerService.Entities;
using HRManagerService.Interface;
using Microsoft.Extensions.Logging;

namespace HRManagerService.Strategies;

public class RandomStrategy(ILogger<RandomStrategy> logger) : ITeamBuildingStrategy
{
    public IEnumerable<Team> BuildTeams(
        IEnumerable<Participant> teamLeads,
        IEnumerable<Participant> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists)
    {
        logger.LogWarning("Запущен RandomStrategy: полностью случайное распределение без учёта пожеланий.");

        var rnd = new Random();

        var leads = teamLeads.ToList();
        var jrs = juniors.ToList();
        var teams = new List<Team>();

        // Перемешаем списки
        leads = leads.OrderBy(_ => rnd.Next()).ToList();
        jrs = jrs.OrderBy(_ => rnd.Next()).ToList();
        
        int count = Math.Min(leads.Count, jrs.Count);

        for (int i = 0; i < count; i++)
        {
            var teamLead = leads[i];
            var junior = jrs[i];
            var team = new Team(teamLead, junior);
            teams.Add(team);
            logger.LogDebug($"TeamLead {teamLead.Name} (ID={teamLead.Id}) + Junior {junior.Name} (ID={junior.Id}).");
        }

        logger.LogWarning($"Всего случайно сформировано {teams.Count} команд.");
        return teams;
    }
}