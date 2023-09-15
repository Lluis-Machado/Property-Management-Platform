using AccountingAPI.DTOs;
using AccountingAPI.Services;
using AccountingAPI.Validators;
using AuthorizeAPI;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountingAPI.Controllers
{
    [Authorize]
    public class TenantsController : Controller
    {
        private readonly ITenantService _tenantService;
        private readonly ILogger<TenantsController> _logger;

        public TenantsController(ITenantService tenantService, ILogger<TenantsController> logger)
        {
            _tenantService = tenantService;
            _logger = logger;
        }

        // POST: Create tenant
        [HttpPost]
        [Route("tenants")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<TenantDTO>> CreateTenantAsync([FromBody] CreateTenantDTO createTenantDTO)
        {
            // request validations
            if (createTenantDTO is null) return BadRequest("Incorrect body format");

            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            TenantDTO tenantDTO = await _tenantService.CreateTenantAsync(createTenantDTO, userName);

            return Created($"tenants/{tenantDTO.Id}", tenantDTO);
        }

        // GET: Get tenant(s)
        [HttpGet]
        [Route("tenants")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<TenantDTO>>> GetTenantsAsync([FromQuery] bool includeDeleted = false, [FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            return Ok(await _tenantService.GetTenantsAsync(includeDeleted, page, pageSize));
        }

        // PATCH: Update tenant
        [HttpPatch]
        [Route("tenants/{tenantId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<TenantDTO>> UpdateTenantAsync(Guid tenantId, [FromBody] UpdateTenantDTO updateTenantDTO)
        {
            // request validations
            if (updateTenantDTO is null) return BadRequest("Incorrect body format");

            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            return Ok(await _tenantService.UpdateTenantAsync(tenantId, updateTenantDTO, userName));
        }

        // DELETE: Delete tenant
        [HttpDelete]
        [Route("tenants/{tenantId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid tenantId)
        {
            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            await _tenantService.SetDeletedTenantAsync(tenantId, true, userName);

            return NoContent();
        }

        // POST: Undelete tenant
        [HttpPost]
        [Route("tenants/{tenantId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid tenantId)
        {
            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            await _tenantService.SetDeletedTenantAsync(tenantId, false, userName);

            return NoContent();
        }
    }
}
