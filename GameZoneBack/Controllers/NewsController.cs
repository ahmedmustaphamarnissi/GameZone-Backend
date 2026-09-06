using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTO.Auth;

namespace GameZoneBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NewsController : ControllerBase
    {

        private readonly IConfiguration _config;

        public NewsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("public")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicNews(int page , int items)
        {
            if (page <= 0 || items <= 0)
                 return BadRequest("credentials page and items must be valid");

            var data = await new Business_Layer.clsNews(_config).GetPublicNews(page, items);
            return Ok(data);
        }

        [HttpGet("user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserCommunity()
        {
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var data = await new Business_Layer.clsNews(_config).GetUserCommunityAsync(currentUserId??-1);
            if (data == null)
                return NotFound("not found news");
            return Ok(data);
        }
    }
}
