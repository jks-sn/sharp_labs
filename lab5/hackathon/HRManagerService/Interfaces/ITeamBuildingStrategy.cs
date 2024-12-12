//HRManagerService/Interfaces/ITeamBuildingStrategy.cs

using Entities;

namespace HRManagerService.Interface;

public interface ITeamBuildingStrategy
{
    IEnumerable<Team> BuildTeams(
        IEnumerable<Participant> teamLeads,
        IEnumerable<Participant> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists);
}