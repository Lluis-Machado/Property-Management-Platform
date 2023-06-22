using Accounting.Models;
using Accounting.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    [Authorize]
    public class LoansController : Controller
    {
        private readonly ILoanRepository _loanRepo;
        private readonly IBusinessPartnerRepository _businessPartnerRepo;
        private readonly IValidator<Loan> _loanValidator;
        private readonly ILogger<LoansController> _logger;

        public LoansController(ILoanRepository loanRepo, IBusinessPartnerRepository businessPartnerRepo, IValidator<Loan> loanValidator, ILogger<LoansController> logger)
        {
            _loanRepo = loanRepo;
            _businessPartnerRepo = businessPartnerRepo;
            _loanValidator = loanValidator;
            _logger = logger;
        }

        // POST: Create Loan

        [HttpPost]
        [Route("loans")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] Loan loan, Guid businessPartnerId)
        {
            // request validations
            if (loan == null) return BadRequest("Incorrect body format");
            if (loan.Id != Guid.Empty) return BadRequest("Loan Id field must be empty");
            if (loan.BusinessPartnerId != businessPartnerId) return BadRequest("Incorrect BusinessPartner Id in body");

            // loan validator
            ValidationResult validationResult = await _loanValidator.ValidateAsync(loan);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // businessPartner validation
            if (!await BusinessPartnerExists(businessPartnerId)) return NotFound("BusinessPartner not found");

            loan = await _loanRepo.InsertLoanAsync(loan);
            return Created($"loans/{loan.Id}", loan);
        }

        // GET: Get loan(s)

        [HttpGet]
        [Route("loans")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Loan>>> GetAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _loanRepo.GetLoansAsync(includeDeleted));
        }

        // PATCH: update loan

        [HttpPatch]
        [Route("loans/{loanId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAsync([FromBody] Loan loan, Guid businessPartnerId, Guid loanId)
        {
            // request validations
            if (loan == null) return BadRequest("Incorrect body format");
            if (loan.Id != loanId) return BadRequest("Loan Id from body incorrect");

            // loan validation
            ValidationResult validationResult = await _loanValidator.ValidateAsync(loan);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // businessPartner validation
            if (!await BusinessPartnerExists(businessPartnerId)) return NotFound("BusinessPartner not found");

            loan.Id = loanId; // copy id to loan object

            int result = await _loanRepo.UpdateLoanAsync(loan);
            if (result == 0) return NotFound("Loan not found");
            return NoContent();
        }

        // DELETE: delete loan

        [HttpDelete]
        [Route("loans/{loanId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid loanId)
        {
            int result = await _loanRepo.SetDeleteLoanAsync(loanId, true);
            if (result == 0) return NotFound("Loan not found");
            return NoContent();
        }

        // POST: undelete loan

        [HttpPost]
        [Route("loans/{loanId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid loanId)
        {
            int result = await _loanRepo.SetDeleteLoanAsync(loanId, false);
            if (result == 0) return NotFound("Loan not found");
            return NoContent();
        }

        private async Task<bool> BusinessPartnerExists(Guid businessPartnerId)
        {
            BusinessPartner? businessPartner = await _businessPartnerRepo.GetBusinessPartnerByIdAsync(businessPartnerId);
            return (businessPartner != null && businessPartner?.Deleted == false);
        }
    }
}
