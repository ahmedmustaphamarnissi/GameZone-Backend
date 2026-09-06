using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Data.enums.Sorts;

namespace GameZoneBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompaniesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CompaniesController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]

        public async Task<IActionResult> GetCategoriesAsync(int pageNumber, int pageSize, PublishersOrderBy order)
        {
            var data = await new Business_Layer.clsCompanies(_config).getCompaniesAsync(pageNumber,pageSize, order);
            if (data.Count == 0)
                return BadRequest("companies Not found");
            return Ok(data);
        }
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]

        public async Task<IActionResult> GetCategoriesAsync(string search, int pageSize = 12 )
        {
            var data = await new Business_Layer.clsCompanies(_config).getCompaniesBySearchAsync(pageSize, search);
            if (data.Count == 0)
                return BadRequest("companies Not found");
            return Ok(data);
        }


        [HttpGet("details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetGamesByPublisherWithFiltrationAsync([FromRoute(Name = "id")] int Id)
        {

            var check = await new Business_Layer.clsCompanies(_config).CheckIfCompanyExist(Id);
            if (check == false)
                return NotFound("not found a company with this id");

            var data = await new Business_Layer.clsCompanies(_config).getCompanyDetailsAsync(Id);
            if (data == null)
                return NotFound("company details are not found");
            return Ok(data);
        }
    }
}
