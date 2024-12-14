// ParticipantService/ParticipantService.cs

using Microsoft.Extensions.Options;
using ParticipantService.Entities;
using ParticipantService.Entities.Consts;
using ParticipantService.Options;

namespace ParticipantService.Services;
public class ParticipantService
{
    private readonly Participant _participant;
    private readonly List<Participant> _probableTeammates;

    public ParticipantService(DataLoader dataLoader, IOptions<ServiceOptions> serviceOptions)
    {
        var options = serviceOptions.Value;
        
        _participant = options.Participant;
        
        var teammateTitle = _participant.Title == ParticipantTitle.Junior ? ParticipantTitle.TeamLead : ParticipantTitle.Junior;
        
        _probableTeammates = dataLoader.LoadProbableTeammates(teammateTitle);
        Console.WriteLine($"I AM a {_participant.Name} and my teammate: {_probableTeammates[0].Name}");
    }

    public Participant GetParticipant()
    {
        return _participant;
    }

    public List<Participant> GetProbableTeammates()
    {
        return _probableTeammates;
    }
}