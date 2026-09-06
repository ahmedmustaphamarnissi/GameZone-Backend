using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;

namespace GameZoneBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public NotificationsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPut("read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendMessages()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized();

            bool ? anser = await new Business_Layer.clsNotifications(_config).ReadUserNotificationsAsync(currentUserId);
            if (anser == null)
                return NotFound("user is not found");
            return Ok(new {message = "notifications are read successfully" });
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserNotificationsAsync
        (int pageNumber, int pageSize = 8)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized();

            if(pageNumber <= 0 || pageSize <= 0)
                return BadRequest("pageNumber and pageSize must be greater than 0");
            
            var notifications = await new Business_Layer.clsNotifications(_config).
                GetUserNotificationsAsync(currentUserId, pageNumber, pageSize);
            if (notifications == null)
                return NotFound("user is not found");

            return Ok(notifications);
        }
    }
}
