using System.Threading.Tasks;
using Dto;
using HRDirectorService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRDirectorService.Controllers;

[ApiController]
[Route("api/hr_director")]
public class HRDirectorController(HackathonService hackathonService, ILogger<HRDirectorController> logger)
    : ControllerBase
{
    [HttpGet("health"), AllowAnonymous]
    public Task<IActionResult> HealthCheck() => Task.FromResult<IActionResult>(Ok());
    
    [HttpPost("hackathon")]
    public async Task<IActionResult> AddHackathon([FromBody] HackathonDto hackathonDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        logger.LogWarning("Received Hackathon {HackathonId} data from HRManager.", hackathonDto.HackathonId);

        await hackathonService.ProcessHackathonAsync(hackathonDto);
        return Ok(new { Message = "Hackathon processed." });
    }
}