using CoreAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoreAPI.Controllers
{
    [ApiController]
    [Route("core")]
    public class CoreController : ControllerBase
    {
        private readonly ICoreService _coreService;
        private readonly ILogger<CoreController> _logger;

        public CoreController(ICoreService coreService, ILogger<CoreController> logger)
        {
            _coreService = coreService;
            _logger = logger;
        }

        #region GET

        [HttpGet]
        [Route("contacts/{Id}")]
        public async Task<ActionResult<string>> GetContact(Guid Id)
        {
            var contact = await _coreService.GetContact(Id);
            return Ok(contact);
        }

        [HttpGet]
        [Route("companies/{Id}")]
        public async Task<ActionResult<string>> GetCompany(Guid Id)
        {
            var contact = await _coreService.GetCompany(Id);
            return Ok(contact);
        }

        [HttpGet]
        [Route("properties/{Id}")]
        public async Task<ActionResult<string>> GetProperty(Guid Id)
        {
            var property = await _coreService.GetProperty(Id);
            return Ok(property);
        }


        [HttpGet]
        [Route("contacts")]
        public async Task<ActionResult<string>> GetContacts(bool includeDeleted = false)
        {
            var contact = await _coreService.GetContacts(includeDeleted);
            return Ok(contact);
        }

        [HttpGet]
        [Route("companies")]
        public async Task<ActionResult<string>> GetCompanies(bool includeDeleted = false)
        {
            var contact = await _coreService.GetCompanies(includeDeleted);
            return Ok(contact);
        }

        [HttpGet]
        [Route("properties")]
        public async Task<ActionResult<string>> GetProperties(bool includeDeleted = false)
        {
            var property = await _coreService.GetProperties(includeDeleted);
            return Ok(property);
        }

        #endregion
        #region CREATE

        [HttpPost("properties")]
        public async Task<ActionResult<string>> CreateProperty([FromBody] string value)
        {

            _logger.LogDebug($"CORE - CreateProperty - RequestBody:\n{value}");

            var property = await _coreService.CreateProperty(value);

            return Ok(property);
        }

        [HttpPost("companies")]
        public async Task<ActionResult<string>> CreateCompany([FromBody] string value)
        {
            var company = await _coreService.CreateCompany(value);

            return Ok(company);
        }

        [HttpPost("contacts")]
        public async Task<ActionResult<string>> CreateContact([FromBody] string value)
        {
            var contacts = await _coreService.CreateContact(value);

            return Ok(contacts);
        }
        #endregion
        #region UPDATE

        [HttpPatch]
        [Route("properties/{Id}")]
        public async Task<ActionResult<string>> UpdateProperty(Guid Id, [FromBody] string value)
        {
            // TODO: Update property, if name changed then update Archive display_name as well

            return NoContent();
        }

        [HttpPatch]
        [Route("contacts/{Id}")]
        public async Task<ActionResult<string>> UpdateContact(Guid Id, [FromBody] string value)
        {
            // TODO: Update contact

            return NoContent();
        }

        [HttpPatch]
        [Route("companies/{Id}")]
        public async Task<ActionResult<string>> UpdateCompany(Guid Id, [FromBody] string value)
        {
            // TODO: Update company

            return NoContent();
        }

        #endregion

        #region DELETE

        [HttpDelete]
        [Route("properties/{Id}")]
        public async Task<IActionResult> DeleteProperty(Guid Id)
        {
            await _coreService.DeleteProperty(Id);
            return NoContent();
        }

        [HttpDelete]
        [Route("companies/{Id}")]
        public async Task<IActionResult> DeleteCompany(Guid Id)
        {
            await _coreService.DeleteCompany(Id);
            return NoContent();
        }

        [HttpDelete]
        [Route("contacts/{Id}")]
        public async Task<IActionResult> DeleteContact(Guid Id)
        {
            await _coreService.DeleteContact(Id);
            return NoContent();
        }

        #endregion
    }
}