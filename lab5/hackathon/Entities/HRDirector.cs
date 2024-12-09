//Entities/HRDirector.cs
namespace Entities;

public class HRDirector
{
    public double CalculateMeanSatisfactionIndex(
        IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists,
        IEnumerable<Team> teams)
    {
        var totalSatisfaction = 0;
        var totalParticipants = 0;

        foreach (var team in teams)
        {
            var teamLeadWishlist = teamLeadsWishlists.First(w => w.ParticipantId == team.TeamLead.Id);
            var juniorWishlist = juniorsWishlists.First(w => w.ParticipantId == team.Junior.Id);

            var teamLeadSatisfaction = Array.IndexOf(teamLeadWishlist.DesiredParticipants, team.Junior.Id);
            var juniorSatisfaction = Array.IndexOf(juniorWishlist.DesiredParticipants, team.TeamLead.Id);

            totalSatisfaction += (teamLeadSatisfaction >= 0 ? teamLeadSatisfaction : 0);
            totalSatisfaction += (juniorSatisfaction >= 0 ? juniorSatisfaction : 0);
            totalParticipants += 2;
        }

        return (double)totalSatisfaction / totalParticipants;
    }
}