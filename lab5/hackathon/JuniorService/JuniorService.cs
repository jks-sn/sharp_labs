using Entities;
using Microsoft.Extensions.Options;

namespace JuniorService;

public class JuniorService(IOptions<ServiceSettings> settings)
{
    public Participant Participant { get; } = settings.Value.Participant;
    public IEnumerable<Participant> ProbableTeammates { get; } = settings.Value.ProbableTeammates;
}