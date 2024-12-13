using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Dto;
using Microsoft.Extensions.Logging;
using HRDirectorService.Services;

namespace HRDirectorService.Controllers;

[NotMapped]
[ApiController]
[Route("api/hr_director")]
public class HRDirectorController(
    ILogger<HRDirectorController> logger,
    HRDirectorOrchestrationService orchestrationService)
    : ControllerBase
{
    [HttpPost("hackathon"), AllowAnonymous]
    public async Task<IActionResult> ReceiveTeamsData([FromBody] TeamsPayloadDto payload)
    {
        logger.LogInformation("Received {Count} teams for HackathonId={HackathonId}", payload.Teams.Count, payload.HackathonId);
        await orchestrationService.ProcessTeamsAsync(payload);
        return Ok(new { Message = "Teams received" });
    }

    [HttpGet("health"), AllowAnonymous]
    public IActionResult HealthCheck()
    {
        return Ok("HRDirectorService is healthy");
    }
}