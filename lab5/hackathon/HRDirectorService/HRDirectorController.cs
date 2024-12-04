using Dto;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace HRDirectorService.Controllers;

[ApiController]
[Route("api/hr_director")]
public class HRDirectorController : ControllerBase
{
    private readonly HRDirectorService _hrDirectorService;

    public HRDirectorController(HRDirectorService hrDirectorService)
    {
        _hrDirectorService = hrDirectorService;
    }
    
    [HttpPost("participants")]
    public IActionResult ReceiveParticipants([FromBody] List<ParticipantDto> participantsDto)
    {
        var participants = participantsDto.Select(p => new Participant(p.Id, p.Title, p.Name)).ToList();
        _hrDirectorService.SetParticipants(participants);
        return Ok();
    }
    
    [HttpPost("teams")]
    public IActionResult ReceiveTeams([FromBody] List<TeamDto> teamsDto)
    {
        var teams = teamsDto.Select(t => new Team
        {
            TeamLead = new Participant(t.TeamLead.Id, t.TeamLead.Title, t.TeamLead.Name),
            Junior = new Participant(t.Junior.Id, t.Junior.Title, t.Junior.Name)
        }).ToList();

        _hrDirectorService.SetTeams(teams);

        return Ok();
    }
    
    [HttpPost("wishlists")]
    public IActionResult ReceiveWishlists([FromBody] List<WishlistDto> wishlistsDto)
    {
        var wishlists = wishlistsDto.Select(w => new Wishlist(
            w.ParticipantId, w.ParticipantTitle, w.DesiredParticipants)).ToList();

        _hrDirectorService.SetWishlists(wishlists);

        return Ok();
    }
}