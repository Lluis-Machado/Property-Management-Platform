using CompaniesAPI.Validators;
using CompanyAPI.Dtos;
using CompanyAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
//using Microsoft.AspNetCore.Authorization;

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

            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            return await _companyService.CreateAsync(companyDto, userName);
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

            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            return await _companyService.UpdateAsync(companyId, companyDto, userName!);
        }

        [HttpPatch]
        [Route("{companyId}/{archiveId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateArchiveIdAsync(Guid companyId, Guid archiveId)
        {
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            return await _companyService.UpdateCompanyArchiveIdAsync(companyId, archiveId, userName);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAsync(bool includeDeleted = false)
        {
            return await _companyService.GetAsync(includeDeleted);
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

        [HttpDelete]
        [Route("{companyId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid companyId)
        {
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            return await _companyService.DeleteAsync(companyId, userName);
        }

        [HttpPatch]
        [Route("{companyId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid companyId)
        {
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            return await _companyService.UndeleteAsync(companyId, userName);
        }
    }
}
