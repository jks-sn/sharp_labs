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
        
        
        var wishlists = await wishlistRepo.GetWishlistsForHackathonAsync(hackathon.Id);
        var teamLeadsWishlists = wishlists.Where(w => participants.Any(p => p.Id == w.ParticipantId && p.Title == ParticipantTitle.TeamLead));
        var juniorsWishlists = wishlists.Where(w => participants.Any(p => p.Id == w.ParticipantId && p.Title == ParticipantTitle.Junior));

        var meanSatisfaction = CalculateHarmonicMean(teams, teamLeadsWishlists, juniorsWishlists);
        hackathon.MeanSatisfactionIndex = meanSatisfaction;

        await hackathonRepo.UpdateHackathonAsync(hackathon);

        logger.LogInformation("Hackathon {HackathonId} MeanSatisfaction={Mean}", hackathon.Id, meanSatisfaction);
    }

    private static double CalculateHarmonicMean(
        IEnumerable<Team> teams,
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
        IEnumerable<Team> teams,
        IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists)
    {
        var satisfactionIndices = new List<int>();

        foreach (var team in teams)
        {
            var teamLeadWishlist = teamLeadsWishlists.FirstOrDefault(w => w.ParticipantId == team.TeamLeadId)?.DesiredParticipants;
            var juniorWishlist = juniorsWishlists.FirstOrDefault(w => w.ParticipantId == team.JuniorId)?.DesiredParticipants;

            if (teamLeadWishlist != null)
            {
                int teamLeadSatisfaction = GetSatisfactionScore(teamLeadWishlist, team.JuniorId);
                satisfactionIndices.Add(teamLeadSatisfaction);
            }

            if (juniorWishlist != null)
            {
                int juniorSatisfaction = GetSatisfactionScore(juniorWishlist, team.TeamLeadId);
                satisfactionIndices.Add(juniorSatisfaction);
            }
        }

        return satisfactionIndices;
    }

    private static int GetSatisfactionScore(int[] wishlist, int assignedPartner)
    {
        int position = Array.IndexOf(wishlist, assignedPartner);
        return position >= 0 ? wishlist.Length - position : 0;
    }
}
