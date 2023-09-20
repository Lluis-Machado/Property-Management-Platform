using CoreAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoreAPI.Controllers
{
    [ApiController]
    [Route("core")]
    public class CoreController : ControllerBase
    {
        private readonly ICoreService _coreService;
        private readonly IHttpContextAccessor _contextAccessor;

        public CoreController(ICoreService coreService, IHttpContextAccessor contextAccessor)
        {
            _coreService = coreService;
            _contextAccessor = contextAccessor;
        }

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

        [HttpPost("properties")]
        public async Task<ActionResult<string>> CreateProperty([FromBody] string requestBody)
        {
            var property = await _coreService.CreateProperty(requestBody);

            return Ok(property);
        }

        [HttpPost("companies")]
        public async Task<ActionResult<string>> CreateCompany([FromBody] string requestBody)
        {
            var company = await _coreService.CreateCompany(requestBody);

            return Ok(company);
        }

        [HttpPost("contacts")]
        public async Task<ActionResult<string>> CreateContact([FromBody] string requestBody)
        {
            var contacts = await _coreService.CreateContact(requestBody);

            return Ok(contacts);
        }

        [HttpPatch]
        [Route("properties/{Id}")]
        public async Task<ActionResult<string>> UpdateProperty(Guid Id, [FromBody] string requestBody)
        {
            // TODO: Update property, if name changed then update Archive display_name as well

            return NoContent();
        }

        [HttpPatch]
        [Route("contacts/{Id}")]
        public async Task<ActionResult<string>> UpdateContact(Guid Id, [FromBody] string requestBody)
        {
            // TODO: Update contact

            return NoContent();
        }

        [HttpPatch]
        [Route("companies/{Id}")]
        public async Task<ActionResult<string>> UpdateCompanies(Guid Id, [FromBody] string requestBody)
        {
            // TODO: Update company

            return NoContent();
        }
    }
}