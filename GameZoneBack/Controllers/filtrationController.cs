using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Data.enums;
using static System.Net.WebRequestMethods;

namespace GameZoneBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class filtrationController : ControllerBase
    {
        private readonly IConfiguration _config;

        public filtrationController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("category/{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetGamesByCategoryFiltrations(int Id)
        {
            if (Id <= 0)
                return BadRequest("Id must be valid");
            var check = await new Business_Layer.clsCategories(_config).CheckIfCategorieExist(Id);
            if(check == false)
                return NotFound("not found a categorie with this id");

            var data = await new Business_Layer.clsGames(_config).GetGamesByCategoryFiltrations(Id);
            return Ok(data);
        }


        [HttpGet("publisher/{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetGamesByPublisherFiltrations(int Id)
        {
            if (Id <= 0)
                return BadRequest("Id must be valid");
            var check = await new Business_Layer.clsCompanies(_config).CheckIfCompanyExist(Id);
            if (check == false)
                return NotFound("not found a publisher with this id");

            var data = await new Business_Layer.clsGames(_config).GetGamesByPublisherFiltrations(Id);
            return Ok(data);
        }

        [HttpGet("user/section")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetGamesBySectionFiltrations(UserStoreSectionType SectionType)
        {
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var data = await new Business_Layer.clsGames(_config).GetGamesBySectionFiltrations(SectionType,userId);
            return Ok(data);
        }

        [HttpGet("user/wishlist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetWishlistGamesFiltrations()
        {
 

            int? currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var checkForUser = await new Business_Layer.clsUser(_config).CheckIfUserExist(currentUserId??0);
            if (checkForUser == false)
                return NotFound("not found a user with this id");
            
            var data = await new Business_Layer.clsWishlist(_config).GetWishListGamesFiltrations(currentUserId);
            return Ok(data);
        }
    }
}
