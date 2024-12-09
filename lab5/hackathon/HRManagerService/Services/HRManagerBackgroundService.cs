//HRManagerService/Services/HRManagerBackgroundService.cs

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HRManagerService.Interface;
using HRManagerService.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HRManagerService.Services;

public class HRManagerBackgroundService(
    ILogger<HRManagerBackgroundService> logger,
    HRManagerService hrManagerService,
    ITeamBuildingStrategy strategy,
    IParticipantRepository participantRepo,
    IWishlistRepository wishlistRepo,
    ITeamRepository teamRepo,
    IHackathonRepository hackathonRepo,
    IHRDirectorClient hrDirectorClient)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("HRManagerBackgroundService запущен.");
        try
        {
            logger.LogInformation("Ожидание получения всех participants.");
            await hrManagerService.WaitAllParticipantsReceivedAsync(stoppingToken);

            logger.LogInformation("Ожидание получения всех wishlists.");
            await hrManagerService.WaitAllWishlistsReceivedAsync(stoppingToken);

            var participants = await participantRepo.GetAllAsync();
            var wishlists = await wishlistRepo.GetAllAsync();

            var teamLeads = participants.Where(p => p.Title == Entities.Consts.ParticipantTitle.TeamLead);
            var juniors = participants.Where(p => p.Title == Entities.Consts.ParticipantTitle.Junior);

            var teamLeadsWishlists = wishlists.Where(w => w.Participant.Title == Entities.Consts.ParticipantTitle.TeamLead);
            var juniorWishlists = wishlists.Where(w => w.Participant.Title == Entities.Consts.ParticipantTitle.Junior);


            logger.LogInformation("Создание команд.");
            var teams = strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorWishlists).ToList();
            logger.LogInformation("Команды созданы.");

            // Создаем хакатон и сохраняем в БД
            var hackathon = new Entities.Hackathon
            {
                MeanSatisfactionIndex = 0.0, // MeanSatisfactionIndex вычислит директор.
                Participants = participants.ToList(),
                Wishlists = wishlists.ToList(),
                Teams = teams
            };
            hackathon = await hackathonRepo.CreateHackathonAsync(hackathon);
            
            // Сохранение команд
            await teamRepo.AddTeamsAsync(teams);
            
            // Отправка данных о хакатоне в HRDirector
            await hrDirectorClient.SendHackathonDataAsync(hackathon.Id);

            logger.LogInformation("Данные о хакатоне отправлены в HRDirector.");
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("HRManagerBackgroundService остановлен по запросу отмены.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка в HRManagerBackgroundService");
        }
    }
}
