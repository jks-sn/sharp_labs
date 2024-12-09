using System;
using System.Linq;
using System.Threading.Tasks;
using Dto;
using Entities;
using HRDirectorService.Interfaces;
using Microsoft.Extensions.Logging;

namespace HRDirectorService.Services;

public class HackathonService(IHackathonRepository hackathonRepo, ILogger<HackathonService> logger)
{
    public async Task ProcessHackathonAsync(HackathonDto hackathonDto)
    {
        double meanSatisfactionIndex = CalculateMeanSatisfaction(hackathonDto);
        
        var hackathon = new Hackathon
        {
            Id = hackathonDto.HackathonId, 
            MeanSatisfactionIndex = meanSatisfactionIndex
        };
        
        await hackathonRepo.AddHackathonAsync(hackathon);
        logger.LogWarning("Hackathon {HackathonId} processed. MeanSatisfactionIndex={Mean}", hackathon.Id, hackathon.MeanSatisfactionIndex);
    }

    private double CalculateMeanSatisfaction(HackathonDto hackathonDto)
    {
        if (hackathonDto.Teams.Count == 0)
        {
            logger.LogWarning("No teams found for Hackathon {HackathonId}", hackathonDto.HackathonId);
            return 0.0;
        }
        
        var participants = hackathonDto.Participants;
        var wishlists = hackathonDto.Wishlists;
        var teams = hackathonDto.Teams;
        
        int GetIndexInWishlist(int participantId, string participantTitle, int targetId)
        {
            var wishlist = wishlists.FirstOrDefault(w =>
                w.ParticipantId == participantId && w.ParticipantTitle.Equals(participantTitle, StringComparison.OrdinalIgnoreCase));

            if (wishlist == null || wishlist.DesiredParticipants.Count == 0)
            {
                return int.MaxValue; // Если нет вишлиста, считаем неудовлетворённым
            }

            var idx = wishlist.DesiredParticipants.IndexOf(targetId);
            return idx == -1 ? wishlist.DesiredParticipants.Count : idx; // Если партнера нет в списке, считаем максимальный индекс
        }

        var totalSatisfaction = 0.0;
        var totalParticipants = 0;

        foreach (var (teamLead, junior) in teams)
        {
            var teamLeadIndex = GetIndexInWishlist(teamLead.Id, teamLead.Title, junior.Id);
            var teamLeadSatisfaction = 0.0;
            if (teamLeadIndex == int.MaxValue)
            {
                teamLeadSatisfaction = 0.0;
            }
            else
            {
                var count = wishlists.FirstOrDefault(w => w.ParticipantId == teamLead.Id && w.ParticipantTitle.Equals(teamLead.Title, StringComparison.OrdinalIgnoreCase))
                           ?.DesiredParticipants.Count ?? 1;
                if (count > 1)
                {
                    teamLeadSatisfaction = 1.0 - (teamLeadIndex / (double)(count - 1));
                }
                else
                {
                    teamLeadSatisfaction = 1.0;
                }
            }

            totalSatisfaction += teamLeadSatisfaction;
            totalParticipants++;

            
            var juniorIndex = GetIndexInWishlist(junior.Id, junior.Title, teamLead.Id);
            var juniorSatisfaction = 0.0;
            if (juniorIndex == int.MaxValue)
            {
                juniorSatisfaction = 0.0;
            }
            else
            {
                var count = wishlists.FirstOrDefault(w => w.ParticipantId == junior.Id && w.ParticipantTitle.Equals(junior.Title, StringComparison.OrdinalIgnoreCase))
                           ?.DesiredParticipants.Count ?? 1;
                if (count > 1)
                {
                    juniorSatisfaction = 1.0 - (juniorIndex / (double)(count - 1));
                }
                else
                {
                    juniorSatisfaction = 1.0;
                }
            }

            totalSatisfaction += juniorSatisfaction;
            totalParticipants++;
        }

        if (totalParticipants == 0)
        {
            return 0.0;
        }
        return totalSatisfaction / totalParticipants;
    }
}
