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
        if (hackathon == null)
        {
            logger.LogWarning("Hackathon with ParticipantId={ParticipantHackathonId} not found.", payload.HackathonId);
            return;
        }
        
        var participants = await participantRepo.GetParticipantsForHackathonAsync(hackathon.Id);
        if (participants == null || !participants.Any())
        {
            logger.LogWarning("No participants found for HackathonId={HackathonId}.", hackathon.Id);
            return;
        }
        
        var participantDict = participants.ToDictionary(
            p => (p.ParticipantId, p.Title),
            p => p.Id);
        
        var internalIdToParticipantId = participants.ToDictionary(p => p.Id, p => p.ParticipantId);
        
        var teamsToSave = new List<Team>();
        var teamsForCalculation = new List<TeamDto>();
        foreach (var teamDto in payload.Teams)
        {
            var teamleadParticipantId = teamDto.TeamLead.ParticipantId;
            var juniorParticipantId = teamDto.Junior.ParticipantId;

            bool teamleadFound = participantDict.TryGetValue((teamleadParticipantId, ParticipantTitle.TeamLead), out int internalTeamLeadId);
            bool juniorFound = participantDict.TryGetValue((juniorParticipantId, ParticipantTitle.Junior), out int internalJuniorId);

            if (!teamleadFound || !juniorFound)
            {
                logger.LogWarning("Cannot find internal IDs for TeamLeadParticipantId={TeamLeadId} or JuniorParticipantId={JuniorId} in HackathonId={HackathonId}",
                    teamleadParticipantId, juniorParticipantId, hackathon.Id);
                continue;
            }
            
            var team = new Team
            {
                HackathonId = hackathon.Id,
                TeamLeadId = internalTeamLeadId,
                JuniorId = internalJuniorId 
            };
            teamsToSave.Add(team);
            teamsForCalculation.Add(new TeamDto(teamDto.TeamLead, teamDto.Junior));
        }
        
        if (teamsToSave.Any())
        {
            await teamRepo.AddTeamsAsync(teamsToSave);
            logger.LogInformation("Added {Count} teams to HackathonId={HackathonId}", teamsToSave.Count, hackathon.Id);
        }
        else
        {
            logger.LogWarning("No valid teams to save for HackathonId={HackathonId}.", hackathon.Id);
            return;
        }
        
        var wishlists = await wishlistRepo.GetWishlistsForHackathonAsync(hackathon.Id);
        if (wishlists == null)
        {
            logger.LogWarning("No wishlists found for HackathonId={HackathonId}.", hackathon.Id);
            wishlists = new List<Wishlist>();
        }
        

        var teamLeadsWishlists = wishlists.Where(w => participants.Any(p => p.ParticipantId == w.ParticipantId && p.Title == ParticipantTitle.TeamLead)).ToList();
        var juniorsWishlists = wishlists.Where(w => participants.Any(p => p.ParticipantId == w.ParticipantId && p.Title == ParticipantTitle.Junior)).ToList();
        
        var meanSatisfaction = CalculateHarmonicMean(teamsForCalculation, teamLeadsWishlists, juniorsWishlists);
        hackathon.MeanSatisfactionIndex = meanSatisfaction;

        await hackathonRepo.UpdateHackathonAsync(hackathon);

        logger.LogInformation("Hackathon {HackathonId} MeanSatisfaction={Mean}", hackathon.Id, meanSatisfaction);
    }

    private static double CalculateHarmonicMean(
        IEnumerable<TeamDto> teams,
        IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists)
    {
        var satisfactionIndices = CalculateSatisfactionIndices(teams, teamLeadsWishlists, juniorsWishlists);

        int n = satisfactionIndices.Count;
        if (n == 0)
        {
            return 0.0;
        }

        double sumOfReciprocals = 0;

        foreach (var index in satisfactionIndices)
        {
            if (index > 0)
            {
                sumOfReciprocals += 1.0 / index;
            }
        }

        return sumOfReciprocals > 0 ? n / sumOfReciprocals : 0.0;
    }

    private static List<int> CalculateSatisfactionIndices(
        IEnumerable<TeamDto> teams,
        IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists)
    {
        var satisfactionIndices = new List<int>();

        foreach (var team in teams)
        {
            var teamLeadParticipantId = team.TeamLead.ParticipantId;
            var juniorParticipantId = team.Junior.ParticipantId;

            var teamLeadWishlist = teamLeadsWishlists.FirstOrDefault(w => w.ParticipantId == teamLeadParticipantId)?.DesiredParticipants;
            if (teamLeadWishlist != null)
            {
                int teamLeadSatisfaction = GetSatisfactionScore(teamLeadWishlist, juniorParticipantId);
                satisfactionIndices.Add(teamLeadSatisfaction);
            }
            
            var juniorWishlist = juniorsWishlists.FirstOrDefault(w => w.ParticipantId == juniorParticipantId)?.DesiredParticipants;
            if (juniorWishlist != null)
            {
                int juniorSatisfaction = GetSatisfactionScore(juniorWishlist, teamLeadParticipantId);
                satisfactionIndices.Add(juniorSatisfaction);
            }
        }

        return satisfactionIndices;
    }

    private static int GetSatisfactionScore(int[] wishlist, int assignedPartnerParticipantId)
    {
        int position = Array.IndexOf(wishlist, assignedPartnerParticipantId);
        return position >= 0 ? wishlist.Length - position : 0;
    }
}
