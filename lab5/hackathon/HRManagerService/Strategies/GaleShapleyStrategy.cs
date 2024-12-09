// Entities/GaleShapleyStrategy.cs

using HRManagerService.Interface;

namespace Entities;
public class GaleShapleyStrategy(ILogger<GaleShapleyStrategy> logger) : ITeamBuildingStrategy
{

    public IEnumerable<Team> BuildTeams(
            IEnumerable<Participant> teamLeads,
            IEnumerable<Participant> juniors,
            IEnumerable<Wishlist> teamLeadsWishlists,
            IEnumerable<Wishlist> juniorsWishlists)
        {
            logger.LogWarning("Начало процесса создания команд по алгоритму Gale-Shapley.");
            var teamLeadsList = teamLeads.ToList();
            var juniorsList = juniors.ToList();
            
            if (!teamLeadsList.Any())
            {
                logger.LogWarning("Отсутствуют тимлиды. Невозможно создать команды.");
                return Enumerable.Empty<Team>();
            }
            
            if (!juniorsList.Any())
            {
                logger.LogWarning("Отсутствуют джуны. Невозможно создать команды.");
                return Enumerable.Empty<Team>();
            }
            
            logger.LogWarning($"Количество тимлидов: {teamLeadsList.Count}, количество джунов: {juniorsList.Count}.");
            
            var teamLeadPreferences = new Dictionary<int, List<int>>();
            foreach (var w in teamLeadsWishlists)
            {
                teamLeadPreferences[w.ParticipantId] = w.DesiredParticipants.ToList();
                logger.LogWarning($"Тимлид {w.ParticipantId} имеет следующие предпочтения: {string.Join(", ", w.DesiredParticipants)}.");
            }

            // Для тимлидов без явных предпочтений создаём пустые списки
            foreach (var tl in teamLeadsList)
            {
                if (!teamLeadPreferences.ContainsKey(tl.Id))
                {
                    teamLeadPreferences[tl.Id] = new List<int>();
                    logger.LogWarning($"Тимлид {tl.Id} не имеет явных предпочтений.");
                }
            }

            // Словари предпочтений джунов
            var juniorPreferences = new Dictionary<int, List<int>>();
            foreach (var w in juniorsWishlists)
            {
                juniorPreferences[w.ParticipantId] = w.DesiredParticipants.ToList();
                logger.LogWarning($"Джун {w.ParticipantId} имеет следующие предпочтения: {string.Join(", ", w.DesiredParticipants)}.");
            }

            // Для джунов без явных предпочтений создаём пустые списки
            foreach (var j in juniorsList)
            {
                if (!juniorPreferences.ContainsKey(j.Id))
                {
                    juniorPreferences[j.Id] = new List<int>();
                    logger.LogWarning($"Джун {j.Id} не имеет явных предпочтений.");
                }
            }

            // Инициализация очередей предложений тимлидов
            var teamLeadProposals = teamLeadsList.ToDictionary(
                tl => tl.Id,
                tl => new Queue<int>(teamLeadPreferences[tl.Id]));
            
            foreach (var tl in teamLeadProposals)
            {
                if (tl.Value.Any())
                {
                    logger.LogWarning($"Тимлид {tl.Key} готов предложить следующих джунов: {string.Join(", ", tl.Value)}.");
                }
                else
                {
                    logger.LogWarning($"Тимлид {tl.Key} не имеет джунов для предложения.");
                }
            }
            
            // engagements: juniorId -> teamLeadId
            var engagements = new Dictionary<int, int>();

            logger.LogWarning("Начало итераций предложений тимлидов джунам.");
            
            // Пока есть свободные тимлиды, у которых остались джуны для предложения
            // Цикл завершается, когда тимлидам некому предлагать
            while (teamLeadProposals.Any(kvp => kvp.Value.Count > 0 && !engagements.ContainsValue(kvp.Key)))
            {
                bool madeProposal = false;

                foreach (var teamLead in teamLeadsList)
                {
                    // Пропускаем, если уже состоят в паре
                    if (engagements.ContainsValue(teamLead.Id))
                    {
                        logger.LogWarning($"Тимлид {teamLead.Id} уже состоит в паре. Пропуск.");
                        continue;
                    }

                    var proposals = teamLeadProposals[teamLead.Id];
                    if (proposals.Count == 0)
                    {
                        logger.LogWarning($"Тимлид {teamLead.Id} не имеет оставшихся джунов для предложения.");
                        continue; // У этого тимлида не осталось кандидатов
                    }

                // Тимлид делает предложение джуну
                    var juniorId = proposals.Dequeue();
                    madeProposal = true;
                    logger.LogWarning($"Тимлид {teamLead.Id} предлагает джуну {juniorId}.");
                    
                    if (!engagements.ContainsKey(juniorId))
                    {
                        // Джун свободен - соглашается
                        engagements[juniorId] = teamLead.Id;
                        logger.LogWarning($"Джун {juniorId} свободен и соглашается на предложение от тимлида {teamLead.Id}.");
                    }
                    else
                    {
                        var currentTeamLeadId = engagements[juniorId];
                        logger.LogWarning($"Джун {juniorId} уже состоит в паре с тимлидом {currentTeamLeadId}.");
                        // Проверяем предпочитает ли джун нового тимлида
                        if (JuniorPrefersNewOverCurrent(juniorPreferences[juniorId], teamLead.Id, currentTeamLeadId))
                        {
                            // Джун переключается на нового тимлида
                            engagements[juniorId] = teamLead.Id;
                            logger.LogWarning($"Джун {juniorId} предпочитает тимлида {teamLead.Id} текущему тимлиду {currentTeamLeadId}. Переключение пар.");
                            // Предыдущий тимлид стал свободен, но он снова попробует в следующих итерациях
                        }
                        else
                        {
                            logger.LogWarning($"Джун {juniorId} предпочитает остаться с тимлидом {currentTeamLeadId}.");
                        }
                        // Иначе джун остаётся с текущим тимлидом, а новый будет пытаться еще
                    }
                }

                // Если ни одного предложения не было сделано за весь проход – выходим из цикла
                // Это предохраняет от потенциального зависания
                if (!madeProposal)
                {
                    logger.LogWarning("Ни одно предложение не было сделано за текущую итерацию. Завершение процесса создания команд.");
                    break;
                }
            }

            logger.LogWarning("Формирование конечных команд на основе engagements.");
            // Формируем команды
            var teams = engagements.Select(e =>
            {
                var junior = juniorsList.FirstOrDefault(j => j.Id == e.Key);
                var teamLead = teamLeadsList.FirstOrDefault(tl => tl.Id == e.Value);
                if (junior == null || teamLead == null)
                {
                    logger.LogWarning($"Невозможно создать команду для джуна {e.Key} и тимлида {e.Value}: один из участников отсутствует.");
                    return null;
                }
                var team = new Team(teamLead, junior);
                logger.LogWarning($"Создана команда: Id {team.Id}, Тимлид {team.TeamLeadId} - {teamLead.Name}, Джун {team.JuniorId} - {junior.Name}");
                return team;
            })
            .Where(t => t != null)
            .ToList();
            
            foreach (var team in teams)
            {
                Console.WriteLine($"Сделали Команды: Id={team.Id}, HackathonId={team.HackathonId}, " +
                                  $"TeamLeadId={team.TeamLeadId}, JuniorId={team.JuniorId}");
            }
            
            logger.LogWarning($"Всего создано {teams.Count} команд.");
            
            return teams;
        }

        private bool JuniorPrefersNewOverCurrent(List<int> juniorPreferenceList, int newTeamLeadId, int currentTeamLeadId)
        {
            // Если у джуна нет предпочтений (пустой список), оставим всё как есть
            if (juniorPreferenceList.Count == 0)
            {
                // Нет предпочтений – предпочитаем статус-кво
                logger.LogWarning($"Джун предпочитает статус-кво, так как нет предпочтений.");
                return false;
            }

            int newIndex = juniorPreferenceList.IndexOf(newTeamLeadId);
            int currentIndex = juniorPreferenceList.IndexOf(currentTeamLeadId);

            // Случаи, когда того или иного в списке нет:
            // - Оба не в списке: оставляем текущего
            if (newIndex < 0 && currentIndex < 0)
            {
                logger.LogWarning($"Джун не имеет предпочтений ни для тимлида {newTeamLeadId}, ни для тимлида {currentTeamLeadId}.");
                return false;
            }

            // - Новый не в списке, а текущий в списке: оставляем текущего
            if (newIndex < 0 && currentIndex >= 0)
            {
                logger.LogWarning($"Джун предпочитает текущего тимлида {currentTeamLeadId}, так как новый тимлид {newTeamLeadId} не в списке предпочтений.");
                return false;
            }

            // - Новый в списке, текущий нет: предпочитаем нового
            if (newIndex >= 0 && currentIndex < 0)
            {
                logger.LogWarning($"Джун предпочитает нового тимлида {newTeamLeadId}, так как текущий тимлид {currentTeamLeadId} не в списке предпочтений.");
                return true;
            }

            // Оба в списке – сравниваем индексы
            bool prefersNew = newIndex < currentIndex;
            if (prefersNew)
                logger.LogWarning($"Джун предпочитает нового тимлида {newTeamLeadId} текущему тимлиду {currentTeamLeadId}.");
            else
                logger.LogWarning($"Джун предпочитает текущего тимлида {currentTeamLeadId} новому тимлиду {newTeamLeadId}.");

            return prefersNew;
        }
}
