// ParticipantService/ParticipantService.cs

using Entities;
using Entities.Consts;
using Microsoft.Extensions.Options;
using ParticipantService.Options;

namespace ParticipantService.Services;
public class ParticipantService
{
    private readonly Participant _participant;
    private readonly List<Participant> _probableTeammates;

    public ParticipantService(DataLoader dataLoader, IOptions<ServiceOptions> serviceOptions)
    {
        var options = serviceOptions.Value;

        // Получаем данные участника из настроек
        _participant = options.Participant;

        // Определяем роль напарников (если Junior, то ищем TeamLead, и наоборот)
        var teammateTitle = _participant.Title == ParticipantTitle.Junior ? ParticipantTitle.TeamLead : ParticipantTitle.Junior;

        // Загружаем потенциальных напарников через DataLoader
        _probableTeammates = dataLoader.LoadProbableTeammates(teammateTitle);
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