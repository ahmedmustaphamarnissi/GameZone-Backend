using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameZoneBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentMethodsController : ControllerBase
    {

        private readonly IConfiguration _config;

        public PaymentMethodsController(IConfiguration config)
        {
            _config = config;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserCards()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized();

            var data = await new Business_Layer.clsPaymentMethods(_config).GetUserCardsAsync(currentUserId);
            if(data == null)
                return NotFound("User not found.");
            //cards can be empty (user don't have any card)
            return Ok(data);
        }


        [HttpPut("default-card")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeDefaultCard(int cardId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return Unauthorized();

            bool? result = await new Business_Layer.clsPaymentMethods(_config)
                .ChangeDefaultCardAsync(cardId, currentUserId);

            if (result == null)
                return NotFound("User or card not found.");

            if (result == false)
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Default card was not changed.");

            return Ok(new
            {
                message = "Default card changed successfully."
            });
        }

        
    }
}
