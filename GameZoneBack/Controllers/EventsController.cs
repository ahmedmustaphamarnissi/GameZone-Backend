using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameZoneBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EventsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetFriendRequests()
        {
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(currentUserId ?? -1);
            if (checkForUser == false)
                return NotFound("not found a user with this id");

            var Data = await new Business_Layer.clsEvents(_config).GetEventDetails();
            if (Data == null)
                return BadRequest("not found any not found");
            return Ok(Data);
        }
    }
}
