using System.Linq;
using System.Threading.Tasks;
using Dto;
using Entities;
using HRManagerService.Clients;
using HRManagerService.Data;
using HRManagerService.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRManagerService.Services;
public class HRDirectorClientService(
    IHRDirectorApi hrDirectorApi,
    IHackathonRepository hackathonRepo,
    ILogger<HRDirectorClientService> logger)
    : IHRDirectorClient
{
    public async Task SendHackathonDataAsync(int hackathonId)
    {
        var hackathon = await hackathonRepo.GetByIdAsync(hackathonId);
        if (hackathon == null)
        {
            logger.LogError("Hackathon with id {Id} not found.", hackathonId);
            return;
        }

        var participantDtos = hackathon.Participants
            .Select(p => new ParticipantDto(p.Id, p.Title, p.Name))
            .ToList();

        var wishlistDtos = hackathon.Wishlists
            .Select(w => new WishlistDto(w.ParticipantId, w.ParticipantTitle, w.DesiredParticipants))
            .ToList();

        var teamDtos = hackathon.Teams
            .Select(t => new TeamDto(
                new ParticipantDto(t.TeamLead.Id, t.TeamLead.Title, t.TeamLead.Name),
                new ParticipantDto(t.Junior.Id, t.Junior.Title, t.Junior.Name)
            ))
            .ToList();

        var hackathonDto = new HackathonDto(hackathon.Id, hackathon.MeanSatisfactionIndex, participantDtos, wishlistDtos, teamDtos);

        await hrDirectorApi.SendHackathonDataAsync(hackathonDto);
        logger.LogInformation("Hackathon {Id} data sent to HRDirector.", hackathonId);
    }
}
