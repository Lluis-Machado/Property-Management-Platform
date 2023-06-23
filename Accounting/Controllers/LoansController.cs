﻿using AccountingAPI.DTOs;
using AccountingAPI.Services;
using AccountingAPI.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountingAPI.Controllers
{
    [Authorize]
    public class LoansController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly ILogger<LoansController> _logger;

        public LoansController(ILoanService loanService, ILogger<LoansController> logger)
        {
            _loanService = loanService;
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

            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            LoanDTO loanDTO = await _loanService.CreateLoanAsync(createLoanDTO, userName);
            return Created($"loans/{loanDTO.Id}", loanDTO);
        }

        // GET: Get loan(s)
        [HttpGet]
        [Route("tenants/{tenantId}/loans")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<LoanDTO>>> GetAsync(Guid tenantId, [FromQuery] bool includeDeleted = false)
        {
            return Ok(await _loanService.GetLoansAsync(tenantId, includeDeleted));
        }

        // PATCH: update loan
        [HttpPatch]
        [Route("tenants/{tenantId}/loans/{loanId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<LoanDTO>> UpdateAsync(Guid tenantId, [FromBody] UpdateLoanDTO updateLoanDTO, Guid loanId)
        {
            // request validations
            if (updateLoanDTO == null) return BadRequest("Incorrect body format");

            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            LoanDTO loanDTO = await _loanService.UpdateLoanAsync(tenantId, loanId, updateLoanDTO, userName);

            return Ok(loanDTO);
        }

        // DELETE: delete loan
        [HttpDelete]
        [Route("tenants/{tenantId}/loans/{loanId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid tenantId, Guid loanId)
        {
            await _loanService.SetDeletedLoanAsync(tenantId,loanId, true);

            return NoContent();
        }

        // POST: undelete loan
        [HttpPost]
        [Route("tenants/{tenantId}/loans/{loanId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid tenantId, Guid loanId)
        {
            await _loanService.SetDeletedLoanAsync(tenantId, loanId, false);

            return NoContent();
        }
    }
}
