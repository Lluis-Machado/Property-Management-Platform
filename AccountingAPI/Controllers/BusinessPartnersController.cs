using AccountingAPI.DTOs;
using AccountingAPI.Services;
using AccountingAPI.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountingAPI.Controllers
{
    [Authorize]
    public class BusinessPartnersController : Controller
    {
        private readonly IBusinessPartnerService _businessPartnerService;
        private readonly ILogger<BusinessPartnersController> _logger;

        public BusinessPartnersController(IBusinessPartnerService businessPartnerServicesitory, ILogger<BusinessPartnersController> logger)
        {
            _businessPartnerService = businessPartnerServicesitory;
            _logger = logger;
        }

        // POST: Create businessPartner
        [HttpPost]
        [Route("tenants/{tenantId}/businessPartners")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<BusinessPartnerDTO>> CreateBusinessPartnerAsync(Guid tenantId, [FromBody] CreateBusinessPartnerDTO createBusinessPartnerDTO)
        {
            // request validations
            if (createBusinessPartnerDTO is null) return BadRequest("Incorrect body format");

            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            BusinessPartnerDTO businessPartnerDTO = await _businessPartnerService.CreateBusinessPartnerAsync(tenantId, createBusinessPartnerDTO, userName);

            return Created($"businessPartners/{businessPartnerDTO.Id}", businessPartnerDTO);
        }

        // GET: Get businessPartner(s) for a specific tenant
        [HttpGet]
        [Route("tenants/{tenantId}/businessPartners")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<BusinessPartnerDTO>>> GetBusinessPartnersAsync(Guid tenantId, [FromQuery] bool includeDeleted = false, [FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            return Ok(await _businessPartnerService.GetBusinessPartnersAsync(tenantId, includeDeleted, page, pageSize));
        }

        // GET: Get businessPartner(s) for a specific tenant
        [HttpGet]
        [Route("businessPartners")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<BusinessPartnerDTO>>> GetAllBusinessPartnersAsync([FromQuery] bool includeDeleted = false, [FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            return Ok(await _businessPartnerService.GetBusinessPartnersAsync(null, includeDeleted, page, pageSize));
        }

        // PATCH: Update businessPartner
        [HttpPatch]
        [Route("tenants/{tenantId}/businessPartners/{businessPartnerId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<BusinessPartnerDTO>> UpdateBusinessPartneAsync(Guid tenantId, Guid businessPartnerId, [FromBody] UpdateBusinessPartnerDTO updateBusinessPartnerDTO)
        {
            // request validations
            if (updateBusinessPartnerDTO is null) return BadRequest("Incorrect body format");

            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            // update
            BusinessPartnerDTO businessPartnerDTO = await _businessPartnerService.UpdateBusinessPartnerAsync(tenantId, businessPartnerId, updateBusinessPartnerDTO, userName);

            return Ok(businessPartnerDTO);
        }

        // DELETE: Delete businessPartner
        [HttpDelete]
        [Route("tenants/{tenantId}/businessPartners/{businessPartnerId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteBusinessPartnerAsync(Guid tenantId, Guid businessPartnerId)
        {
            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            await _businessPartnerService.SetDeletedBusinessPartnerAsync(tenantId, businessPartnerId, true, userName);

            return NoContent();
        }

        // POST: undelete businessPartner
        [HttpPost]
        [Route("tenants/{tenantId}/businessPartners/{businessPartnerId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteBusinessPartnerAsync(Guid tenantId, Guid businessPartnerId)
        {
            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            await _businessPartnerService.SetDeletedBusinessPartnerAsync(tenantId, businessPartnerId, false, userName);

            return NoContent();
        }
    }
}
