using Accounting.Models;
using Accounting.Repositories;
using Accounting.Security;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IValidator<Invoice> _invoiceValidator;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(IInvoiceRepository invoiceRepo, IValidator<Invoice> invoiceValidator, ILogger<InvoicesController> logger)
        {
            _invoiceValidator = invoiceValidator;
            _invoiceRepo = invoiceRepo;
            _logger = logger;
        }

        // POST: Create Invoice
        [Authorize]
        [HttpPost]
        [Route("invoices")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] Invoice invoice)
        {
            // request validations
            if (invoice == null) return BadRequest("Incorrect body format");
            if (invoice.Id != Guid.Empty) return BadRequest("Id field must be empty");

            // invoice validation
            ValidationResult validationResult = await _invoiceValidator.ValidateAsync(invoice);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            invoice = await _invoiceRepo.InsertInvoiceAsync(invoice);
            return Created($"invoices/{invoice.Id}", invoice);
        }

        // GET: Get invoice(s)
        [Authorize]
        [HttpGet]
        [Route("invoices")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetAsync()
        {
            return Ok(await _invoiceRepo.GetInvoicesAsync());
        }

        // POST: update invoice
        [Authorize]
        [HttpPost]
        [Route("invoices/{invoiceId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAsync([FromBody] Invoice invoice, Guid invoiceId)
        {
            // request validations
            if (invoice == null) return BadRequest("Incorrect body format");
            if (invoice.Id != invoiceId) return BadRequest("id from body incorrect");

            // invoice validation
            ValidationResult validationResult = await _invoiceValidator.ValidateAsync(invoice);
            if (!validationResult.IsValid) return BadRequest("~");

            invoice.Id = invoiceId; // copy id to invoice object

            int result = await _invoiceRepo.UpdateInvoiceAsync(invoice);
            if (result == 0) return NotFound("Invoice not found");
            return NoContent();
        }

        // DELETE: delete invoice
        [Authorize]
        [HttpDelete]
        [Route("invoices/{invoiceId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid invoiceId)
        {
            int result = await _invoiceRepo.SetDeleteInvoiceAsync(invoiceId, true);
            if (result == 0) return NotFound("Invoice not found");
            return NoContent();
        }

        // POST: undelete invoice
        [Authorize]
        [HttpPost]
        [Route("invoices/{invoiceId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid invoiceId)
        {
            int result = await _invoiceRepo.SetDeleteInvoiceAsync(invoiceId, false);
            if (result == 0) return NotFound("Invoice not found");
            return NoContent();
        }
    }
}
