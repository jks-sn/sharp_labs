// HRManagerService/Strategies/GaleShapleyStrategy.cs

using HRManagerService.Entities;
using HRManagerService.Interface;
using Microsoft.Extensions.Logging;

namespace HRManagerService.Strategies;

public class GaleShapleyStrategy(ILogger<GaleShapleyStrategy> logger) : ITeamBuildingStrategy
{
    public IEnumerable<Team> BuildTeams(
        IEnumerable<Participant> teamLeads,
        IEnumerable<Participant> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists)
    {
        logger.LogWarning("Начало процесса создания команд по алгоритму Gale-Shapley (исправленная версия).");

        // Превращаем в списки, чтобы обращаться по индексам
        var teamLeadsList = teamLeads.ToList();
        var juniorsList = juniors.ToList();

        // Словари предпочтений для TeamLead
        var teamLeadPreferences = new Dictionary<int, Queue<int>>();
        foreach (var w in teamLeadsWishlists)
        {
            // w.ParticipantId — ParticipantId тимлида, w.DesiredParticipants — список ParticipantId джунов
            teamLeadPreferences[w.ParticipantId] = new Queue<int>(w.DesiredParticipants);
        }
        // Для тимлидов, у которых нет wishlist, заводим пустую очередь
        foreach (var tl in teamLeadsList)
        {
            if (!teamLeadPreferences.ContainsKey(tl.ParticipantId))
            {
                teamLeadPreferences[tl.ParticipantId] = new Queue<int>();
            }
        }

        // Словари предпочтений для Junior
        var juniorPreferences = new Dictionary<int, List<int>>();
        foreach (var w in juniorsWishlists)
        {
            juniorPreferences[w.ParticipantId] = w.DesiredParticipants.ToList();
        }
        // Для джунов без wishlist — пустой список
        foreach (var j in juniorsList)
        {
            if (!juniorPreferences.ContainsKey(j.ParticipantId))
            {
                juniorPreferences[j.ParticipantId] = new List<int>();
            }
        }

        // Сопоставление: juniorId -> teamLeadId
        var engagements = new Dictionary<int, int>();

        // Множество "свободных" тимлидов, которые ещё не состоят в паре
        var freeTeamLeads = new HashSet<int>(teamLeadsList.Select(tl => tl.ParticipantId));

        // Пока существует свободный тимлид, у которого есть кандидаты
        while (freeTeamLeads.Any())
        {
            bool atLeastOneProposal = false;

            // Копируем коллекцию свободных тимлидов, чтобы итерироваться по ней
            var freeTeamLeadsSnapshot = freeTeamLeads.ToList();

            foreach (var teamLeadId in freeTeamLeadsSnapshot)
            {
                // Если у этого тимлида закончились джуны в списке — он остаётся "свободен", но предложить не может
                if (teamLeadPreferences[teamLeadId].Count == 0)
                {
                    logger.LogDebug($"Тимлид {teamLeadId} исчерпал все предложения и останется без пары.");
                    // Удаляем его из freeTeamLeads
                    freeTeamLeads.Remove(teamLeadId);
                    continue;
                }

                // Тимлид делает предложение первому в очереди джуну
                var juniorId = teamLeadPreferences[teamLeadId].Dequeue();
                atLeastOneProposal = true;
                logger.LogDebug($"Тимлид {teamLeadId} предлагает джуну {juniorId}.");

                // Проверяем, свободен ли джун
                if (!engagements.ContainsKey(juniorId))
                {
                    // Джун был свободен — соглашение
                    engagements[juniorId] = teamLeadId;
                    // Тимлид становится занят
                    freeTeamLeads.Remove(teamLeadId);
                    logger.LogDebug($"Джун {juniorId} был свободен, теперь образует пару с тимлидом {teamLeadId}.");
                }
                else
                {
                    var currentTeamLeadId = engagements[juniorId];
                    // Проверяем, предпочитает ли джун нового тимлида
                    if (JuniorPrefersNewOverCurrent(juniorPreferences[juniorId], teamLeadId, currentTeamLeadId))
                    {
                        // Джун переходит к новому тимлиду
                        engagements[juniorId] = teamLeadId;
                        // Новый тимлид занят
                        freeTeamLeads.Remove(teamLeadId);
                        // Старый тимлид становится свободным
                        freeTeamLeads.Add(currentTeamLeadId);
                        logger.LogDebug($"Джун {juniorId} переключился на тимлида {teamLeadId}. Тимлид {currentTeamLeadId} снова свободен.");
                    }
                    else
                    {
                        // Джун остаётся с текущим тимлидом, а teamLeadId остаётся свободным, но он уже вытянул этого джуна из очереди
                        logger.LogDebug($"Джун {juniorId} остался с текущим тимлидом {currentTeamLeadId}, тимлид {teamLeadId} продолжит искать других.");
                    }
                }
            }

            // Если ни одного предложения не было сделано в этом цикле — выходим
            if (!atLeastOneProposal)
            {
                logger.LogWarning("Ни одного предложения не было сделано, завершаем алгоритм досрочно.");
                break;
            }
        }

        // Формируем списки TeamLead—Junior
        var teams = engagements.Select(e =>
        {
            var junior = juniorsList.FirstOrDefault(j => j.ParticipantId == e.Key);
            var teamLead = teamLeadsList.FirstOrDefault(tl => tl.ParticipantId == e.Value);
            if (junior == null || teamLead == null) return null;
            return new Team(teamLead, junior);
        })
        .Where(t => t != null)
        .ToList();

        foreach (var team in teams)
        {
            Console.WriteLine($"Сделали Команды: HackathonId={team.HackathonId}, " +
                              $"TeamLeadId={team.TeamLead.ParticipantId}, JuniorId={team.Junior.ParticipantId}");
        }

        logger.LogWarning($"Всего создано {teams.Count} команд.");

        return teams;
    }

    private bool JuniorPrefersNewOverCurrent(List<int> juniorPreferenceList, int newTeamLeadId, int currentTeamLeadId)
    {
        // Если у джуна нет предпочтений вообще
        if (juniorPreferenceList == null || juniorPreferenceList.Count == 0)
        {
            logger.LogWarning("Джун не имеет предпочтений, оставляем статус-кво.");
            return false;
        }

        int newIndex = juniorPreferenceList.IndexOf(newTeamLeadId);
        int currentIndex = juniorPreferenceList.IndexOf(currentTeamLeadId);

        // Если оба не в списке - оставляем текущего
        if (newIndex == -1 && currentIndex == -1)
        {
            logger.LogWarning($"Джун не имеет в списке ни {newTeamLeadId}, ни {currentTeamLeadId}, остаётся со старым.");
            return false;
        }
        // Новый не в списке, текущий в списке
        if (newIndex == -1 && currentIndex >= 0)
        {
            logger.LogWarning($"Джун предпочитает текущего тимлида {currentTeamLeadId} (нового {newTeamLeadId} нет в списке).");
            return false;
        }
        // Новый в списке, а текущий нет
        if (newIndex >= 0 && currentIndex == -1)
        {
            logger.LogWarning($"Джун предпочитает нового тимлида {newTeamLeadId}, текущий {currentTeamLeadId} не в списке.");
            return true;
        }

        // Если оба в списке — сравниваем индекс (чем меньше индекс, тем выше приоритет)
        bool prefersNew = (newIndex < currentIndex);
        if (prefersNew)
            logger.LogWarning($"Джун предпочитает нового тимлида {newTeamLeadId} текущему {currentTeamLeadId}.");
        else
            logger.LogWarning($"Джун предпочитает текущего тимлида {currentTeamLeadId} новому {newTeamLeadId}.");

        return prefersNew;
    }
}
