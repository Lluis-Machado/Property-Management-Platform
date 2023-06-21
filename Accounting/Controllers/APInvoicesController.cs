using AccountingAPI.DTOs;
using AccountingAPI.Services;
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
        private readonly IValidator<CreateAPInvoiceDTO> _createAPInvoiceDTOValidator;
        private readonly IValidator<UpdateAPInvoiceDTO> _updateAPInvoiceDTOValidator;
        private readonly ILogger<APInvoicesController> _logger;

        public APInvoicesController(IAPInvoiceService apInvoiceService, IValidator<CreateAPInvoiceDTO> createAPInvoiceDTOValidator, ILogger<APInvoicesController> logger, IValidator<UpdateAPInvoiceDTO> updateAPInvoiceDTOValidator)
        {
            _createAPInvoiceDTOValidator = createAPInvoiceDTOValidator;
            _apInvoiceService = apInvoiceService;
            _logger = logger;
            _updateAPInvoiceDTOValidator = updateAPInvoiceDTOValidator;
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
            if (createAPInvoiceDTO == null) return BadRequest("Incorrect body format");

            // invoice validation
            ValidationResult validationResult = await _createAPInvoiceDTOValidator.ValidateAsync(createAPInvoiceDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            APInvoiceDTO invoiceDTO = await _apInvoiceService.CreateAPInvoiceAndLinesAsync(createAPInvoiceDTO, User?.Identity?.Name, businessPartnerId);
            return Created($"apinvoices/{invoiceDTO.Id}", invoiceDTO);
        }

        // GET: Get AP invoice(s)
        [HttpGet]
        [Route("tenants/{tenantId}/apinvoices")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<APInvoiceDTO>>> GetAPInvoicesAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _apInvoiceService.GetAPInvoicesAsync(includeDeleted));
        }

        // PATCH: update invoice
        [HttpPatch]
        [Route("tenants/{tenantId}/apinvoices/{invoiceId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAPInvoiceAsync([FromBody] UpdateAPInvoiceDTO updateAPInvoiceDTO, Guid invoiceId)
        {
            // request validations
            if (updateAPInvoiceDTO == null) return BadRequest("Incorrect body format");

            // invoice validation
            ValidationResult validationResult = await _updateAPInvoiceDTOValidator.ValidateAsync(updateAPInvoiceDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // check if exists
            if (!await _apInvoiceService.CheckIfAPInvoiceExistsAsync(invoiceId)) return NotFound("Invoice not found");

            APInvoiceDTO invoiceDTO = await _apInvoiceService.UpdateAPInvoiceAndLinesAsync(updateAPInvoiceDTO, User?.Identity?.Name, invoiceId);
            return Ok(invoiceDTO);
        }

        // DELETE: delete ap invoice
        [HttpDelete]
        [Route("tenants/{tenantId}/apinvoices/{invoiceId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid invoiceId)
        {
            // check if exists
            if (!await _apInvoiceService.CheckIfAPInvoiceExistsAsync(invoiceId)) return NotFound("Invoice not found");

            await _apInvoiceService.SetDeletedAPInvoiceAsync(invoiceId, true);

            return NoContent();
        }

        // POST: undelete invoice
        [HttpPost]
        [Route("tenants/{tenantId}/apinvoices/{invoiceId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid invoiceId)
        {
            // check if exists
            if (!await _apInvoiceService.CheckIfAPInvoiceExistsAsync(invoiceId)) return NotFound("Invoice not found");

            await _apInvoiceService.SetDeletedAPInvoiceAsync(invoiceId, false);

            return NoContent();
        }
    }
}
