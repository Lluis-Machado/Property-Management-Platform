using Azure;
using Documents.Models;
using Documents.Services.AzureBlobStorage;
using FluentValidation;
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
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> Create([FromBody] Tenant tenant)
        {
            try
            {
                //validations
                if (tenant == null) return BadRequest("Incorrect body format");

                await _tenantValidator.ValidateAndThrowAsync(tenant);

                if(tenant.Name == null) return BadRequest("Tenant Name is empty");

                //create tenant
                await _azureBlobStorage.CreateBlobContainerAsync(tenant.Name);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError("Internal exception while Create tenant request: {@RQ} {@Exception}",tenant, e);
                return Conflict("Internal exception ocurred");
            }

        }

        // GET: Get tenant(s)
        [Authorize]
        [HttpGet]
        [Route("tenants")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<IEnumerable<Tenant>>> GetAsync([FromQuery] bool includeDeleted = false)
        {
            try
            {
                IEnumerable<Tenant> tenants = await _azureBlobStorage.GetBlobContainersAsync(100, includeDeleted);
                return Ok(tenants);
            }
            catch (Exception e)
            {
                _logger.LogError("Internal exception while Get tenant(s) request: {@RQ} {@Exception}", includeDeleted, e);
                return Conflict("Internal exception ocurred");
            }
        }

        // DELETE: Delete tenant
        [Authorize]
        [HttpDelete]
        [Route("tenants/{tenantName}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(string tenantName)
        {
            try
            {
                bool deleted = await _azureBlobStorage.DeleteBlobContainerAsync(tenantName);
                if(!deleted) return NotFound("Tenant not found");
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError("Internal exception while Delete tenant request: {RQ} {@Exception}", tenantName, e);
                return Conflict("Internal exception ocurred");
            }
        }

        // POST: Undelete tenant
        [Authorize]
        [HttpPost]
        [Route("tenants/{tenantName}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(string tenantName)
        {
            try
            {
                bool undeleted = await _azureBlobStorage.UndeleteBlobContainerAsync(tenantName);
                if (!undeleted) return NotFound("Tenant not found");
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError("Internal exception while Undelete tenant request: {RQ} {@Exception}", tenantName, e);
                return Conflict("Internal exception ocurred");
            }
        }


    }
}
