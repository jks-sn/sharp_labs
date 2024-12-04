using Entities;

namespace JuniorService;

public class ServiceSettings
{
    public Participant Participant { get; set; }
    public IEnumerable<Participant> ProbableTeammates { get; set; }
}