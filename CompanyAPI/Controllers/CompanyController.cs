using CompanyAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using CompanyAPI.Dtos;

namespace CompanyAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("companies")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<CompanyDetailedDto>> CreateAsync([FromBody] CreateCompanyDto companyDto)
        {
            // validations
            if (companyDto == null) return new BadRequestObjectResult("Incorrect body format");

            var lastUser = "test";

            return await _companyService.CreateAsync(companyDto, lastUser);
        }

        [HttpPatch]
        [Route("{companyId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<CompanyDetailedDto>> UpdateAsync(Guid companyId, [FromBody] UpdateCompanyDto companyDto)
        {
            // validations
            if (companyDto == null) return new BadRequestObjectResult("Incorrect body format");
            
            var lastUser = "test";

            return await _companyService.UpdateAsync(companyId, companyDto, lastUser);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAsync(bool includeDeteted = false)
        {
            return await _companyService.GetAsync(includeDeteted);
        }

        [HttpGet("{companyId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<CompanyDetailedDto>> GetByIdAsync(Guid companyId)
        {
            var contact = await _companyService.GetByIdAsync(companyId);
            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }
        /*
        [HttpGet("{companyId}/properties")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<CompanyDetailedDto>> GetWithProperties(Guid companyId)
        {
            var company = await _companyService.GetWithProperties(companyId);

            return company;
        }*/

        [HttpDelete]
        [Route("{companyId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid companyId)
        {
            var lastUser = "test";

            return await _companyService.DeleteAsync(companyId, lastUser);
        }

        [HttpPatch]
        [Route("{companyId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid companyId)
        {
            var lastUser = "test";

            return await _companyService.UndeleteAsync(companyId, lastUser);
        }
    }
}
