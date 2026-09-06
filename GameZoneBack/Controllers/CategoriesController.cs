using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;

namespace GameZoneBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CategoriesController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]

        public async Task<IActionResult> GetCategoriesAsync()
        {
            var data = await new Business_Layer.clsCategories(_config).GetCategoriesAsync();
            if(data.Count == 0)
                return BadRequest("categories Not found");
            return Ok(data);
        }
    }
}
