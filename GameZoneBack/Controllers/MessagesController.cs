using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Data.enums;
using Models.DTO;

namespace GameZoneBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public MessagesController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> GetMessages(int pageNumber = 1)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized();

            var data = await new Business_Layer.clsMessages(_config).GetMessagesAsync(currentUserId, pageNumber);
            return Ok(data);
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserMessages(int userId, int pageNumber = 1)
        {
            if (userId <= 0 || pageNumber <= 0)
                return BadRequest();

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized();

            var data = await new Business_Layer.clsMessages(_config)
                .GetMessagesAsync(currentUserId, userId, pageNumber);

            return Ok(data);
        }

        [HttpPut("read/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReadUserMessages(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID.");
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized();

            var checkForFriendship = await new Business_Layer.clsFriendRequests(_config).CheckIfIdFromFriendsList(currentUserId, userId);
            if(!checkForFriendship)
                return BadRequest("You are not friends with this user.");

            var data = await new Business_Layer.clsMessages(_config)
                .ReadUserMessages(currentUserId, userId);
            if (data == false)
                return StatusCode(
                    StatusCodes.Status500InternalServerError, "reading messages is failed");

            return Ok(new {message = "user messages are read successfuly"});
        }

        [HttpPut("remove")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoveMessage([FromBody] RemoveMessageDTO removeMessageDto)
        {
            if(removeMessageDto.messageId <= 0)
                return BadRequest("Invalid message ID.");
            if (!Enum.IsDefined(
        typeof(MessageRemoveType),
        removeMessageDto.removeType))
            {
                return BadRequest("Invalid remove type.");
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized();

            var checkForMessage = await new Business_Layer.clsMessages(_config).CeckIfTheUserIsTheSender(currentUserId, removeMessageDto.messageId);
            if (!checkForMessage)
                return Forbid("You are not the owner of this message.");

            var data = await new Business_Layer.clsMessages(_config)
                .RemoveUserMessageAsync(removeMessageDto);
            if (data == null)
                return NotFound("Message not found.");
            if (data == false)
                return StatusCode(
                    StatusCodes.Status500InternalServerError, "Removing message failed");

            return Ok(new { message = "Message removed successfully" });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendMessages([FromBody] SendMessageDTO sendMessageDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized();

            var checkForFriendship = await new Business_Layer.clsFriendRequests(_config).CheckIfIdFromFriendsList(currentUserId, sendMessageDto.receiverId);
            if (!checkForFriendship)
                return BadRequest("You are not friends with this user.");

            var data = await new Business_Layer.clsMessages(_config)
                .SendMessageAsync(currentUserId, sendMessageDto);
            if (data == false)
                return StatusCode(
                    StatusCodes.Status500InternalServerError, "sending message failed");

            return Ok(new { message = "Message sent successfully" });
        }
    }
}
