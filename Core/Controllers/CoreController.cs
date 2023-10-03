using CoreAPI.Services;
using CoreAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CoreAPI.Controllers
{
    [ApiController]
    // TODO: Remove duplicated route indication - Api Gw already adds service name
    // Must be done throughout all services!
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

        // TODO: Return object on update or just NoContent()?

        [HttpPatch]
        [Route("properties/{Id}")]
        public async Task<ActionResult<string>> UpdateProperty(Guid Id, [FromBody] string value)
        {
            return Ok(await _coreService.UpdateProperty(Id, value));
        }

        [HttpPatch]
        [Route("contacts/{Id}")]
        public async Task<ActionResult<string>> UpdateContact(Guid Id, [FromBody] string value)
        {
            return Ok(await _coreService.UpdateContact(Id, value));
        }

        [HttpPatch]
        [Route("companies/{Id}")]
        public async Task<ActionResult<string>> UpdateCompany(Guid Id, [FromBody] string value)
        {
            return Ok(await _coreService.UpdateCompany(Id, value));
        }

        #endregion

        #region DELETE

        [HttpDelete]
        [Route("properties/{Id}")]
        public async Task<IActionResult> DeleteProperty(Guid Id)
        {
            try
            {
                await _coreService.DeleteProperty(Id);
            }
            catch (OwnershipExistsException ex)
            {
                return UnprocessableEntity(ex.Message);
            }
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