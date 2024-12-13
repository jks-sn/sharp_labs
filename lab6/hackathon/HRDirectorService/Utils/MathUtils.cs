using Dto;

namespace HRDirectorService.Utils;

public class MathUtils
{
    private double CalculateMeanSatisfaction(HackathonDto hackathonDto, ILogger<MathUtils> logger)
    {
        if (hackathonDto.Teams.Count == 0)
        {
            logger.LogWarning("No teams found for Hackathon {HackathonId}", hackathonDto.HackathonId);
            return 0.0;
        }

        double ComputeSatisfactionIndex(int participantId, string participantTitle, int partnerId, HackathonDto dto)
        {
            var wishlist = dto.Wishlists.FirstOrDefault(w =>
                w.ParticipantId == participantId &&
                w.ParticipantTitle.Equals(participantTitle, StringComparison.OrdinalIgnoreCase));

            if (wishlist == null)
            {
                throw new InvalidOperationException(
                    $"Participant {participantId} with title {participantTitle} has no wishlist.");
            }
            
            var index = Array.IndexOf(wishlist.DesiredParticipants, partnerId);
            if (index == -1)
            {
                throw new InvalidOperationException(
                    $"Partner {partnerId} not found in the wishlist of participant {participantId}.");
            }

            return 5 - index;
        }

        double totalSatisfaction = 0.0;
        int totalParticipants = 0;

        foreach (var (teamLead, junior) in hackathonDto.Teams)
        {
            try
            {
                totalSatisfaction += ComputeSatisfactionIndex(teamLead.Id, teamLead.Title, junior.Id, hackathonDto);
                totalParticipants++;

                totalSatisfaction += ComputeSatisfactionIndex(junior.Id, junior.Title, teamLead.Id, hackathonDto);
                totalParticipants++;
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex.Message);
            }
        }

        return totalParticipants > 0 ? totalSatisfaction / totalParticipants : 0.0;
    }
}