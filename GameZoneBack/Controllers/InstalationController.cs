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
    public class InstalationController : ControllerBase
    {
        private readonly IConfiguration _config;

        public InstalationController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateNewInstalationColumn([FromBody] PostInstalationGameDTO postInstalationGameDTO)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int currentUserId))
                return Unauthorized();

            var userService = new Business_Layer.clsUser(_config);
            var installationService = new Business_Layer.clsInstalationGames(_config);
            var purchasedGamesService = new Business_Layer.clsPurshasedGames(_config);

            var checkForUser = await userService.CheckIfUserExist(currentUserId);
            if (!checkForUser)
                return NotFound("User not found.");

            var checkIfGameIsPurchased =
                await purchasedGamesService.IsGamePurshased(postInstalationGameDTO.gameId, currentUserId);

            if (!checkIfGameIsPurchased)
                return BadRequest("Game not purchased.");

            var data = await installationService.CreateNewInstalationColumn(postInstalationGameDTO, currentUserId);

            if (data == null)
                return BadRequest("Error creating installation.");

            return Ok(data);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateGameInstallation([FromBody] UpdateInstalationGameDTO request)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int currentUserId))
                return Unauthorized();

            var userService = new Business_Layer.clsUser(_config);
            var installationService = new Business_Layer.clsInstalationGames(_config);

            var checkForUser = await userService.CheckIfUserExist(currentUserId);
            if (!checkForUser)
                return NotFound("User not found.");

            var checkIfInstallationExists =
                await installationService.CheckIfInstallationGameExist(request.InstallationId, currentUserId);

            if (!checkIfInstallationExists)
                return NotFound("Installation not found.");

            var updated = await installationService.UpdateInstalationGame(request);

            if (!updated)
                return BadRequest("Error updating installation.");

            return Ok(updated);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteGameInstallation(int InstallationGameId)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int currentUserId))
                return Unauthorized();

            var userService = new Business_Layer.clsUser(_config);
            var installationService = new Business_Layer.clsInstalationGames(_config);

            var checkForUser = await userService.CheckIfUserExist(currentUserId);
            if (!checkForUser)
                return NotFound("User not found.");

            var checkIfInstallationExists =
                await installationService.CheckIfInstallationGameExist(InstallationGameId, currentUserId);

            if (!checkIfInstallationExists)
                return NotFound("Installation not found.");

            var deleted = await installationService.DeleteInstallationGame(InstallationGameId);

            if (!deleted)
                return BadRequest("Error deleting installation.");

            return Ok(deleted);
        }
    }
}
