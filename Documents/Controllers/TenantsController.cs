using Documents.Models;
﻿using Auth.Utils;
using Documents.Services.AzureBlobStorage;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Tenants.Controllers
{
    [ApiController]
    [Route("api/dms")]
    public class TenantsController : Controller
    {
        private readonly IAzureBlobStorage _azureBlobStorage;
        private readonly IValidator<Tenant> _tenantValidator;

        public TenantsController(IAzureBlobStorage azureBlobStorage, IValidator<Tenant> tenantValidator)
        {
            _azureBlobStorage = azureBlobStorage;
            _tenantValidator = tenantValidator;
        }

        // POST: Create tenant
        [HttpPost]
        [Route("tenants")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [SecurityControl]
        public async Task<IActionResult> Create([FromBody] Tenant tenant)
        {
            try
            {
                if (tenant == null) return BadRequest("Incorrect body format");

                await _tenantValidator.ValidateAndThrowAsync(tenant);

                await _azureBlobStorage.CreateBlobContainerAsync(tenant.Name);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: Get tenant(s)
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

        // DELETE: Delete tenant
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

        // POST: Undelete tenant
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
