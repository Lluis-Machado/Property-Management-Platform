using Azure;
using Documents.Models;
using Documents.Services.AzureBlobStorage;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaxManagement.Security;

namespace Tenants.Controllers
{
    [ApiController]
    public class TenantsController : Controller
    {
        private readonly ILogger<TenantsController> _logger;
        private readonly IAzureBlobStorage _azureBlobStorage;
        private readonly IValidator<Tenant> _tenantValidator;

        public TenantsController(IAzureBlobStorage azureBlobStorage, IValidator<Tenant> tenantValidator, ILogger<TenantsController> logger)
        {
            _azureBlobStorage = azureBlobStorage;
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
        public async Task<IActionResult> Create([FromBody] Tenant tenant)
        {
            //validations
            if (tenant == null) return BadRequest("Incorrect body format");

            ValidationResult validationResult = await _tenantValidator.ValidateAsync(tenant);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            if (tenant.Name == null) return BadRequest("Tenant Name is empty");

            //create tenant
            await _azureBlobStorage.CreateBlobContainerAsync(tenant.Name);
            return Created($"tenants/{tenant.Name}", tenant);
        }

        // GET: Get tenant(s)
        [Authorize]
        [HttpGet]
        [Route("tenants")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Tenant>>> GetAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _azureBlobStorage.GetBlobContainersAsync(100, includeDeleted));
        }

        // DELETE: Delete tenant
        [Authorize]
        [HttpDelete]
        [Route("tenants/{tenantName}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(string tenantName)
        {
            await _azureBlobStorage.DeleteBlobContainerAsync(tenantName);
            return NoContent();
        }

        // POST: Undelete tenant
        [Authorize]
        [HttpPatch]
        [Route("tenants/{tenantName}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(string tenantName)
        {
            await _azureBlobStorage.UndeleteBlobContainerAsync(tenantName);
            return NoContent();
        }
    }
}
