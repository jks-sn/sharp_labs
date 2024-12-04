
using System.Diagnostics;
using Entities.Interface;

namespace Entities;

public class HRManager(ITeamBuildingStrategy strategy)
{
    public IEnumerable<Team> BuildTeams(
        IEnumerable<Participant> teamLeads,
        IEnumerable<Participant> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists)
    {
        Debug.Assert(((List<Participant>)teamLeads).Count == ((List<Participant>)juniors).Count);
        return strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);
    }
}