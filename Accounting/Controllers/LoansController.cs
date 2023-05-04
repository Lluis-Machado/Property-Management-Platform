using Accounting.Models;
using Accounting.Repositories;
using Accounting.Security;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class LoansController : Controller
    {
        private readonly ILoanRepository _loanRepo;
        private readonly IValidator<Loan> _loanValidator;
        private readonly ILogger<LoansController> _logger;

        public LoansController(ILoanRepository loanRepo, IValidator<Loan> loanValidator, ILogger<LoansController> logger)
        {
            _loanRepo = loanRepo;
            _loanValidator = loanValidator;
            _logger = logger;
        }

        // POST: Create Loan
        [Authorize]
        [HttpPost]
        [Route("loans")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] Loan loan)
        {
            // request validations
            if (loan == null) return BadRequest("Incorrect body format");
            if (loan.Id != Guid.Empty) return BadRequest("loan Id field must be empty");

            // loan validator
            ValidationResult validationResult = await _loanValidator.ValidateAsync(loan);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            loan = await _loanRepo.InsertLoanAsync(loan);
            return Created($"loans/{loan.Id}", loan);
        }

        // GET: Get loan(s)
        [Authorize]
        [HttpGet]
        [Route("loans")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Loan>>> GetAsync()
        {
            return Ok(await _loanRepo.GetLoansAsync());
        }

        // POST: update loan
        [Authorize]
        [HttpPost]
        [Route("loans/{loanId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAsync([FromBody] Loan loan, Guid loanId)
        {
            // request validations
            if (loan == null) return BadRequest("Incorrect body format");
            if (loan.Id != loanId) return BadRequest("businessPartner Id from body incorrect");

            // loan validation
            ValidationResult validationResult = await _loanValidator.ValidateAsync(loan);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            loan.Id = loanId; // copy id to loan object

            int result = await _loanRepo.UpdateLoanAsync(loan);
            if (result == 0) return NotFound("Loan not found");
            return NoContent();
        }

        // DELETE: delete loan
        [Authorize]
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
        [Authorize]
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
    }
}
