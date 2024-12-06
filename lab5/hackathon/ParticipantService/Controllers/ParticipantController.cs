// ParticipantService/ParticipantController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ParticipantService.Options;

namespace ParticipantService
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantController : ControllerBase
    {
        private readonly ServiceOptions _serviceOptions;

        public ParticipantController(IOptions<ServiceOptions> serviceOptions)
        {
            _serviceOptions = serviceOptions.Value;
        }
        
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { Status = "Healthy", Participant = _serviceOptions.Participant.Name });
        }
    }
}