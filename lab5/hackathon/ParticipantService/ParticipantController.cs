using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ParticipantService
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantController : ControllerBase
    {
        private readonly ServiceSettings _serviceSettings;

        public ParticipantController(IOptions<ServiceSettings> serviceSettings)
        {
            _serviceSettings = serviceSettings.Value;
        }
        
        [HttpGet("health"), AllowAnonymous]
        public IActionResult HealthCheck()
        {
            return Ok(new { Status = "Healthy", Participant = _serviceSettings.Participant.Name });
        }
    }
}