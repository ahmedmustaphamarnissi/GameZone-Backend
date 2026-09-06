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
    public class ReviewsController : ControllerBase
    {


        private readonly IConfiguration _config;

        public ReviewsController(IConfiguration config)
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
        public async Task<IActionResult> AddNewReview([FromBody] ReviewPostDTO review)
        {
            
            var checkForGame = await new Business_Layer.clsGames(_config).CheckIfGameExist(review.gameId);
            if (checkForGame == false)
                return NotFound("not found a game with this id");
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(review.userId);
            if (checkForUser == false)
                return NotFound("not found a user with this id");
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != review.userId)
                return Forbid("you can't review with another user account");
            var res = await new Business_Layer.clsReview(_config).AddReviewAsync(review);
            return Ok(res);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviews([FromQuery] ReviewsRequest review)
        {

            var checkForGame = await new Business_Layer.clsGames(_config).CheckIfGameExist(review.gameId);
            if (checkForGame == false)
                return NotFound("not found a game with this id");
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId == null)
                return Forbid("access denied");
            var res = await new Business_Layer.clsReview(_config).GetReviewAsync(review, userId);
            return Ok(res);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteReview(int ReviewId)
        {

            try
            {
                int? reviewUserId =
                    await new Business_Layer.clsReview(_config)
                        .GetUserIdByReviewIdAsync(ReviewId);

                if (reviewUserId == null)
                    return NotFound("There is no review with this id.");

                int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (userId != reviewUserId && reviewUserId!=null)
                    return StatusCode(StatusCodes.Status403Forbidden, "You can't delete another user's review.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
           
            try
            {
                bool res = await new Business_Layer.clsReview(_config).DeleteReviewByReviewIdAsync(ReviewId);

                if (!res)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Review is not deleted");

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateReview([FromBody]ReviewsPutDTO newReview)
        {

            try
            {
                int? reviewUserId =
                    await new Business_Layer.clsReview(_config)
                        .GetUserIdByReviewIdAsync(newReview.reviewId);

                if (reviewUserId == null)
                    return NotFound("There is no review with this id.");

                int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (userId != reviewUserId && reviewUserId != null)
                    return StatusCode(StatusCodes.Status403Forbidden, "You can't update another user's review.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

            try
            {
                var res = await new Business_Layer.clsReview(_config).UpdateReviewAsync(newReview);

                if (res == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Review is not updated");

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
