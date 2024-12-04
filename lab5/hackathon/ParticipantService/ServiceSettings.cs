using Entities;

namespace ParticipantService;

public class ServiceSettings
{
    public Participant Participant { get; set; }
    public List<Participant> ProbableTeammates { get; set; }
}