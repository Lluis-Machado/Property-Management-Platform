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
    public class APInvoicesController : Controller
    {
        private readonly IAPInvoiceService _apInvoiceService;
        private readonly ILogger<APInvoicesController> _logger;

        public APInvoicesController(IAPInvoiceService apInvoiceService, ILogger<APInvoicesController> logger)
        {
            _apInvoiceService = apInvoiceService;
            _logger = logger;
        }

        // POST: Create APInvoice
        [HttpPost]
        [Route("tenants/{tenantId}/businesspartners/{businessPartnerId}/apinvoices")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<APInvoiceDTO>> CreateAPInvoiceAsync([FromBody] CreateAPInvoiceDTO createAPInvoiceDTO, Guid tenantId, Guid businessPartnerId)
        {
            // request validations
            if (createAPInvoiceDTO is null) return BadRequest("Incorrect body format");

            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            APInvoiceDTO aPInvoiceDTO = await _apInvoiceService.CreateAPInvoiceAndLinesAsync(createAPInvoiceDTO, userName, businessPartnerId);
           
            return Created($"apinvoices/{aPInvoiceDTO.Id}", aPInvoiceDTO);
        }

        // GET: Read AP invoice(s)
        [HttpGet]
        [Route("tenants/{tenantId}/apinvoices")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<APInvoiceDTO>>> GetAPInvoicesAsync(Guid tenantId, [FromQuery] bool includeDeleted = false)
        {
            return Ok(await _apInvoiceService.GetAPInvoicesAsync(includeDeleted));
        }

        // PATCH: Update invoice
        [HttpPatch]
        [Route("tenants/{tenantId}/apinvoices/{invoiceId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAPInvoiceAsync([FromBody] UpdateAPInvoiceDTO updateAPInvoiceDTO,Guid tenantId, Guid invoiceId)
        {
            // request validations
            if (updateAPInvoiceDTO == null) return BadRequest("Incorrect body format");

            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            APInvoiceDTO invoiceDTO = await _apInvoiceService.UpdateAPInvoiceAndLinesAsync(tenantId, invoiceId, updateAPInvoiceDTO, userName);
            return Ok(invoiceDTO);
        }

        // DELETE: Delete ap invoice
        [HttpDelete]
        [Route("tenants/{tenantId}/apinvoices/{invoiceId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid invoiceId)
        {
            await _apInvoiceService.SetDeletedAPInvoiceAsync(invoiceId, true, userName);

            return NoContent();
        }

        // POST: Undelete invoice
        [HttpPost]
        [Route("tenants/{tenantId}/apinvoices/{invoiceId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid invoiceId)
        {
            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            // check if exists
            if (!await _apInvoiceService.CheckIfAPInvoiceExistsAsync(invoiceId)) return NotFound("Invoice not found");

            await _apInvoiceService.SetDeletedAPInvoiceAsync(invoiceId, false, userName);

            return NoContent();
        }
    }
}
