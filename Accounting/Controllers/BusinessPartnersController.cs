using AccountingAPI.DTOs;
using AccountingAPI.Services;
using AccountingAPI.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountingAPI.Controllers
{
    [Authorize]
    public class BusinessPartnersController : Controller
    {
        private readonly IBusinessPartnerService _businessPartnerService;
        private readonly IValidator<CreateBusinessPartnerDTO> _businessPartnerValidator;
        private readonly ILogger<BusinessPartnersController> _logger;

        public BusinessPartnersController(IBusinessPartnerService businessPartnerServicesitory, IValidator<CreateBusinessPartnerDTO> businessPartnerValidator, ILogger<BusinessPartnersController> logger)
        {
            _businessPartnerService = businessPartnerServicesitory;
            _businessPartnerValidator = businessPartnerValidator;
            _logger = logger;
        }

        // POST: Create businessPartner
        [HttpPost]
        [Route("tenants/{tenantId}/businessPartners")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<BusinessPartnerDTO>> CreateBusinessPartnerAsync([FromBody] CreateBusinessPartnerDTO createBusinessPartnerDTO, Guid tenantId)
        {
            // request validations
            if (createBusinessPartnerDTO == null) return BadRequest("Incorrect body format");

            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            ValidationResult validationResult = await _businessPartnerValidator.ValidateAsync(createBusinessPartnerDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            BusinessPartnerDTO businessPartnerDTO = await _businessPartnerService.CreateBusinessPartnerAsync(createBusinessPartnerDTO, userName, tenantId);

            return Created($"businessPartners/{businessPartnerDTO.Id}", businessPartnerDTO);
        }

        // GET: Get businessPartner(s)
        [HttpGet]
        [Route("tenants/{tenantId}/businessPartners")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<BusinessPartnerDTO>>> GetBusinessPartnersAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _businessPartnerService.GetBusinessPartnersAsync(includeDeleted));
        }

        // PATCH: update businessPartner
        [HttpPatch]
        [Route("tenants/{tenantId}/businessPartners/{businessPartnerId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<BusinessPartnerDTO>> UpdateBusinessPartneAsync([FromBody] CreateBusinessPartnerDTO createBusinessPartnerDTO, Guid businessPartnerId)
        {
            // request validations
            if (createBusinessPartnerDTO == null) return BadRequest("Incorrect body format");

            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            ValidationResult validationResult = await _businessPartnerValidator.ValidateAsync(createBusinessPartnerDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // check if exists
            if (!await _businessPartnerService.CheckIfBusinessPartnerExists(businessPartnerId)) return NotFound("Business Partner not found");

            // update
            BusinessPartnerDTO businessPartnerDTO = await _businessPartnerService.UpdateBusinessPartnerAsync(createBusinessPartnerDTO, userName, businessPartnerId);

            return Ok(businessPartnerDTO);
        }

        // DELETE: delete businessPartner
        [HttpDelete]
        [Route("tenants/{tenantId}/businessPartners/{businessPartnerId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteBusinessPartnerAsync(Guid businessPartnerId)
        {
            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            // check if exists
            if (!await _businessPartnerService.CheckIfBusinessPartnerExists(businessPartnerId)) return NotFound("Business Partner not found");

            await _businessPartnerService.SetDeletedBusinessPartnerAsync(businessPartnerId, true);

            return NoContent();
        }

        // POST: undelete businessPartner
        [HttpPost]
        [Route("tenants/{tenantId}/businessPartners/{businessPartnerId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteBusinessPartnerAsync(Guid businessPartnerId)
        {
            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            // check if exists
            if (!await _businessPartnerService.CheckIfBusinessPartnerExists(businessPartnerId)) return NotFound("Business Partner not found");

            await _businessPartnerService.SetDeletedBusinessPartnerAsync(businessPartnerId, false);

            return NoContent();
        }
    }
}
