using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using static System.Net.WebRequestMethods;

namespace GameZoneBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IConfiguration _config;

        public WishlistController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("Add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddGameToWishlist(int gameId, int userId)
        {

            var checkForGame = await new Business_Layer.clsGames(_config).CheckIfGameExist(gameId);
            if (checkForGame == false)
                return NotFound("not found a game with this id");
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(userId);
            if (checkForUser == false)
                return NotFound("not found a user with this id");
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (currentUserId != userId)
                return StatusCode(StatusCodes.Status403Forbidden, "You can't control another user's account wishlist.");

            var IsGamePurshased = await new Business_Layer.clsPurshasedGames(_config).IsGamePurshased(gameId, userId);

            if (IsGamePurshased == false)
            {

                var CheckIfGameInWishlist = await new Business_Layer.clsWishlist(_config).IsGameInWishList(gameId, userId);
                if (CheckIfGameInWishlist == false)
                {
                    var res = await new Business_Layer.clsWishlist(_config).AddGameToWishList(gameId, userId);
                    if (res == false)
                        return BadRequest("this game is not added to the wishlist");
                    return Ok();
                }
                else
                {
                    return BadRequest("this game is already in the wishlist");
                }
            }
            else
            {
                return BadRequest("this game is already purshased");
            }
        }

        [HttpDelete("Remove")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoveGameFromWishlist(int gameId, int userId)
        {

            var checkForGame = await new Business_Layer.clsGames(_config).CheckIfGameExist(gameId);
            if (checkForGame == false)
                return NotFound("not found a game with this id");
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(userId);
            if (checkForUser == false)
                return NotFound("not found a user with this id");
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (currentUserId != userId)
                return StatusCode(StatusCodes.Status403Forbidden, "You can't control another user's account wishlist.");

            var CheckIfGameInWishlist = await new Business_Layer.clsWishlist(_config).IsGameInWishList(gameId, userId);
            if (CheckIfGameInWishlist == true)
            {
                var res = await new Business_Layer.clsWishlist(_config).RemoveGameFromWishList(gameId, userId);
                if (res == false)
                    return BadRequest("this game is not removed from the wishlist");
                return Ok();
            }
            else
            {
                return BadRequest("this game is not in the wishlist");
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetWishListGames([FromQuery] WishListRequest request)
        {
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(currentUserId??-1);
            if (checkForUser == false)
                return NotFound("not found a user with this id");

            var Data = await new Business_Layer.clsWishlist(_config).GetWishListGamesAsync(request, currentUserId);
            if(Data == null) 
                return BadRequest("Wishlist is not found");
            return Ok(Data);
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetWishListGamesBySearch([FromQuery] WishListBySearchRequest request)

        {
            if (request.pageNumber <= 0 || request.pageSize <= 0)
                return BadRequest("page size and page number must be > 0");

            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(currentUserId ?? -1);
            if (checkForUser == false)
                return NotFound("not found a user with this id");

            var Data = await new Business_Layer.clsWishlist(_config).GetWishListGamesBySearchAsync(request, currentUserId);
            if (Data == null)
                return BadRequest("Wishlist games with this search text are not found");
            return Ok(Data);
        }
    }
}
