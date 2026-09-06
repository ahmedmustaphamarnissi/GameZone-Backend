using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameZoneBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeatureController : ControllerBase
    {

        private readonly IConfiguration _config;

        public FeatureController(IConfiguration config)
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
            var data = await new Business_Layer.clsFeature(_config).GetAllFeaturesAsync();
            if (data.Count == 0)
                return BadRequest("features Not found");
            return Ok(data);
        }
    }
}
