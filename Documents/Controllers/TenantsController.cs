using Auth.Utils;
using Documents.Extensions;
using Documents.Models;
using Documents.Services.AzureBlobStorage;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Tenants.Controllers
{
    [ApiController]
    [Route("api/dms")]
    public class TenantsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IAzureBlobStorage _azureBlobStorage;

        public TenantsController(IConfiguration config, IAzureBlobStorage azureBlobStorage)
        {
            _config = config;
            _azureBlobStorage = azureBlobStorage;
        }

        // Create tenant
        [HttpPost]
        [Route("tenants")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [SecurityControl]
        public async Task<IActionResult> Create([FromBody] Tenant tenant)
        {
            try
            {
                // tenant name validation
                if (string.IsNullOrEmpty(tenant.Name)) return BadRequest("Tenant name is empty");

                // special characters and empty spaces validation
                if(tenant.Name.HasSpecialCharacters()) return BadRequest("Tenant cannot have special characters");

                await _azureBlobStorage.CreateBlobContainerAsync(tenant.Name);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Get tenant(s)
        [HttpGet]
        [Route("tenants")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<Tenant>), (int)HttpStatusCode.OK)]
        [SecurityControl]
        public async Task<IActionResult> GetAsync([FromQuery] bool includeDeleted = false)
        {
            try
            {
                List<Tenant> tenants = await _azureBlobStorage.GetBlobContainers(100, includeDeleted);
                return Ok(tenants);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Delete tenant
        [HttpDelete]
        [Route("tenants/{tenantName}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [SecurityControl]
        public async Task<IActionResult> DeleteAsync(string tenantName)
        {
            try
            {
                await _azureBlobStorage.DeleteBlobContainerAsync(tenantName);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Undelete tenant
        [HttpPost]
        [Route("tenants/{tenantName}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [SecurityControl]
        public async Task<IActionResult> UndeleteAsync(string tenantName)
        {
            try
            {
                await _azureBlobStorage.UndeleteBlobContainerAsync(tenantName);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


    }
}
