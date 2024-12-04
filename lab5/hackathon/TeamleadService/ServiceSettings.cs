using Entities;

namespace TeamleadService;

public class ServiceSettings
{
    public Participant Participant { get; set; }
    public IEnumerable<Participant> ProbableTeammates { get; set; }
}