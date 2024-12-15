//HRDirectorService/Services/HRDirectorOrchestrationService.cs

using System.Threading.Tasks;
using Dto;
using HRDirectorService.Repositories;
using Microsoft.Extensions.Logging;
using Entities;
using System.Linq;
using HRDirectorService.Entities.Consts;
using HRDirectorService.Interfaces;
using Microsoft.AspNetCore.Components.Web;
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
        var hackathon = await hackathonRepo.GetIdByHackathonIdAsync(payload.HackathonId);
        
        var participants = await participantRepo.GetParticipantsForHackathonAsync(hackathon.Id);
        var participantDict = participants.ToDictionary(
            p => (p.ParticipantId, p.Title, p.HackathonId),
            p => p.Id);

        var teams = new List<Team>();
        foreach (var teamDto in payload.Teams)
        {
            var teamleadId = teamDto.TeamLead.Id;
            int juniorId = teamDto.Junior.Id;
            
            bool teamleadFound = participantDict.TryGetValue((teamleadId, ParticipantTitle.TeamLead, hackathon.Id), out int internalTeamLeadId);
            bool juniorFound = participantDict.TryGetValue((juniorId, ParticipantTitle.Junior, hackathon.Id), out int internalJuniorId);

            if (!teamleadFound || !juniorFound)
            {
                logger.LogWarning("Cannot find internal IDs for TeamLeadId={ExternalTeamLeadId} or JuniorId={ExternalJuniorId} in HackathonExternalId={ExternalHackathonId}", 
                    teamleadId, juniorId, hackathon.Id);
                continue;
            }
            
            var team = new Team
            {
                HackathonId = hackathon.Id, 
                TeamLeadId = internalTeamLeadId, 
                JuniorId = internalJuniorId,
            };

            teams.Add(team);
        }

        await teamRepo.AddTeamsAsync(teams);
        logger.LogInformation("Added {Count} teams to HackathonId={HackathonId}", teams.Count, hackathon.Id);
        
        
        var meanSatisfaction = await CalculateMeanSatisfactionAsync(hackathon.Id);
        hackathon.MeanSatisfactionIndex = meanSatisfaction;

        await hackathonRepo.UpdateHackathonAsync(hackathon);

        logger.LogInformation("Hackathon {HackathonId} MeanSatisfaction={Mean}", hackathon.Id, meanSatisfaction);
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
            var teamLead = participants.FirstOrDefault(p => p.Id == team.TeamLeadId);
            var junior = participants.FirstOrDefault(p => p.Id == team.JuniorId);

            if (teamLead == null || junior == null)
            {
                logger.LogWarning("TeamLead or Junior not found for TeamId={TeamId}", team.Id);
                continue;
            }

            var teamLeadWishlist = wishlists.FirstOrDefault(w => w.ParticipantId == teamLead.Id);
            var juniorWishlist = wishlists.FirstOrDefault(w => w.ParticipantId == junior.Id);

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
