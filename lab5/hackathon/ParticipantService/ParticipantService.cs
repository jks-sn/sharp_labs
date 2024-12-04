using System.Collections.Generic;
using Entities;
using Microsoft.Extensions.Options;

namespace ParticipantService;
public class ParticipantService(IOptions<ServiceSettings> serviceSettings)
{
    private readonly ServiceSettings _serviceSettings = serviceSettings.Value;

    public Participant GetParticipant()
    {
        return _serviceSettings.Participant;
    }

    public List<Participant> GetProbableTeammates()
    {
        return _serviceSettings.ProbableTeammates;
    }
}