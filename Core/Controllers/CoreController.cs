using CoreAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;

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
            var contact = await _coreService.GetContact(Id, _contextAccessor);
            return Ok(contact);
        }

        [HttpGet]
        [Route("company/{Id}")]
        public async Task<ActionResult<string>> GetCompany(Guid Id)
        {
            var contact = await _coreService.GetContact(Id, _contextAccessor);
            return Ok(contact);
        }

        [HttpGet]
        [Route("properties/{Id}")]
        public async Task<ActionResult<string>> GetProperty(Guid Id)
        {
            var property = await _coreService.GetProperty(Id, _contextAccessor);
            return Ok(property);
        }

        [HttpPost("properties")]
        public async Task<ActionResult<string>> CreateProperty([FromBody] string requestBody)
        {
            var property = await _coreService.CreateProperty(requestBody, _contextAccessor);

            return Ok(property);
        }

        [HttpPatch]
        [Route("properties/{Id}")]
        public async Task<ActionResult<string>> UpdateProperty(Guid Id, [FromBody] string requestBody)
        {
            // TODO: Update property, if name changed then update Archive display_name as well

            return NoContent();
        }
    }
}