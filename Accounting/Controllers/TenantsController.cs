using Accounting.Models;
using Accounting.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

namespace Accounting.Controllers
{
    public class TenantsController : Controller
    {
        private readonly ITenantRepository _tenantRepo;
        private readonly IValidator<Tenant> _tenantValidator;
        public TenantsController(ITenantRepository tenantRepo, IValidator<Tenant> tenantValidator)
        {
            _tenantRepo = tenantRepo;
            _tenantValidator = tenantValidator;
        }

        // POST: Create tenant
        [HttpPost]
        [Route("tenants")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAsync([FromBody] Tenant tenant)
        {
            try
            {
                // validations
                if (tenant == null) return BadRequest("Incorrect body format");
                if (tenant.Id != Guid.Empty) return BadRequest("Id field must be empty");

                await _tenantValidator.ValidateAndThrowAsync(tenant);

                Guid tenantId = await _tenantRepo.InsertTenantAsync(tenant);
                return Ok(tenantId);
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
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                IEnumerable<Tenant> tenants = await _tenantRepo.GetTenantsAsync();
                return Ok(tenants.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: update tenant
        [HttpPost]
        [Route("tenants/{tenantId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAsync([FromBody] Tenant tenant, Guid tenantId)
        {
            try
            {
                // validations
                if (tenant == null) return BadRequest("Incorrect body format");
                if (tenant.Id != tenantId) return BadRequest("Account Id from body incorrect");
                tenant.Id = tenantId;

                await _tenantValidator.ValidateAndThrowAsync(tenant);

                int result = await _tenantRepo.UpdateTenantAsync(tenant);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: delete tenant
        [HttpDelete]
        [Route("tenants/{tenantId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync(Guid tenantId)
        {
            try
            {
                Tenant tenant = await _tenantRepo.GetTenantByIdAsync(tenantId);
                tenant.Deleted = true;
                int result = await _tenantRepo.UpdateTenantAsync(tenant);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: undelete tenant
        [HttpPost]
        [Route("tenants/{tenantId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UndeleteAsync(Guid tenantId)
        {
            try
            {
                Tenant tenant = await _tenantRepo.GetTenantByIdAsync(tenantId);
                tenant.Deleted = false;
                int result = await _tenantRepo.UpdateTenantAsync(tenant);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
