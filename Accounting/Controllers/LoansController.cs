using Accounting.Repositories;
using Accounting.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Accounting.Validators;

namespace Accounting.Controllers
{
    public class LoansController : Controller
    {
        private readonly ILoanRepository _loanRepo;
        private readonly IValidator<Loan> _loanValidator;
        public LoansController(ILoanRepository loanRepo, IValidator<Loan> loanValidator) 
        {
            _loanRepo = loanRepo;
            _loanValidator = loanValidator;
        }

        // POST: Create Loan
        [HttpPost]
        [Route("loans")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAsync([FromBody] Loan loan)
        {
            try
            {
                // validations
                if (loan == null) return BadRequest("Incorrect body format");
                if (loan.Id != Guid.Empty) return BadRequest("Id field must be empty");

                await _loanValidator.ValidateAndThrowAsync(loan);

                Guid loanId = await _loanRepo.InsertLoanAsync(loan);
                return Ok(loanId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: Get loan(s)
        [HttpGet]
        [Route("loans")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<Loan>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                IEnumerable<Loan> loans = await _loanRepo.GetLoansAsync();
                return Ok(loans.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: update loan
        [HttpPost]
        [Route("loans/{loanId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAsync([FromBody] Loan loan, Guid loanId)
        {
            try
            {
                // validations
                if (loan == null) return BadRequest("Incorrect body format");
                if (loan.Id != loanId) return BadRequest("businessPartner Id from body incorrect");
                loan.Id = loanId;

                await _loanValidator.ValidateAndThrowAsync(loan);

                int result = await _loanRepo.UpdateLoanAsync(loan);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: delete loan
        [HttpDelete]
        [Route("loans/{loanId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync(Guid loanId)
        {
            try
            {
                Loan loan= await _loanRepo.GetLoanByIdAsync(loanId);
                loan.Deleted = true;
                int result = await _loanRepo.UpdateLoanAsync(loan);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: undelete loan
        [HttpPost]
        [Route("loans/{loanId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UndeleteAsync(Guid loanId)
        {
            try
            {
                Loan loan= await _loanRepo.GetLoanByIdAsync(loanId);
                loan.Deleted = false;
                int result = await _loanRepo.UpdateLoanAsync(loan);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
