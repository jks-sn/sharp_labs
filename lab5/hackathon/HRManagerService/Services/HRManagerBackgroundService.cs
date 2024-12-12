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
    IServiceProvider serviceProvider)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogWarning("HRManagerBackgroundService запущен.");
        try
        {
            using var scope = serviceProvider.CreateScope();
            var hrManagerService = scope.ServiceProvider.GetRequiredService<HRManagerService>();
            var strategy = scope.ServiceProvider.GetRequiredService<ITeamBuildingStrategy>();
            var participantRepo = scope.ServiceProvider.GetRequiredService<IParticipantRepository>();
            var wishlistRepo = scope.ServiceProvider.GetRequiredService<IWishlistRepository>();
            var teamRepo = scope.ServiceProvider.GetRequiredService<ITeamRepository>();
            var hackathonRepo = scope.ServiceProvider.GetRequiredService<IHackathonRepository>();
            var hrDirectorClient = scope.ServiceProvider.GetRequiredService<IHRDirectorClient>();
            int requiredCount = hrManagerService.GetExpectedCount();
            logger.LogWarning("Ожидание получения всех participants и wishlists.");
            while (!stoppingToken.IsCancellationRequested)
            {
                var participantCount = await hrManagerService.GetParticipantCountAsync();
                var wishlistCount = await hrManagerService.GetWishlistCountAsync();
                if (participantCount >= requiredCount && wishlistCount >= requiredCount)
                {
                    logger.LogWarning("Достигнуто необходимое количество участников и вишлистов. Начинаем обработку...");
                    
                    var participants = await participantRepo.GetAllAsync();
                    var wishlists = await wishlistRepo.GetAllAsync();

                    var teamLeads = participants.Where(p => p.Title == Entities.Consts.ParticipantTitle.TeamLead);
                    var juniors = participants.Where(p => p.Title == Entities.Consts.ParticipantTitle.Junior);

                    var teamLeadsWishlists =
                        wishlists.Where(w => w.Participant.Title == Entities.Consts.ParticipantTitle.TeamLead);
                    var juniorWishlists =
                        wishlists.Where(w => w.Participant.Title == Entities.Consts.ParticipantTitle.Junior);


                    logger.LogWarning("Создание команд.");
                    var teams = strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorWishlists).ToList();
                    logger.LogWarning("Команды созданы.");
                    var hackathon = new Entities.Hackathon
                    {
                        MeanSatisfactionIndex = 0.0, // MeanSatisfactionIndex вычислит директор.
                        Participants = participants.ToList(),
                        Wishlists = wishlists.ToList(),
                        Teams = teams
                    };
                    hackathon = await hackathonRepo.CreateHackathonAsync(hackathon);
                    
                    await hrDirectorClient.SendHackathonDataAsync(hackathon.Id);

                    logger.LogWarning("Данные о хакатоне отправлены в HRDirector.");

                    break;
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("HRManagerBackgroundService остановлен по запросу отмены.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка в HRManagerBackgroundService");
        }
    }
}
