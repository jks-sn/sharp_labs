using System.Threading.Tasks;
using Entities;
using Entities.Consts;
using HRManagerService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRManagerService.Controllers;

[ApiController]
[Route("api/hr_manager")]
public class ParticipantController : ControllerBase
{
    private readonly IParticipantService _participantService;
    private readonly ILogger<ParticipantController> _logger;

    public ParticipantController(IParticipantService participantService, ILogger<ParticipantController> logger)
    {
        _participantService = participantService;
        _logger = logger;
    }

    [HttpPost("participant")]
    public async Task<IActionResult> AddParticipant([FromBody] ParticipantInputModel inputModel)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid participant data received.");
            return BadRequest(ModelState);
        }

        await _participantService.AddParticipantAsync(inputModel);
        _logger.LogInformation("Participant added: {Id}", inputModel.Id);

        return Ok(new { Message = "Participant added." });
    }

    [HttpPost("wishlist")]
    public async Task<IActionResult> AddWishlist([FromBody] WishlistInputModel inputModel)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid wishlist data received.");
            return BadRequest(ModelState);
        }

        await _participantService.AddWishlistAsync(inputModel);
        _logger.LogInformation("Wishlist added for participant: {ParticipantId}", inputModel.ParticipantId);

        return Ok(new { Message = "Wishlist added." });
    }
}