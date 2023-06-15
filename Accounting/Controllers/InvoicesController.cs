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
    public class InvoicesController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IValidator<CreateInvoiceDTO> _createInvoiceDTOValidator;
        private readonly IValidator<UpdateInvoiceDTO> _updateInvoiceDTOValidator;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(IInvoiceService invoiceService, IValidator<CreateInvoiceDTO> createInvoiceDTOValidator, ILogger<InvoicesController> logger, IValidator<UpdateInvoiceDTO> updateInvoiceDTOValidator)
        {
            _createInvoiceDTOValidator = createInvoiceDTOValidator;
            _invoiceService = invoiceService;
            _logger = logger;
            _updateInvoiceDTOValidator = updateInvoiceDTOValidator;
        }

        // POST: Create Invoice
        [HttpPost]
        [Route("tenants/{tenantId}/businesspartners/{businessPartnerId}/invoices")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<InvoiceDTO>> CreateInvoiceAsync([FromBody] CreateInvoiceDTO createInvoiceDTO,Guid tenantId, Guid businessPartnerId)
        {
            // request validations
            if (createInvoiceDTO == null) return BadRequest("Incorrect body format");

            // invoice validation
            ValidationResult validationResult = await _createInvoiceDTOValidator.ValidateAsync(createInvoiceDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            InvoiceDTO invoiceDTO = await _invoiceService.CreateInvoiceAndLinesAsync(createInvoiceDTO, User?.Identity?.Name, businessPartnerId);
            return Created($"invoices/{invoiceDTO.Id}", invoiceDTO);
        }

        // GET: Get invoice(s)
        [HttpGet]
        [Route("tenants/{tenantId}/invoices")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<InvoiceDTO>>> GetInvoicesAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _invoiceService.GetInvoicesAsync(includeDeleted));
        }

        // PATCH: update invoice
        [HttpPatch]
        [Route("tenants/{tenantId}/invoices/{invoiceId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateInvoiceAsync([FromBody] UpdateInvoiceDTO updateInvoiceDTO, Guid invoiceId)
        {
            // request validations
            if (updateInvoiceDTO == null) return BadRequest("Incorrect body format");

            // invoice validation
            ValidationResult validationResult = await _updateInvoiceDTOValidator.ValidateAsync(updateInvoiceDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // check if exists
            if (!await _invoiceService.CheckIfInvoiceExistsAsync(invoiceId)) return NotFound("Invoice not found");

            InvoiceDTO invoiceDTO = await _invoiceService.UpdateInvoiceAndLinesAsync(updateInvoiceDTO, User?.Identity?.Name, invoiceId);
            return Ok(invoiceDTO);
        }

        // DELETE: delete invoice
        [HttpDelete]
        [Route("tenants/{tenantId}/invoices/{invoiceId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid invoiceId)
        {
            // check if exists
            if (!await _invoiceService.CheckIfInvoiceExistsAsync(invoiceId)) return NotFound("Invoice not found");

            await _invoiceService.SetDeleteInvoiceAsync(invoiceId, true);

            return NoContent();
        }

        // POST: undelete invoice
        [HttpPost]
        [Route("tenants/{tenantId}/invoices/{invoiceId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid invoiceId)
        {
            // check if exists
            if (!await _invoiceService.CheckIfInvoiceExistsAsync(invoiceId)) return NotFound("Invoice not found");

            await _invoiceService.SetDeleteInvoiceAsync(invoiceId, false);

            return NoContent();
        }
    }
}
