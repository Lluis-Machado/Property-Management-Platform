﻿using Accounting.Repositories;
using Accounting.Models;
using Accounting.Security;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class BusinessPartnersController : Controller
    {
        private readonly IBusinessPartnerRepository _businessPartnerRepo;
        private readonly IValidator<BusinessPartner> _businessPartnerValidator;
        private readonly ILogger<BusinessPartnersController> _logger;

        public BusinessPartnersController(IBusinessPartnerRepository businessPartnerRepository, IValidator<BusinessPartner> businessPartnerValidator, ILogger<BusinessPartnersController> logger)
        {
            _businessPartnerRepo = businessPartnerRepository;
            _businessPartnerValidator = businessPartnerValidator;
            _logger = logger;
        }

        // POST: Create businessPartner
        [Authorize]
        [HttpPost]
        [Route("businessPartners")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] BusinessPartner businessPartner)
        {
            // request validations
            if (businessPartner == null) return BadRequest("Incorrect body format");
            if (businessPartner.Id != Guid.Empty) return BadRequest("businessPartner Id field must be empty");

            // businessPartner validation
            ValidationResult validationResult = await _businessPartnerValidator.ValidateAsync(businessPartner);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            businessPartner = await _businessPartnerRepo.InsertBusinessPartnerAsync(businessPartner);
            return Created($"businessPartners/{businessPartner.Id}", businessPartner);
        }

        // GET: Get businessPartner(s)
        [Authorize]
        [HttpGet]
        [Route("businessPartners")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<BusinessPartner>>> GetAsync()
        {
            return Ok(await _businessPartnerRepo.GetBusinessPartnersAsync());
        }

        // POST: update businessPartner
        [Authorize]
        [HttpPost]
        [Route("businessPartners/{businessPartnerId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAsync([FromBody] BusinessPartner businessPartner, Guid businessPartnerId)
        {
            // request validations
            if (businessPartner == null) return BadRequest("Incorrect body format");
            if (businessPartner.Id != businessPartnerId) return BadRequest("businessPartner Id from body incorrect");

            // businessPartner validation
            ValidationResult validationResult = await _businessPartnerValidator.ValidateAsync(businessPartner);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            businessPartner.Id = businessPartnerId; // copy id to businessPartner object

            int result = await _businessPartnerRepo.UpdateBusinessPartnerAsync(businessPartner);
            if (result == 0) return NotFound("BusinessPartner not found");
            return NoContent();
        }

        // DELETE: delete businessPartner
        [Authorize]
        [HttpDelete]
        [Route("businessPartners/{businessPartnerId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid businessPartnerId)
        {
            int result = await _businessPartnerRepo.SetDeleteBusinessPartnerAsync(businessPartnerId, true);
            if (result == 0) return NotFound("businessPartner not found");
            return NoContent();
        }

        // POST: undelete businessPartner
        [Authorize]
        [HttpPost]
        [Route("businessPartners/{businessPartnerId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid businessPartnerId)
        {
            int result = await _businessPartnerRepo.SetDeleteBusinessPartnerAsync(businessPartnerId, false);
            if (result == 0) return NotFound("businessPartner not found");
            return NoContent();
        }
    }
}
