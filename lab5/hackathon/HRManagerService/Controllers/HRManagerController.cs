//HRManagerService/Controllers/HRManagerController.cs

using System.ComponentModel.DataAnnotations;
using Entities;
using HRManagerService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using HRManagerService.Options;
using HRManagerService.Services;

namespace HRManagerService.Controllers;

[ApiController]
[Route("api/hr_manager")]
public class HRManagerController(
    IParticipantService participantService,
    IOptions<ControllerOptions> controllerSettings,
    ILogger<HRManagerController> logger)
    : ControllerBase
{
    private readonly ControllerOptions _controllerSettings = controllerSettings.Value;
    private readonly ILogger<HRManagerController> _logger = logger;

    [HttpGet("health"), AllowAnonymous]
    public IActionResult HealthCheck()
    {
        return Ok(new { Status = "Healthy", ParticipantsNumber = _controllerSettings.ParticipantsNumber });
    }

    [HttpPost("participant"), AllowAnonymous]
    public async Task<IActionResult> AddParticipant([FromBody] ParticipantInputModel inputModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Добавление участника через HRManagerController: {Id}, {Title}, {Name}",
                inputModel.Id, inputModel.Title, inputModel.Name);
            await participantService.AddParticipantAsync(inputModel);
            return Ok(new { Message = "Participant added." });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("wishlist"), AllowAnonymous]
    public async Task<IActionResult> AddWishlist([FromBody] WishlistInputModel inputModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation(
                "Добавление wishlist через HRManagerController: ParticipantId={ParticipantId}, ParticipantTitle={ParticipantTitle}, Count={Count}",
                inputModel.ParticipantId, inputModel.ParticipantTitle.ToString(), inputModel.DesiredParticipants.Count);
            await participantService.AddWishlistAsync(inputModel);
            return Ok(new { Message = "Wishlist added." });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}