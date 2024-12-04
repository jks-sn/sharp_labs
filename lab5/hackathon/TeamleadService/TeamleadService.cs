using Entities;
using Microsoft.Extensions.Options;

namespace TeamleadService;

public class TeamleadService
{
    public Participant Participant { get; }
    public IEnumerable<Participant> ProbableTeammates { get; }
    public readonly TaskCompletionSource<bool> HackathonStartedTcs = new();

    public TeamleadService(IOptions<ServiceSettings> settings)
    {
        Participant = settings.Value.Participant;
        ProbableTeammates = settings.Value.ProbableTeammates;
    }
}