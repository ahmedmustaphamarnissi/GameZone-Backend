using System.Security.Claims;
using GameZoneBack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;

namespace GameZoneBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public UsersController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserProfile(int Id)
        {

            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserProfileExistAsync(Id);
            if (checkForUser == false)
                return NotFound("not found a user with this id");
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId == null)
                return StatusCode(StatusCodes.Status403Forbidden, "access denied");
            var checkForBlock = await new Business_Layer.clsUser(_config).CheckIfTheUserBlocked(Id, userId ?? -1);
            if (checkForBlock == true)
                return NotFound("you and the user are blocked you can check your block list !");
            if (userId == Id)
            {
                var res = await new Business_Layer.clsUser(_config).GetUserProfileAsync(Id, true, userId);
                if (res == null)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "error founding user profile");
                return Ok(res);
            }
            else
            {
                var res = await new Business_Layer.clsUser(_config).GetUserProfileAsync(Id, false, userId);
                if (res == null)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "error founding your profile");
                return Ok(res);
            }
            
        }
        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserDashboard()
        {

            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId == null)
                return StatusCode(StatusCodes.Status403Forbidden, "access denied");
            var result = await new Business_Layer.clsUser(_config).GetUserDashboardAsync(userId??-1);
            if (result == null)
                return NotFound("not found user dashboard");
            return Ok(result);

        }

        [HttpGet("settings/profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserProfileSettings()
        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized();

            var result = await new Business_Layer.clsUser(_config).GetUserProfileSettingsAsync(currentUserId);
            if (result == null)
                return NotFound("not found user profile settings");
            return Ok(result);

        }

        [HttpPost("deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeactivateUserAccount(
    [FromBody] DeactivateAccountDTO request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized();

            var userBusiness = new Business_Layer.clsUser(_config);

            var user = await userBusiness.GetUserByUserIdAsync(currentUserId);

            if (user == null)
                return NotFound("User not found.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return Unauthorized("Invalid password.");

            bool? result = await userBusiness.InactiveUserAccount(currentUserId);

            if (result == null)
                return NotFound("User not found.");

            if (result == false)
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Deactivate account operation failed.");

            return Ok(new
            {
                message = "Account deactivated successfully."
            });
        }


        [HttpPut("password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeUserPassword(
    [FromBody] ChangePasswordDTO request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized();

            var userBusiness = new Business_Layer.clsUser(_config);

            var user = await userBusiness.GetUserByUserIdAsync(currentUserId);

            if (user == null)
                return NotFound("User not found.");

            if (!BCrypt.Net.BCrypt.Verify(request.oldPassword, user.Password))
                return Unauthorized("your old password is wrong");

            if(request.newPassword == request.oldPassword)
                return BadRequest("you pick the same password twice");


            bool? result = await userBusiness.ChangeUserPassword(currentUserId , request.newPassword);

            if (result == null)
                return NotFound("User not found.");

            if (result == false)
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "password is not changed");

            return Ok(new
            {
                message = "password is changed successfully."
            });
        }

        [HttpPut("credentials")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeUserCredentials(
    [FromBody] UpdateUserDTO request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized();

            var userBusiness = new Business_Layer.clsUser(_config);


            bool? result = await userBusiness.ChangeUserCredentialsAsync(
                currentUserId, request);

            if (result == null)
                return NotFound("User creadials have issues or user not found");

            if (result == false)
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "user creadentials is not changed");

            return Ok(new
            {
                message = "user creadentials has changed successfully."
            });
        }

        [HttpGet("navbar-state")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNavbarState()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized();

            var data = await new Business_Layer.clsNotifications(_config).GetNavbarStateAsync(currentUserId);
            if (data == null)
                return NotFound("user is not found");

            return Ok(data);
        }
    }

}



