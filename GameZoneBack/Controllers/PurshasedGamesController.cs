using System.ComponentModel.DataAnnotations;
using System.Net;
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
    public class PurshasedGamesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public PurshasedGamesController(IConfiguration config)
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
        public async Task<IActionResult> GetPurshasedGames()
        {
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(currentUserId ?? -1);
            if (checkForUser == false)
                return NotFound("not found a user with this id");

            var Data = await new Business_Layer.clsPurshasedGames(_config).GetPurchasedGames(currentUserId);
            if (Data == null)
                return BadRequest("Purchased games not found");
            return Ok(Data);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetPurshasedGameDetails(int id)
        {
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(currentUserId ?? -1);
            if (checkForUser == false)
                return NotFound("not found a user with this id");
            var checkForGame = await new Business_Layer.clsPurshasedGames(_config).IsGamePurshased(id, currentUserId ?? -1);
            if (checkForGame == false)
                return NotFound("not found a purshased game with this id");

            var Data = await new Business_Layer.clsPurshasedGames(_config).GetPurchasedGameDetailsAsync(id, currentUserId);
            if (Data == null)
                return BadRequest("Purchased game details not found");
            return Ok(Data);
        }

        [HttpGet("history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetPurshasedHistory([FromQuery] PurshasedHistoryRequest request)
        {
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(currentUserId ?? -1);
            if (checkForUser == false)
                return NotFound("not found a user with this id");

            var Data = await new Business_Layer.clsPurshasedGames(_config).GetPurshasedHistoryAsync(request, currentUserId);
            if (Data == null)
                return BadRequest("Purchased games history is not found");
            return Ok(Data);
        }

        [HttpPut("favorite/add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddFavoriteGame(int GameId)
        {
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            bool checkIfGamePurchased = await new Business_Layer.clsPurshasedGames(_config).
                IsGamePurshased(GameId, currentUserId ?? -1);

            if (!checkIfGamePurchased)
                return NotFound("this game is not in your purshased List");
            var Data = await new Business_Layer.clsPurshasedGames(_config).
                PerformActionOnFavoriteGame(currentUserId ?? -1, GameId, true);
            if (!Data)
                return BadRequest("adding game to favorite list is failed");
            return Ok(Data);
        }

        [HttpPut("favorite/remove")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemoveFavoriteGame(int GameId)
        {
            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            bool checkIfGamePurchased = await new Business_Layer.clsPurshasedGames(_config).
                IsGamePurshased(GameId, currentUserId ?? -1);

            if (!checkIfGamePurchased)
                return NotFound("this game is not in your purshased List");
            var Data = await new Business_Layer.clsPurshasedGames(_config).
                PerformActionOnFavoriteGame(currentUserId ?? -1, GameId, false);
            if (!Data)
                return BadRequest("removing game to favorite list is failed");
            return Ok(Data);
        }

        [HttpPost("purchase")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PurchaseAGame(
    [FromBody] PurchaseGameDTO request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized();

            var purchasedGamesBusiness =
                new Business_Layer.clsPurshasedGames(_config);

            var paymentMethodsBusiness =
                new Business_Layer.clsPaymentMethods(_config);

            // Check if the user already owns the game
            bool checkIfGamePurchased = await purchasedGamesBusiness
                .IsGamePurshased(request.gameId, currentUserId);

            if (checkIfGamePurchased)
                return Conflict("This game is already in your purchased list.");

            // Check if the card already exists
            int? cardId = await paymentMethodsBusiness
                .CheckIfCardExist(request);

            // Create the card only if it doesn't exist
            if (!cardId.HasValue)
            {
                cardId = await paymentMethodsBusiness
                    .CreateNewPaymentMethod(currentUserId, request);

                if (!cardId.HasValue)
                    return BadRequest("Unable to add the payment method.");
            }

            // Complete the purchase
            bool? result = await purchasedGamesBusiness
                .PurchaseAGameAsync(
                    currentUserId,
                    request.gameId,
                    cardId.Value
                );

            if (!result.HasValue)
                return NotFound("The game was not found.");

            if (!result.Value)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "The game could not be purchased."
                );
            }

            var user = await purchasedGamesBusiness
                .GetUserPurchaseCredentials(currentUserId, request.gameId);

            if (user == null)
            {
                return Ok(new
                {
                    message = "Game purchased successfully, but purchase details could not be retrieved for the confirmation email."
                });
            }

            var emailService = new Business_Layer.clsEmailService(_config);

            var userName = WebUtility.HtmlEncode(user.UserName);
            var gameName = WebUtility.HtmlEncode(user.GameName);

            var priceText = user.PurchasePrice == 0
                ? "Free"
                : $"${user.PurchasePrice:F2}";

            bool emailSent = await emailService.SendEmailAsync(
                user.Email,
                $"Your GameZone order: {user.GameName}",
                $"""
<!DOCTYPE html>
<html>
<body style="font-family: Arial, sans-serif; color: #333; line-height: 1.6;">

    <h2>Thank you for your purchase!</h2>

    <p>Hello <strong>{userName}</strong>,</p>

    <p>
        Your order on <strong>GameZone</strong> has been completed successfully.
        Here is a summary:
    </p>

    <div style="padding: 15px; border: 1px solid #ddd; border-radius: 8px;">
        <h3 style="margin-top: 0;">Order Summary</h3>

        <p><strong>Game:</strong> {gameName}</p>

        <p><strong>Amount Paid:</strong> {priceText}</p>
    </div>

    <p>The game has been added to your GameZone library.</p>

    <p>We hope you enjoy playing!</p>

    <br />

    <p>
        Best regards,<br />
        <strong>The GameZone Team</strong>
    </p>

</body>
</html>
"""
            );

            if (!emailSent)
            {
                return Ok(new
                {
                    message = "Game purchased successfully, but the confirmation email could not be sent."
                });
            }

            return Ok(new
            {
                message = "Game purchased successfully and confirmation email was sent."
            });
        }

        [HttpPost("gift")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> SendAGiftForFriend(
    [FromBody] PurchaseGameDTO request,
    [Required][Range(1, int.MaxValue, ErrorMessage = "recever id must be valid")] int receverId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized();

            var purchasedGamesBusiness =
                new Business_Layer.clsPurshasedGames(_config);

            var paymentMethodsBusiness =
                new Business_Layer.clsPaymentMethods(_config);

            bool checkForFriendShip = await new Business_Layer.clsFriendRequests(_config)
                .CheckIfIdFromFriendsList(currentUserId, receverId);

            if (!checkForFriendShip)
                return BadRequest("this user is not in your friend list");

            // Check if the user already owns the game
            bool checkIfGamePurchased = await purchasedGamesBusiness
                .IsGamePurshased(request.gameId, receverId);

            if (checkIfGamePurchased)
                return Conflict("your friend have already this game in his purchased list.");

            // Check if the card already exists
            int? cardId = await paymentMethodsBusiness
                .CheckIfCardExist(request);

            // Create the card only if it doesn't exist
            if (!cardId.HasValue)
            {
                cardId = await paymentMethodsBusiness
                    .CreateNewPaymentMethod(currentUserId, request);

                if (!cardId.HasValue)
                    return BadRequest("Unable to add the payment method.");
            }

            // Complete the purchase
            bool? result = await purchasedGamesBusiness
                .PurchaseAGameAsync(
                    receverId,
                    request.gameId,
                    cardId.Value,
                    true
                );

            if (!result.HasValue)
                return NotFound("The game was not found.");

            if (!result.Value)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "The game could not be purchased."
                );
            }

            var user = await purchasedGamesBusiness
                .GetUserPurchaseCredentials(receverId, request.gameId);

            if (user == null)
            {
                return Ok(new
                {
                    message = "Game purchased successfully, but purchase details could not be retrieved for the confirmation email."
                });
            }

            var emailService = new Business_Layer.clsEmailService(_config);

            var userName = WebUtility.HtmlEncode(user.UserName);
            var gameName = WebUtility.HtmlEncode(user.GameName);

            bool emailSent = await emailService.SendEmailAsync(
                user.Email,
                $"A friend sent you a gift on GameZone: {user.GameName}",
                $"""
<!DOCTYPE html>
<html>
<body style="font-family: Arial, sans-serif; color: #333; line-height: 1.6;">

    <h2>You've received a gift!</h2>

    <p>Hello <strong>{userName}</strong>,</p>

    <p>
        Great news! One of your friends has sent you a gift on
        <strong>GameZone</strong>.
    </p>

    <div style="padding: 15px; border: 1px solid #ddd; border-radius: 8px;">
        <h3 style="margin-top: 0;">Gift Details</h3>

        <p><strong>Game:</strong> {gameName}</p>
    </div>

    <p>The game has been added to your GameZone library. Enjoy!</p>

    <br />

    <p>
        Best regards,<br />
        <strong>The GameZone Team</strong>
    </p>

</body>
</html>
"""
            );

            if (!emailSent)
            {
                return Ok(new
                {
                    message = "Game purchased successfully, but the confirmation email could not be sent."
                });
            }

            return Ok(new
            {
                message = "Game purchased successfully and confirmation email was sent."
            });
        }


    }
}
