//HRManagerService/Services/TeamBuildingOrchestrationService.cs

using System.ComponentModel.DataAnnotations.Schema;
using HRManagerService.Interfaces;
using Microsoft.Extensions.Logging;
using Entities;
using System.Linq;
using Dto;
using HRManagerService.Clients;
using HRManagerService.Interface;

namespace HRManagerService.Services;

[NotMapped]
public class TeamBuildingOrchestrationService(
    IServiceProvider serviceProvider,
    ILogger<TeamBuildingOrchestrationService> logger)
    : ITeamBuildingOrchestrationService
{
    private readonly Dictionary<int, int> _expectedCounts = new();
    private readonly HashSet<int> _hackathonBuilt = new();
    
    private readonly object _sync = new();
    
    public void OnHackathonStart(int hackathonId, int expectedCount)
    {
        _expectedCounts[hackathonId] = expectedCount;
        logger.LogInformation("Hackathon {HackathonId} started, expecting {Count}", hackathonId, expectedCount);
    }

    public void OnDataReceived(int hackathonId)
    {
        lock (_sync)
        {
            if (_hackathonBuilt.Contains(hackathonId))
            {
                logger.LogDebug("Hackathon {HackathonId} teams already built, skipping...", hackathonId);
                return;
            }

            if (IsReadyToBuildTeams(hackathonId))
            {
                BuildAndSendTeams(hackathonId);

                _hackathonBuilt.Add(hackathonId);
            }
        }
    }

    public bool IsReadyToBuildTeams(int hackathonId)
    {
        using var scope = serviceProvider.CreateScope();
        var participantRepo = scope.ServiceProvider.GetRequiredService<IParticipantRepository>();
        var wishlistRepo = scope.ServiceProvider.GetRequiredService<IWishlistRepository>();
        if (!_expectedCounts.TryGetValue(hackathonId, out var expectedCount))
        {
            return false;
        }
        
        var participantCount = participantRepo.GetParticipantCountForHackathonAsync(hackathonId).Result;
        var wishlistCount = wishlistRepo.GetWishlistCountForHackathonAsync(hackathonId).Result;

        logger.LogWarning("For Hackathon {HackathonId}: Participants={P}, Wishlists={W}, Expected={E}",
            hackathonId, participantCount, wishlistCount, expectedCount);

        return participantCount >= expectedCount && wishlistCount >= expectedCount;
    }

    public void BuildAndSendTeams(int hackathonId)
    {
        using var scope = serviceProvider.CreateScope();
        var participantRepo = scope.ServiceProvider.GetRequiredService<IParticipantRepository>();
        var wishlistRepo = scope.ServiceProvider.GetRequiredService<IWishlistRepository>();
        var teamRepo = scope.ServiceProvider.GetRequiredService<ITeamRepository>();
        var hrDirectorApi = scope.ServiceProvider.GetRequiredService<IHRDirectorApi>();
        var strategy = scope.ServiceProvider.GetRequiredService<ITeamBuildingStrategy>();
        
        logger.LogWarning("Building teams for Hackathon {HackathonId}", hackathonId);

        var participants = participantRepo.GetParticipantsForHackathonAsync(hackathonId).Result;
        var wishlists = wishlistRepo.GetWishlistsForHackathonAsync(hackathonId).Result;

        var teamLeads = participants.Where(p => p.Title == Entities.Consts.ParticipantTitle.TeamLead);
        var juniors = participants.Where(p => p.Title == Entities.Consts.ParticipantTitle.Junior);
        var teamLeadsWishlists = wishlists.Where(w => w.Participant.Title == Entities.Consts.ParticipantTitle.TeamLead);
        var juniorWishlists = wishlists.Where(w => w.Participant.Title == Entities.Consts.ParticipantTitle.Junior);

        var teams = strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorWishlists).ToList();

        foreach (var team in teams)
        {
            team.HackathonId = hackathonId;
            team.Junior.HackathonId = hackathonId;
            team.TeamLead.HackathonId = hackathonId;
        }
        
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