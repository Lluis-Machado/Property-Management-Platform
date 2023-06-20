using AccountingAPI.DTOs;
using AccountingAPI.Services;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountingAPI.Controllers
{
    [Authorize]
    public class LoansController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly IValidator<CreateLoanDTO> _loanValidator;
        private readonly ILogger<LoansController> _logger;

        public LoansController(ILoanService loanService, IValidator<CreateLoanDTO> loanValidator, ILogger<LoansController> logger)
        {
            _loanService = loanService;
            _loanValidator = loanValidator;
            _logger = logger;
        }

        // POST: Create Loan
        [HttpPost]
        [Route("tenants/{tenantId}/businesspartners/{businessPartnerId}/loans")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<LoanDTO>> CreateAsync([FromBody] CreateLoanDTO createLoanDTO, Guid businessPartnerId)
        {
            // request validations
            if (createLoanDTO == null) return BadRequest("Incorrect body format");

            // loan validator
            ValidationResult validationResult = await _loanValidator.ValidateAsync(createLoanDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            LoanDTO loanDTO = await _loanService.CreateLoanAsync(createLoanDTO, User?.Identity?.Name);
            return Created($"loans/{loanDTO.Id}", loanDTO);
        }

        // GET: Get loan(s)
        [HttpGet]
        [Route("tenants/{tenantId}/loans")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<LoanDTO>>> GetAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _loanService.GetLoansAsync(includeDeleted));
        }

        // PATCH: update loan
        [HttpPatch]
        [Route("tenants/{tenantId}/loans/{loanId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<LoanDTO>> UpdateAsync([FromBody] CreateLoanDTO createLoanDTO, Guid loanId)
        {
            // request validations
            if (createLoanDTO == null) return BadRequest("Incorrect body format");

            // loan validation
            ValidationResult validationResult = await _loanValidator.ValidateAsync(createLoanDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // check if exists
            if (!await _loanService.CheckIfLoanExistsAsync(loanId)) return NotFound("Loan not found");

            LoanDTO loanDTO = await _loanService.UpdateLoanAsync(createLoanDTO, User?.Identity?.Name);

            return Ok(loanDTO);
        }

        // DELETE: delete loan
        [HttpDelete]
        [Route("tenants/{tenantId}/loans/{loanId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid loanId)
        {
            // check if exists
            if (!await _loanService.CheckIfLoanExistsAsync(loanId)) return NotFound("Loan not found");

            await _loanService.SetDeletedLoanAsync(loanId, true);

            return NoContent();
        }

        // POST: undelete loan
        [HttpPost]
        [Route("tenants/{tenantId}/loans/{loanId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid loanId)
        {
            // check if exists
            if (!await _loanService.CheckIfLoanExistsAsync(loanId)) return NotFound("Loan not found");

            await _loanService.SetDeletedLoanAsync(loanId, false);

            return NoContent();
        }
    }
}
