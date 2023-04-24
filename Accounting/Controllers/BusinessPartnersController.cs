using Accounting.Repositories;
using Accounting.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class BusinessPartnersController :  Controller
    {
        private readonly IBusinessPartnerRepository _businessPartnerRepo;
        private readonly IValidator<BusinessPartner> _businessPartnerValidator; 
        public BusinessPartnersController(IBusinessPartnerRepository businessPartnerRepository, IValidator<BusinessPartner> businessPartnerValidator)
        {
            _businessPartnerRepo = businessPartnerRepository;
            _businessPartnerValidator = businessPartnerValidator;
        }

        // POST: Create businessPartner
        [HttpPost]
        [Route("businessPartners")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAsync([FromBody] BusinessPartner businessPartner)
        {
            try
            {
                // validations
                if (businessPartner == null) return BadRequest("Incorrect body format");
                if (businessPartner.Id != Guid.Empty) return BadRequest("Id field must be empty");

                await _businessPartnerValidator.ValidateAndThrowAsync(businessPartner);

                Guid businessPartnerId = await _businessPartnerRepo.InsertBusinessPartnerAsync(businessPartner);
                return Ok(businessPartnerId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: Get businessPartner(s)
        [HttpGet]
        [Route("businessPartners")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<BusinessPartner>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                IEnumerable<BusinessPartner> businessPartners = await _businessPartnerRepo.GetBusinessPartnersAsync();
                return Ok(businessPartners.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: update businessPartner
        [HttpPost]
        [Route("businessPartners/{businessPartnerId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAsync([FromBody] BusinessPartner businessPartner, Guid businessPartnerId)
        {
            try
            {
                // validations
                if (businessPartner == null) return BadRequest("Incorrect body format");
                if (businessPartner.Id != businessPartnerId) return BadRequest("businessPartnerId from body incorrect");
                businessPartner.Id = businessPartnerId;

                await _businessPartnerValidator.ValidateAndThrowAsync(businessPartner);

                int result = await _businessPartnerRepo.UpdateBusinessPartnerAsync(businessPartner);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: delete businessPartner
        [HttpDelete]
        [Route("businessPartners/{businessPartnerId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync(Guid businessPartnerId)
        {
            try
            {
                BusinessPartner businessPartner = await _businessPartnerRepo.GetBusinessPartnerByIdAsync(businessPartnerId);
                businessPartner.Deleted = true;
                int result = await _businessPartnerRepo.UpdateBusinessPartnerAsync(businessPartner);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: undelete businessPartner
        [HttpPost]
        [Route("businessPartners/{businessPartnerId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UndeleteAsync(Guid businessPartnerId)
        {
            try
            {
                BusinessPartner businessPartner = await _businessPartnerRepo.GetBusinessPartnerByIdAsync(businessPartnerId);
                businessPartner.Deleted = false;
                int result = await _businessPartnerRepo.UpdateBusinessPartnerAsync(businessPartner);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
