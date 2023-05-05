using Accounting.Models;
using Accounting.Repositories;
using Accounting.Security;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class TenantsController : Controller
    {
        private readonly ITenantRepository _tenantRepo;
        private readonly IValidator<Tenant> _tenantValidator;
        private readonly ILogger<TenantsController> _logger;

        public TenantsController(ITenantRepository tenantRepo, IValidator<Tenant> tenantValidator, ILogger<TenantsController> logger)
        {
            _tenantRepo = tenantRepo;
            _tenantValidator = tenantValidator;
            _logger = logger;
        }

        // POST: Create tenant
        [Authorize]
        [HttpPost]
        [Route("tenants")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] Tenant tenant)
        {
            // request validations
            if (tenant == null) return BadRequest("Incorrect body format");
            if (tenant.Id != Guid.Empty) return BadRequest("Tenant Id field must be empty");

            // tenant validation
            ValidationResult validationResult = await _tenantValidator.ValidateAsync(tenant);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            tenant = await _tenantRepo.InsertTenantAsync(tenant);
            return Created($"tenants/{tenant.Id}", tenant);
        }

        // GET: Get tenant(s)
        [Authorize]
        [HttpGet]
        [Route("tenants")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Tenant>>> GetAsync()
        {
            return Ok(await _tenantRepo.GetTenantsAsync());
        }

        // POST: update tenant
        [Authorize]
        [HttpPost]
        [Route("tenants/{tenantId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromBody] Tenant tenant, Guid tenantId)
        {
            // request validations
            if (tenant == null) return BadRequest("Incorrect body format");
            if (tenant.Id != tenantId) return BadRequest("Tenant Id from body incorrect");

            // tenant validation
            ValidationResult validationResult = await _tenantValidator.ValidateAsync(tenant);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            tenant.Id = tenantId; // copy id to tenant object

            int result = await _tenantRepo.UpdateTenantAsync(tenant);
            if (result == 0) return NotFound("Tenant not found");
            return NoContent();
        }

        // DELETE: delete tenant
        [Authorize]
        [HttpDelete]
        [Route("tenants/{tenantId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid tenantId)
        {
            int result = await _tenantRepo.SetDeleteTenantAsync(tenantId, true);
            if (result == 0) return NotFound("Tenant not found");
            return NoContent();
        }

        // POST: undelete tenant
        [Authorize]
        [HttpPost]
        [Route("tenants/{tenantId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid tenantId)
        {
            int result = await _tenantRepo.SetDeleteTenantAsync(tenantId, false);
            if (result == 0) return NotFound("Tenant not found");
            return NoContent();
        }
    }
}
