using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.DTO.Filter;

namespace GameZoneBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GamesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public GamesController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("NewGames")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetGamesNewReleases(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("page size and page number must be > 0");

            var data = await new Business_Layer.clsGames(_config).GetGamesNewReleasesAsync(pageNumber, pageSize);
            if (data.Count == 0 || data == null)
                return NotFound("not found new games");
            return Ok(data);
        }
        [HttpGet("FiltrationNewGames")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetGamesNewReleasesWithFiltration([FromQuery]NewReleasesFilterDTO filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("page size and page number must be > 0");



            var data = await new Business_Layer.clsGames(_config).getNewReleasesWithFilterAsync(filter);
            if (data.Count == 0 || data == null)
                return NotFound("not found new games");
            return Ok(data);
        }

        [HttpGet("trending")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetTrendingGames()
        {
            var data = await new Business_Layer.clsGames(_config).getTrendingGamesAsync();
            if (data.Count == 0 || data == null)
                return NotFound("not found any game trending now");
            return 
                Ok(data);
        }
        [HttpGet("topSellers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopSellersGames(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("page size and page number must be > 0");

            var data = await new Business_Layer.clsGames(_config).getTopSellersGamesAsync(pageNumber, pageSize);
            if (data.Count == 0 || data == null)
                return NotFound("not found new games");
            return Ok(data);
        }

        [HttpGet("topSellersFiltration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopSellersGamesWithFiltration([FromQuery] TopSellersFilter filter )
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("page size and page number must be > 0");

            var data = await new Business_Layer.clsGames(_config).getTopSellersGamesWithFiltrationAsync(filter);
            if (data.Count == 0 || data == null)
                return NotFound("not found new games");
            return Ok(data);
        }
        [HttpGet("public/store")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetpublicStoreGames()
        {


            var data = await new Business_Layer.clsGames(_config).GetPublicStoreGamesAsync();
            if (data == null)
                return NotFound("not found public store");
            return Ok(data);
        }

        [HttpGet("user/store")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<IActionResult> GetUserStoreGames()
        {
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var data = await new Business_Layer.clsGames(_config).GetUserStoreGamesAsync(userId);
            if (data == null)
                return NotFound("not found user store");
            return Ok(data);
        }

        [HttpGet("public/storeFiltration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetpublicStoreGamesByFiltration([FromQuery] PublicStoreFiltrationRequest filtration)
        {


            var data = await new Business_Layer.clsGames(_config).GetPublicStoreGamesWithFiltrationAsync(filtration);
            if (data == null)
                return NotFound("not found public store with filtrations");
            return Ok(data);
        }

        [HttpGet("user/storeFiltration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUserStoreGamesByFiltration([FromQuery] PublicStoreFiltrationRequest filtration)
        {

            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var data = await new Business_Layer.clsGames(_config).GetUserStoreGamesWithFiltrationAsync(userId,filtration);
            if (data == null)
                return NotFound("not found user store with filtrations");
            return Ok(data);
        }

        [HttpGet("public/store/search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetpublicStoreSearchGames(string search, int pageSize = 5)
        {


            var data = await new Business_Layer.clsGames(_config).GetpublicStoreSearchGames(search,pageSize);
            if (data == null)
                return NotFound("not found new games");
            return Ok(data);
        }

        [HttpGet("CommingSoon")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommingSoonGames(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("page size and page number must be > 0");

            var data = await new Business_Layer.clsGames(_config).GetCommingSoonGamesAsync(pageNumber, pageSize);
            if (data.Count == 0 || data == null)
                return NotFound("not found comming soon games");
            return Ok(data);
        }
        [HttpGet("FiltrationCommingSoon")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommingSoonGamesWithFiltration([FromQuery] NewReleasesFilterDTO filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("page size and page number must be > 0");



            var data = await new Business_Layer.clsGames(_config).getCommingSoonGamesWithFilterAsync(filter);
            if (data.Count == 0 || data == null)
                return NotFound("not found comming soon games");
            return Ok(data);
        }

        [HttpGet("free")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetFreeGamesWithFiltration([FromQuery] FreeGamesFilterDTO filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("page size and page number must be > 0");



            var data = await new Business_Layer.clsGames(_config).getFreeGamesWithFilterAsync(filter);
            if (data.Count == 0 || data == null)
                return NotFound("not found comming soon games");
            return Ok(data);
        }
        [HttpGet("specialOffers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetSpecialOffersGamesWithFiltrationAsync([FromQuery] SpecialOffersFilter filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("page size and page number must be > 0");



            var data = await new Business_Layer.clsGames(_config).GetSpecialOffersGamesWithFiltrationAsync(filter);
            if (data.Count == 0 || data == null)
                return NotFound("not found free games");
            return Ok(data);
        }
        [HttpGet("category/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetGamesByCategorieWithFiltrationAsync([FromQuery] GamesByCategoriesFiltration filter )
        {
            if (filter.pageNumber <= 0 || filter.pageSize <= 0)
                return BadRequest("page size and page number must be > 0");

            var check = await new Business_Layer.clsCategories(_config).CheckIfCategorieExist(filter.categoryId);
            if (check == false)
                return NotFound("not found a category with this id");

            var data = await new Business_Layer.clsGames(_config).GetGamesByCategoryIdWithFiltrationAsync(filter);
            if (data.Count == 0 || data == null)
                return NotFound("not found games in this category");
            return Ok(data);
        }
        [HttpGet("publisher/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetGamesByPublisherWithFiltrationAsync([FromQuery] GamesByPublisherFiltration filter)
        {
            if (filter.pageNumber <= 0 || filter.pageSize <= 0)
                return BadRequest("page size and page number must be > 0");

            var check = await new Business_Layer.clsCompanies(_config).CheckIfCompanyExist(filter.publisherId);
            if (check == false)
                return NotFound("not found a publisher with this id");

            var data = await new Business_Layer.clsGames(_config).GetGamesByPublisherIdWithFiltrationAsync(filter);
            if (data.Count == 0 || data == null)
                return NotFound("not found games created by this publisher");
            return Ok(data);
        }

        [HttpGet("hover/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetGamesByPublisherWithFiltrationAsync([FromRoute(Name = "id")] int Id)
        {

            var check = await new Business_Layer.clsGames(_config).CheckIfGameExist(Id);
            if (check == false)
                return NotFound("not found a game with this id");

            var data = await new Business_Layer.clsGames(_config).GetHoverOnGameDataAsync(Id);
            if (data == null)
                return NotFound("not game hover data");
            return Ok(data);
        }

        [HttpGet("details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetGamesDetailsById([FromRoute(Name = "id")] int Id)
        {

            var check = await new Business_Layer.clsGames(_config).CheckIfGameExist(Id);
            if (check == false)
                return NotFound("not found a game with this id");
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var data = await new Business_Layer.clsGames(_config).GetGameDetailsAsync(Id, userId);
            if (data == null)
                return NotFound("not game hover data");
            return Ok(data);
        }
        [HttpGet("user/section")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetGamesBySectionWithFiltrationAsync([FromQuery] GamesBySectionFiltrations filter)
        {
            if (filter.pageNumber <= 0 || filter.pageSize <= 0)
                return BadRequest("page size and page number must be > 0");
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var data = await new Business_Layer.clsGames(_config).GetGamesBySectionWithFiltrationAsync(filter , userId);
            if (data.Count == 0 || data == null)
                return NotFound("not found games in this section");
            return Ok(data);
        }

    }
}

