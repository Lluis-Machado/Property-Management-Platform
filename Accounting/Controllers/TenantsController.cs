using AccountingAPI.DTOs;
using AccountingAPI.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountingAPI.Controllers
{
    [Authorize]
    public class TenantsController : Controller
    {
        private readonly ITenantService _tenantService;
        private readonly IValidator<CreateTenantDTO> _tenantDTOValidator;
        private readonly ILogger<TenantsController> _logger;

        public TenantsController(ITenantService tenantService, IValidator<CreateTenantDTO> tenantDTOValidator, ILogger<TenantsController> logger)
        {
            _tenantService = tenantService;
            _tenantDTOValidator = tenantDTOValidator;
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
            if (createTenantDTO == null) return BadRequest("Incorrect body format");

            ValidationResult validationResult = await _tenantDTOValidator.ValidateAsync(createTenantDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            TenantDTO tenantDTO = await _tenantService.CreateTenantAsync(createTenantDTO, User?.Identity?.Name);

            return Created($"tenants/{tenantDTO.Id}", tenantDTO);
        }

        // GET: Get tenant(s)
        [HttpGet]
        [Route("tenants")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<TenantDTO>>> GetTenantsAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _tenantService.GetTenantsAsync(includeDeleted));
        }

        // PATCH: update tenant
        [HttpPatch]
        [Route("tenants/{tenantId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<TenantDTO>> UpdateTenantAsync([FromBody] CreateTenantDTO createTenantDTO, Guid tenantId)
        {
            // request validations
            if (createTenantDTO == null) return BadRequest("Incorrect body format");

            ValidationResult validationResult = await _tenantDTOValidator.ValidateAsync(createTenantDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // check if exists
            if (!await _tenantService.CheckIfTenantExistsAsync(tenantId)) return NotFound("Tenant not found");

            return Ok(await _tenantService.UpdateTenantAsync(createTenantDTO, User?.Identity?.Name, tenantId));
        }

        // DELETE: delete tenant
        [HttpDelete]
        [Route("tenants/{tenantId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid tenantId)
        {
            // check if exists
            if (!await _tenantService.CheckIfTenantExistsAsync(tenantId)) return NotFound("Tenant not found");

            await _tenantService.SetDeletedTenantAsync(tenantId, true);

            return NoContent();
        }

        // POST: undelete tenant
        [HttpPost]
        [Route("tenants/{tenantId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid tenantId)
        {
            // check if exists
            if (!await _tenantService.CheckIfTenantExistsAsync(tenantId)) return NotFound("Tenant not found");

            await _tenantService.SetDeletedTenantAsync(tenantId, false);

            return NoContent();
        }
    }
}
