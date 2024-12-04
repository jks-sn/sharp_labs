using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using HRManagerService.Options;
using Entities;
using Entities.Consts;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace HRManagerService.Controllers
{
    [ApiController]
    [Route("api/hr_manager")]
    public class HrManagerController : ControllerBase
    {
        private readonly HRManagerService _hrManagerService;
        private readonly ControllerOptions _controllerSettings;
        private readonly ILogger<HrManagerController> _logger;

        public HrManagerController(HRManagerService hrManagerService, IOptions<ControllerOptions> controllerSettings, ILogger<HrManagerController> logger)
        {
            _hrManagerService = hrManagerService;
            _controllerSettings = controllerSettings.Value;
            _logger = logger;
        }
        
        [HttpGet("health"), AllowAnonymous]
        public IActionResult HealthCheck()
        {
            return Ok(new { Status = "Healthy", ParticipantsNumber = _controllerSettings.ParticipantsNumber });
        }
        
        [HttpPost("participant"), AllowAnonymous]
        public IActionResult AddParticipant([FromBody] ParticipantInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_hrManagerService.AllParticipantsReceived)
            {
                return BadRequest(new { Message = "All participants have already been added." });
            }

            // Конвертация строки в Enum
            if (!Enum.TryParse<ParticipantTitle>(inputModel.Title, true, out var title))
            {
                return BadRequest(new { Message = "Invalid Participant Title." });
            }

            var participant = new Participant
            {
                Id = inputModel.Id,
                Name = inputModel.Name,
                Title = title
            };

            _hrManagerService.AddParticipant(participant);
            return Ok(new { Message = "Participant added.", TotalParticipants = _hrManagerService.Participants.Count });
        }
        
        [HttpPost("wishlist"), AllowAnonymous]
        public IActionResult AddWishlist([FromBody] WishlistInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_hrManagerService.Participants.Any(p => p.Id == inputModel.ParticipantId))
            {
                return BadRequest(new { Message = $"Participant with ID {inputModel.ParticipantId} does not exist." });
            }

            // Конвертация строки в Enum
            if (!Enum.TryParse<ParticipantTitle>(inputModel.ParticipantTitle, true, out var participantTitle))
            {
                return BadRequest(new { Message = "Invalid Participant Title." });
            }

            var wishlist = new Wishlist
            {
                ParticipantId = inputModel.ParticipantId,
                ParticipantTitle = participantTitle,
                DesiredParticipants = inputModel.DesiredParticipants
            };

            _hrManagerService.AddWishlist(wishlist);
            return Ok(new { Message = "Wishlist added.", TotalWishlists = _hrManagerService.Wishlists.Count });
        }
    }
    
    public class ParticipantInputModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } // Строковое представление, будет конвертировано в Enum

        [Required]
        public string Name { get; set; }
    }
    
    public class WishlistInputModel
    {
        [Required]
        public int ParticipantId { get; set; }

        [Required]
        public string ParticipantTitle { get; set; } // Строковое представление, будет конвертировано в Enum

        [Required]
        public List<int> DesiredParticipants { get; set; } = new List<int>();
    }
}
