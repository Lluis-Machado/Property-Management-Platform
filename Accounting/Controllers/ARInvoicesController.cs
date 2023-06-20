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
    public class ARInvoicesController : Controller
    {
        private readonly IARInvoiceService _arInvoiceService;
        private readonly IValidator<CreateARInvoiceDTO> _createARInvoiceDTOValidator;
        private readonly IValidator<UpdateARInvoiceDTO> _updateARInvoiceDTOValidator;
        private readonly ILogger<ARInvoicesController> _logger;

        public ARInvoicesController(IARInvoiceService arInvoiceService, IValidator<CreateARInvoiceDTO> createARInvoiceDTOValidator, ILogger<ARInvoicesController> logger, IValidator<UpdateARInvoiceDTO> updateARInvoiceDTOValidator)
        {
            _createARInvoiceDTOValidator = createARInvoiceDTOValidator;
            _arInvoiceService = arInvoiceService;
            _logger = logger;
            _updateARInvoiceDTOValidator = updateARInvoiceDTOValidator;
        }

        // POST: Create ARInvoice
        [HttpPost]
        [Route("tenants/{tenantId}/businesspartners/{businessPartnerId}/arinvoices")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ARInvoiceDTO>> CreateARInvoiceAsync([FromBody] CreateARInvoiceDTO createARInvoiceDTO,Guid tenantId, Guid businessPartnerId)
        {
            // request validations
            if (createARInvoiceDTO == null) return BadRequest("Incorrect body format");

            // invoice validation
            ValidationResult validationResult = await _createARInvoiceDTOValidator.ValidateAsync(createARInvoiceDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            ARInvoiceDTO invoiceDTO = await _arInvoiceService.CreateARInvoiceAndLinesAsync(createARInvoiceDTO, User?.Identity?.Name, businessPartnerId);
            return Created($"arinvoices/{invoiceDTO.Id}", invoiceDTO);
        }

        // GET: Get AP invoice(s)
        [HttpGet]
        [Route("tenants/{tenantId}/arinvoices")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ARInvoiceDTO>>> GetARInvoicesAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _arInvoiceService.GetARInvoicesAsync(includeDeleted));
        }

        // PATCH: update invoice
        [HttpPatch]
        [Route("tenants/{tenantId}/arinvoices/{invoiceId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateARInvoiceAsync([FromBody] UpdateARInvoiceDTO updateARInvoiceDTO, Guid invoiceId)
        {
            // request validations
            if (updateARInvoiceDTO == null) return BadRequest("Incorrect body format");

            // invoice validation
            ValidationResult validationResult = await _updateARInvoiceDTOValidator.ValidateAsync(updateARInvoiceDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // check if exists
            if (!await _arInvoiceService.CheckIfARInvoiceExistsAsync(invoiceId)) return NotFound("Invoice not found");

            ARInvoiceDTO invoiceDTO = await _arInvoiceService.UpdateARInvoiceAndLinesAsync(updateARInvoiceDTO, User?.Identity?.Name, invoiceId);
            return Ok(invoiceDTO);
        }

        // DELETE: delete ap invoice
        [HttpDelete]
        [Route("tenants/{tenantId}/arinvoices/{invoiceId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid invoiceId)
        {
            // check if exists
            if (!await _arInvoiceService.CheckIfARInvoiceExistsAsync(invoiceId)) return NotFound("Invoice not found");

            await _arInvoiceService.SetDeletedARInvoiceAsync(invoiceId, true);

            return NoContent();
        }

        // POST: undelete invoice
        [HttpPost]
        [Route("tenants/{tenantId}/arinvoices/{invoiceId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid invoiceId)
        {
            // check if exists
            if (!await _arInvoiceService.CheckIfARInvoiceExistsAsync(invoiceId)) return NotFound("Invoice not found");

            await _arInvoiceService.SetDeletedARInvoiceAsync(invoiceId, false);

            return NoContent();
        }
    }
}
