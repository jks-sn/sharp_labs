//HRDirectorService/Services/HRDirectorOrchestrationService.cs

using System.Threading.Tasks;
using Dto;
using HRDirectorService.Repositories;
using Microsoft.Extensions.Logging;
using Entities;
using System.Linq;
using HRDirectorService.Interfaces;
using Hackathon = HRDirectorService.Entities.Hackathon;

namespace HRDirectorService.Services;

public class HRDirectorOrchestrationService(
    IParticipantRepository participantRepo,
    IWishlistRepository wishlistRepo,
    ITeamRepository teamRepo,
    IHackathonRepository hackathonRepo,
    ILogger<HRDirectorOrchestrationService> logger)
{
    public void OnDataReceived(int hackathonId)
    {
        // Может быть тут что-то будет
    }

    public async Task ProcessTeamsAsync(TeamsPayloadDto payload)
    {
        int hackathonId = payload.HackathonId;
        logger.LogInformation("Processing teams for hackathon {HackathonId}", hackathonId);
        
        var hackathon = await hackathonRepo.GetByIdAsync(hackathonId);
        if (hackathon == null)
        {
            hackathon = new Hackathon { Id = hackathonId };
            hackathon = await hackathonRepo.CreateHackathonAsync(hackathon);
        }
        
        var teams = payload.Teams.Select(tDto =>
        {
            var team = new Team
            {
                HackathonId = hackathonId,
                TeamLeadId = tDto.TeamLead.Id,
                TeamLeadTitle = Entities.Consts.ParticipantTitleExtensions.FromString(tDto.TeamLead.Title),
                JuniorId = tDto.Junior.Id,
                JuniorTitle = Entities.Consts.ParticipantTitleExtensions.FromString(tDto.Junior.Title)
            };
            return team;
        }).ToList();

        await teamRepo.AddTeamsAsync(teams);

        var meanSatisfaction = await CalculateMeanSatisfactionAsync(hackathonId);
        hackathon.MeanSatisfactionIndex = meanSatisfaction;

        await hackathonRepo.UpdateHackathonAsync(hackathon);

        logger.LogInformation("Hackathon {HackathonId} MeanSatisfaction={Mean}", hackathonId, meanSatisfaction);
    }

    private async Task<double> CalculateMeanSatisfactionAsync(int hackathonId)
    {
        // Логика из вашего HackathonService
        var participants = await participantRepo.GetParticipantsForHackathonAsync(hackathonId);
        var wishlists = await wishlistRepo.GetWishlistsForHackathonAsync(hackathonId);
        var teams = await teamRepo.GetTeamsForHackathonAsync(hackathonId);

        if (!teams.Any())
            return 0.0;

        double total = 0.0;
        int count = 0;

        foreach (var team in teams)
        {
            var teamLead = participants.FirstOrDefault(p => p.Id == team.TeamLeadId && p.Title == team.TeamLeadTitle);
            var junior = participants.FirstOrDefault(p => p.Id == team.JuniorId && p.Title == team.JuniorTitle);

            if (teamLead == null || junior == null)
                continue;

            var teamLeadWishlist = wishlists.FirstOrDefault(w => w.ParticipantId == teamLead.Id && w.ParticipantTitle == teamLead.Title);
            var juniorWishlist = wishlists.FirstOrDefault(w => w.ParticipantId == junior.Id && w.ParticipantTitle == junior.Title);

            if (teamLeadWishlist != null && juniorWishlist != null)
            {
                var leadIndex = Array.IndexOf(teamLeadWishlist.DesiredParticipants, junior.Id);   
                var juniorIndex = Array.IndexOf(juniorWishlist.DesiredParticipants, teamLead.Id);

                if (leadIndex >= 0)
                {
                    total += (5 - leadIndex);
                    count++;
                }
                if (juniorIndex >= 0)
                {
                    total += (5 - juniorIndex);
                    count++;
                }
            }
        }

        return (count == 0) ? 0.0 : total / count;
    }
}
