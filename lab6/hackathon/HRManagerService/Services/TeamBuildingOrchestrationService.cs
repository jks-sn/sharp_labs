//HRManagerService/Services/TeamBuildingOrchestrationService.cs

using System.ComponentModel.DataAnnotations.Schema;
using HRManagerService.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq;
using Dto;
using HRManagerService.Clients;
using HRManagerService.Interface;

namespace HRManagerService.Services;

[NotMapped]
public class TeamBuildingOrchestrationService(
    IHRDirectorApi hrDirectorApi,
    IParticipantRepository participantRepo,
    IWishlistRepository wishlistRepo,
    ITeamRepository teamRepo,
    ITeamBuildingStrategy strategy,
    ILogger<TeamBuildingOrchestrationService> logger)
    : ITeamBuildingOrchestrationService
{
    private readonly Dictionary<int, bool> _hackathonBuilt = new();

    private readonly Dictionary<int, int> _expectedCounts = new();
    
    private readonly object _sync = new();
    
    public void OnHackathonStart(int hackathonId, int expectedCount)
    {
        lock (_sync)
        {
            _expectedCounts[hackathonId] = expectedCount;
            _hackathonBuilt[hackathonId] = false; 
        }
    }

    public void OnDataReceived(int hackathonId)
    {
        lock (_sync)
        {
            if (_hackathonBuilt.TryGetValue(hackathonId, out var built) && built)      
            {
                logger.LogDebug("Hackathon {HackathonId} teams already built, skipping...", hackathonId);
                return;
            }

            if (IsReadyToBuildTeams(hackathonId))
            {
                BuildAndSendTeams(hackathonId);
                _hackathonBuilt[hackathonId] = true; 
            }
        }
    }

    public bool IsReadyToBuildTeams(int hackathonId)
    {
        if (!_expectedCounts.TryGetValue(hackathonId, out var expectedCount))
        {
            return false;
        }
        
        var participantCount = participantRepo.GetParticipantCountForHackathonAsync(hackathonId).Result;

        logger.LogWarning("For Hackathon {HackathonId}: Participants={P}, Expected={E}",
            hackathonId, participantCount, expectedCount);

        return participantCount >= expectedCount;
    }

    public void BuildAndSendTeams(int hackathonId)
    {
        logger.LogWarning("Building teams for Hackathon {HackathonId}", hackathonId);

        var participants = participantRepo.GetParticipantsForHackathonAsync(hackathonId).Result;
        var wishlists = wishlistRepo.GetWishlistsForHackathonAsync(hackathonId).Result;

        var teamLeads = participants.Where(p => p.Title == Entities.Consts.ParticipantTitle.TeamLead);
        var juniors = participants.Where(p => p.Title == Entities.Consts.ParticipantTitle.Junior);
        var teamLeadsWishlists = wishlists.Where(w => w.Participant.Title == Entities.Consts.ParticipantTitle.TeamLead);
        var juniorWishlists = wishlists.Where(w => w.Participant.Title == Entities.Consts.ParticipantTitle.Junior);
        
        foreach (var participant in participants)
        {
            logger.LogInformation($"Обрабатывается участник: {participant.Name} (ID: {participant.Id})");
        }
        
        var teams = strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorWishlists).ToList();

        foreach (var team in teams)
        {
            team.HackathonId = hackathonId;
            team.Junior.HackathonId = hackathonId;
            team.TeamLead.HackathonId = hackathonId;
        }
        
        teamRepo.AddTeamsAsync(teams).Wait();

        var teamDtos = teams.Select(t => new TeamDto(
            new ParticipantDto(t.TeamLead.ParticipantId, t.TeamLead.Title.ToString(), t.TeamLead.Name),
            new ParticipantDto(t.Junior.ParticipantId, t.Junior.Title.ToString(), t.Junior.Name)
        )).ToList();

        var payload = new TeamsPayloadDto(hackathonId, teamDtos);

        hrDirectorApi.SendHackathonDataAsync(payload).Wait();
        logger.LogInformation("Teams for Hackathon {HackathonId} sent to HRDirector", hackathonId);
    }
}