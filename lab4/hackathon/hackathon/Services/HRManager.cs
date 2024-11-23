// Services/HRManager.cs

using Hackathon.Interface;
using Hackathon.Strategy;
using Hackathon.Options;
using Hackathon.Model;
using Microsoft.Extensions.Options;

namespace Hackathon.Services;
public class HRManager(IOptions<HRManagerOptions> options, IAssignmentStrategyFactory strategyFactory) : IHRManager
{
    private readonly IAssignmentStrategy _strategy = strategyFactory.GetStrategy(options.Value.AssignmentStrategy);
    public List<Team> AssignTeams(List<Junior> juniors, List<TeamLead> teamLeads)
    {
        return _strategy.AssignPairs(juniors, teamLeads);
    }
}
