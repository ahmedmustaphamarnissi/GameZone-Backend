using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Business_Layer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GameZoneBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValidationController : ControllerBase
    {

        private readonly IConfiguration _config;

        public ValidationController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("Email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateEmailAsync(string Email)
        {
            string pattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            if (string.IsNullOrEmpty(Email) || !Regex.IsMatch(Email, pattern))
                return BadRequest("Email must be valid");
            var PersonId = await new clsPerson(_config).GetPersonIdByEmailAsync(Email);

            if (PersonId != null)
                return BadRequest("email is already used");
            return Ok();


        }

        [HttpPost("UserName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateUserNameAsync(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
                return BadRequest("User name must be valid");
            var UserId = await new clsUser(_config).GetUserIdByEmailAsync(UserName);

            if (UserId != null)
                return BadRequest("User name is already used");
            return Ok();

        }
    }
}
