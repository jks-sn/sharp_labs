using System;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using HRManagerService.Data;
using HRManagerService.Interface;
using HRManagerService.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRManagerService.Services
{
    public class HackathonService(
        HRManagerDbContext dbContext,
        ITeamBuildingStrategy teamBuildingStrategy,
        IHRDirectorClient hrDirectorClient,
        ILogger<HackathonService> logger)
        : IHackathonService
    {
        public async Task CreateHackathonAsync()
        {
            // Проверяем, есть ли достаточно участников
            var totalParticipants = await dbContext.Participants.CountAsync();
            if (totalParticipants < 10) // Например, минимальное количество участников
            {
                logger.LogInformation("Недостаточно участников для создания хакатона.");
                return;
            }

            // Создаём новый хакатон
            var hackathon = new Hackathon();
            dbContext.Hackathons.Add(hackathon);
            await dbContext.SaveChangesAsync();

            // Получаем участников и их Wishlist
            var participants = await dbContext.Participants.ToListAsync();
            var wishlists = await dbContext.Wishlists.ToListAsync();

            // Связываем участников и Wishlist с хакатоном
            foreach (var participant in participants)
            {
                participant.HackathonId = hackathon.Id;
            }

            foreach (var wishlist in wishlists)
            {
                wishlist.HackathonId = hackathon.Id;
            }

            await dbContext.SaveChangesAsync();

            // Разбиваем участников по ролям
            var teamLeads = participants.Where(p => p.Title == Entities.Consts.ParticipantTitle.TeamLead);
            var juniors = participants.Where(p => p.Title == Entities.Consts.ParticipantTitle.Junior);

            // Получаем Wishlist по ролям
            var teamLeadWishlists = wishlists.Where(w => w.ParticipantTitle == Entities.Consts.ParticipantTitle.TeamLead);
            var juniorWishlists = wishlists.Where(w => w.ParticipantTitle == Entities.Consts.ParticipantTitle.Junior);

            // Генерируем команды
            var teams = teamBuildingStrategy.BuildTeams(teamLeads, juniors, teamLeadWishlists, juniorWishlists).ToList();

            // Сохраняем команды в базу данных
            foreach (var team in teams)
            {
                team.HackathonId = hackathon.Id;
                dbContext.Teams.Add(team);
            }

            await dbContext.SaveChangesAsync();

            // Отправляем данные в HRDirectorService
            await hrDirectorClient.SendHackathonDataAsync(hackathon.Id);

            logger.LogInformation("Хакатон {HackathonId} создан и данные отправлены в HRDirectorService.", hackathon.Id);
        }
    }
}
