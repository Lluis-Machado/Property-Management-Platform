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
    public class InvoicesController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IBusinessPartnerRepository _businessPartnerRepo;
        private readonly IValidator<Invoice> _invoiceValidator;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(IInvoiceRepository invoiceRepo, IBusinessPartnerRepository businessPartnerRepository, IValidator<Invoice> invoiceValidator, ILogger<InvoicesController> logger)
        {
            _invoiceValidator = invoiceValidator;
            _businessPartnerRepo = businessPartnerRepository;
            _invoiceRepo = invoiceRepo;
            _logger = logger;
        }

        // POST: Create Invoice

        [HttpPost]
        [Route("invoices")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] Invoice invoice, Guid businessPartnerId)
        {
            // request validations
            if (invoice == null) return BadRequest("Incorrect body format");
            if (invoice.Id != Guid.Empty) return BadRequest("Invoice Id field must be empty");
            if (invoice.BusinessPartnerId != businessPartnerId) return BadRequest("Incorrect BusinessPartner Id in body");

            // invoice validation
            ValidationResult validationResult = await _invoiceValidator.ValidateAsync(invoice);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // businessPartner validation
            if (!await BusinessPartnerExists(businessPartnerId)) return NotFound("BusinessPartner not found");

            invoice = await _invoiceRepo.InsertInvoiceAsync(invoice);
            return Created($"invoices/{invoice.Id}", invoice);
        }

        // GET: Get invoice(s)

        [HttpGet]
        [Route("invoices")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _invoiceRepo.GetInvoicesAsync(includeDeleted));
        }

        // POST: update invoice

        [HttpPost]
        [Route("invoices/{invoiceId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAsync([FromBody] Invoice invoice, Guid businessPartnerId, Guid invoiceId)
        {
            // request validations
            if (invoice == null) return BadRequest("Incorrect body format");
            if (invoice.Id != invoiceId) return BadRequest("Invoice Id from body incorrect");
            if (invoice.BusinessPartnerId != businessPartnerId) return BadRequest("Incorrect BusinessPartner Id in body");

            // invoice validation
            ValidationResult validationResult = await _invoiceValidator.ValidateAsync(invoice);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // businessPartner validation
            if (!await BusinessPartnerExists(businessPartnerId)) return NotFound("BusinessPartner not found");

            invoice.Id = invoiceId; // copy id to invoice object

            int result = await _invoiceRepo.UpdateInvoiceAsync(invoice);
            if (result == 0) return NotFound("Invoice not found");
            return NoContent();
        }

        // DELETE: delete invoice

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

        private async Task<bool> BusinessPartnerExists(Guid businessPartnerId)
        {
            BusinessPartner? businessPartner = await _businessPartnerRepo.GetBusinessPartnerByIdAsync(businessPartnerId);
            return (businessPartner != null && businessPartner?.Deleted == false);
        }
    }
}
