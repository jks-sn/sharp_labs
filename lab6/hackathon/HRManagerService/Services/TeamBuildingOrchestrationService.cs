using HRManagerService.Interfaces;
using Microsoft.Extensions.Logging;
using Entities;
using System.Linq;
using Dto;
using HRManagerService.Clients;
using HRManagerService.Interface;

namespace HRManagerService.Services;

public class TeamBuildingOrchestrationService(
    IParticipantRepository participantRepo,
    IWishlistRepository wishlistRepo,
    ITeamRepository teamRepo,
    ITeamBuildingStrategy strategy,
    IHRDirectorApi hrDirectorApi,
    ILogger<TeamBuildingOrchestrationService> logger)
    : ITeamBuildingOrchestrationService
{
    private readonly Dictionary<int,int> _expectedCounts = new();

    public void OnHackathonStart(int hackathonId, int expectedCount)
    {
        _expectedCounts[hackathonId] = expectedCount;
        logger.LogInformation("Hackathon {HackathonId} started, expecting {Count}", hackathonId, expectedCount);
    }

    public void OnDataReceived(int hackathonId)
    {
        if (IsReadyToBuildTeams(hackathonId))
        {
            BuildAndSendTeams(hackathonId);
        }
    }
    
    public bool IsReadyToBuildTeams(int hackathonId)
    {
        if (!_expectedCounts.TryGetValue(hackathonId, out var expectedCount))
        {
            return false;
        }

        var participantCount = participantRepo.GetParticipantCountForHackathonAsync(hackathonId).Result;
        var wishlistCount = wishlistRepo.GetWishlistCountForHackathonAsync(hackathonId).Result;
        
        logger.LogInformation("For Hackathon {HackathonId}: Participants={P}, Wishlists={W}, Expected={E}",
            hackathonId, participantCount, wishlistCount, expectedCount);

        return participantCount >= expectedCount && wishlistCount >= expectedCount;
    }
    public void BuildAndSendTeams(int hackathonId)
    {
        logger.LogInformation("Building teams for Hackathon {HackathonId}", hackathonId);

        var participants = participantRepo.GetParticipantsForHackathonAsync(hackathonId).Result;
        var wishlists = wishlistRepo.GetWishlistsForHackathonAsync(hackathonId).Result;

        var teamLeads = participants.Where(p => p.Title == Entities.Consts.ParticipantTitle.TeamLead);
        var juniors = participants.Where(p => p.Title == Entities.Consts.ParticipantTitle.Junior);
        var teamLeadsWishlists = wishlists.Where(w => w.Participant.Title == Entities.Consts.ParticipantTitle.TeamLead);
        var juniorWishlists = wishlists.Where(w => w.Participant.Title == Entities.Consts.ParticipantTitle.Junior);

        var teams = strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorWishlists).ToList();
        
        teamRepo.AddTeamsAsync(teams).Wait(); 
        
        var teamDtos = teams.Select(t => new TeamDto(
            new ParticipantDto(t.TeamLead.Id, t.TeamLead.Title.ToString(), t.TeamLead.Name),
            new ParticipantDto(t.Junior.Id, t.Junior.Title.ToString(), t.Junior.Name)
        )).ToList();

        var payload = new TeamsPayloadDto(hackathonId, teamDtos);
        
        hrDirectorApi.SendHackathonDataAsync(payload).Wait();
        logger.LogInformation("Teams for Hackathon {HackathonId} sent to HRDirector", hackathonId);
    }
}
