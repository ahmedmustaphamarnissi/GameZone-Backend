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
    public class CommentsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CommentsController(IConfiguration config)
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
        public async Task<IActionResult> AddNewComment([FromBody] CommentPostDTO comment)
        {

            var checkForGame = await new Business_Layer.clsGames(_config).CheckIfGameExist(comment.gameId);
            if (checkForGame == false)
                return NotFound("not found a game with this id");
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(comment.userId);
            if (checkForUser == false)
                return NotFound("not found a user with this id");
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != comment.userId)
                return StatusCode(StatusCodes.Status403Forbidden, "You can't add a comment for another user.");
            var res = await new Business_Layer.clsComments(_config).AddCommentAsync(comment);
            return Ok(res);
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetComments([FromQuery] CommentsRequest comment)
        {

            var checkForGame = await new Business_Layer.clsGames(_config).CheckIfGameExist(comment.gameId);
            if (checkForGame == false)
                return NotFound("not found a game with this id");
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId == null)
                return StatusCode(StatusCodes.Status403Forbidden, "access denied");
            var res = await new Business_Layer.clsComments(_config).GetCommentsAsync(comment, userId);
            return Ok(res);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteComment(int CommentId)
        {

            try
            {
                int? commentUserId =
                    await new Business_Layer.clsComments(_config)
                        .GetUserIdByCommentIdAsync(CommentId);

                if (commentUserId == null)
                    return NotFound("There is no comment with this id.");

                int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (userId != commentUserId && commentUserId != null)
                    return StatusCode(StatusCodes.Status403Forbidden, "You can't delete another user's comment.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

            try
            {
                bool res = await new Business_Layer.clsComments(_config).DeleteCommentAsync(CommentId);

                if (!res)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Comment is not deleted");

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
        public async Task<IActionResult> UpdateComment([FromBody] CommentPutDTO newComment)
        {

            try
            {
                int? commentUserId =
                    await new Business_Layer.clsComments(_config)
                        .GetUserIdByCommentIdAsync(newComment.commentId);

                if (commentUserId == null)
                    return NotFound("There is no comment with this id.");

                int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (userId != commentUserId && commentUserId != null)
                    return StatusCode(StatusCodes.Status403Forbidden, "You can't update another user's comment.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

            try
            {
                var res = await new Business_Layer.clsComments(_config).UpdateCommentAsync(newComment);

                if (res == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Comment is not updated");

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
