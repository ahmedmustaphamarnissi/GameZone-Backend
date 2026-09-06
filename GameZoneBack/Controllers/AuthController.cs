using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Business_Layer;
using GameZoneBack.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.DTO.Auth;

namespace GameZoneBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.EmailOrUsername) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Missing required fields.");

            // Try by email first, then username
            var person = await new clsPerson(_config).GetPersonByEmailAsync(request.EmailOrUsername);

            string hashedPassword;
            Claim[] claims;

            if (person != null)
            {
                // ✅ Verify password
                if (!BCrypt.Net.BCrypt.Verify(request.Password, person.User?.Password ?? person.Employee?.Password))
                    return Unauthorized("Invalid Credentials");

                if (person.Grade.GradeName == "Customer")
                {
                    if (person.User.Status.StatusName != "Active")
                        return Unauthorized(person.User.Status.Description);

                    claims = new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, person.User.UserId.ToString()),
                new Claim(ClaimTypes.Email, person.Email),
                new Claim(ClaimTypes.Role, person.Grade.GradeName)
            };
                }
                else
                {
                    if (person.Employee.Status.StatusName != "Active")
                        return Unauthorized(person.Employee.Status.Description);

                    claims = new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, person.Employee.EmployeeId.ToString()),
                new Claim(ClaimTypes.Email, person.Email),
                new Claim(ClaimTypes.Role, person.Grade.GradeName)
            };
                }
            }
            else
            {
                // Fallback to username
                var user = await new clsUser(_config).GetUserByUserNameAsync(request.EmailOrUsername);
                if (user == null)
                    return NotFound("User not found."); // ✅ same message for both cases
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                    return Unauthorized("Invalid Credentials"); // ✅ same message for both cases

                claims = new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Person.Email),
            new Claim(ClaimTypes.Role, user.Person.Grade.GradeName)
        };
            }

            var secret = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("JWT secret key is missing.");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secret));

            var token = new JwtSecurityToken(
                issuer: "GameZoneApi",
                audience: "GameZoneApiUsers",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var PersonId = await new clsPerson(_config).GetPersonIdByEmailAsync(request.Email);
            if(PersonId != null)
                return BadRequest("the email is already used in another account");

            var UserId = await new clsUser(_config).GetUserIdByEmailAsync(request.Email);

            if (UserId != null) return BadRequest("the user name is already used in another account");

            var CountryId = await new clsCountry(_config).GetCountryIdByCountryCodeAsync(request.CountryCode);

            
            var user = await new clsUser(_config).AddNewUserAsync(request, Convert.ToInt32(CountryId));
            if (user != null)
            {
                var claims = new[]
                    {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Person.Email),
            new Claim(ClaimTypes.Role, user.Person.Grade.GradeName)
                };
                var secret = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

                if (string.IsNullOrWhiteSpace(secret))
                    throw new InvalidOperationException("JWT secret key is missing.");

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secret));

                var token = new JwtSecurityToken(
                    issuer: "GameZoneApi",
                    audience: "GameZoneApiUsers",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            else
            {
                return BadRequest("User is not Created");
            }

        }
    }
}
