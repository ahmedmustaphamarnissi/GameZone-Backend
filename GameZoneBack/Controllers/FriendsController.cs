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
    public class FriendsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public FriendsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("requests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetFriendRequests(int pageSize, int pageNumber)
        {
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(currentUserId ?? -1);
            if (checkForUser == false)
                return NotFound("not found a user with this id");

            var Data = await new Business_Layer.clsFriendRequests(_config).GetFriendRequestsAsync(currentUserId ?? -1, pageSize, pageNumber);
            if (Data == null)
                return BadRequest("Friend requests are not found");
            return Ok(Data);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PerformActionOnFriendRequest([FromBody] PerformActionOnFriendRequestDTO dto)
        {
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(currentUserId ?? -1);
            if (checkForUser == false)
                return NotFound("not found a user with this id");

            var Data = await new Business_Layer.clsFriendRequests(_config).PerformActionOnFriendRequest(currentUserId ?? -1, dto);
            if (Data == null)
                return BadRequest("can not perform action on friend request");
            return Ok(Data);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetFriends([FromQuery] GetFriendsRequest request)
        {
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(currentUserId ?? -1);
            if (checkForUser == false)
                return NotFound("not found a user with this id");

            var Data = await new Business_Layer.clsFriendRequests(_config).GetFriendAsync(currentUserId ?? -1, request);
            if (Data == null)
                return BadRequest("not found friends");
            return Ok(Data);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteFriendById(int friendRequestId)
        {
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(currentUserId ?? -1);
            if (checkForUser == false)
                return NotFound("not found a user with this id");

            var checkForFriend = await new Business_Layer.clsFriendRequests(_config).CheckIfExistFriend(friendRequestId);
            if(!checkForFriend)
                return NotFound("not found a freindship with this id");
            var Data = await new Business_Layer.clsFriendRequests(_config).DeleteFriendAsync(friendRequestId);
            if (!Data )
                return BadRequest("friend is not removed");
            return Ok(Data);
        }

        [HttpGet("blocked")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetBlockedFriends([FromQuery] GetFriendsRequest request)
        {
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(currentUserId ?? -1);
            if (checkForUser == false)
                return NotFound("not found a user with this id");

            var Data = await new Business_Layer.clsFriendRequests(_config).GetBlockedFriendsAsync(currentUserId ?? -1, request);
            if (Data == null)
                return BadRequest("not found blocked friends");
            return Ok(Data);
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersRequest request)
        {
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(currentUserId ?? -1);
            if (checkForUser == false)
                return NotFound("not found a user with this id");

            var Data = await new Business_Layer.clsFriendRequests(_config).GetUsersAsync(currentUserId ?? -1, request);
            if (Data == null)
                return BadRequest("not found blocked friends");
            return Ok(Data);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SendFriendRequest(int userId)
        {
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(userId);
            if (checkForUser == false)
                return NotFound("not found a user with this id");

            var checkForFriend = await new Business_Layer.clsFriendRequests(_config).
                CheckIfExistFriendship(currentUserId ?? -1, userId);
            if (!checkForFriend)
            {
                var res = await new Business_Layer.clsFriendRequests(_config).
                CreateNewFrienshipAsync(currentUserId ?? -1, userId);
                if (!res)
                {
                    return BadRequest("Request is not sended");
                }
                return Ok(res);
            }
            else
            {
                var res = await new Business_Layer.clsFriendRequests(_config).
                    UpdateStateFromRejectedToPending(currentUserId ?? -1, userId);
                if (!res)
                {
                    return BadRequest("Request is not sended");
                }
                return Ok(res);
            }
        }

        [HttpGet("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUserFriends([FromQuery] GetUsersRequest request , int Id)
        {
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (Id == currentUserId)
                return BadRequest("this api can't be called for the user it self");
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(Id);
            if (checkForUser == false)
                return NotFound("not found a user with this id");
            
            var Data = await new Business_Layer.clsFriendRequests(_config).GetUserFriendsAsync(Id, request,currentUserId);
            if (Data == null)
                return BadRequest("not found user friends");
            return Ok(Data);
        }
    }
}
