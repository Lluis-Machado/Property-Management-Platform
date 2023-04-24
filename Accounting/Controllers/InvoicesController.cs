using Accounting.Models;
using Accounting.Repositories;
using Accounting.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IValidator<Invoice> _invoiceValidator;

        public InvoicesController(IInvoiceRepository invoiceRepo, IValidator<Invoice> invoiceValidator) 
        {
            _invoiceValidator = invoiceValidator;    
            _invoiceRepo = invoiceRepo;
        }

        // POST: Create Invoice
        [HttpPost]
        [Route("invoices")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAsync([FromBody] Invoice invoice)
        {
            try
            {
                // validations
                if (invoice == null) return BadRequest("Incorrect body format");
                if (invoice.Id != Guid.Empty) return BadRequest("Id field must be empty");

                await _invoiceValidator.ValidateAndThrowAsync(invoice);

                Guid invoiceId = await _invoiceRepo.InsertInvoiceAsync(invoice);
                return Ok(invoiceId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: Get invoice(s)
        [HttpGet]
        [Route("invoices")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<Invoice>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                IEnumerable<Invoice> invoices = await _invoiceRepo.GetInvoicesAsync();
                return Ok(invoices.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: update invoice
        [HttpPost]
        [Route("invoices/{invoiceId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAsync([FromBody] Invoice invoice, Guid invoiceId)
        {
            try
            {
                // validations
                if (invoice == null) return BadRequest("Incorrect body format");
                if (invoice.Id != invoiceId) return BadRequest("id from body incorrect");
                invoice.Id = invoiceId;

                await _invoiceValidator.ValidateAndThrowAsync(invoice);

                int result = await _invoiceRepo.UpdateInvoiceAsync(invoice);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: delete invoice
        [HttpDelete]
        [Route("invoices/{invoiceId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync(Guid invoiceId)
        {
            try
            {
                Invoice invoice = await _invoiceRepo.GetInvoiceByIdAsync(invoiceId);
                invoice.Deleted = true;
                int result = await _invoiceRepo.UpdateInvoiceAsync(invoice);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: undelete invoice
        [HttpPost]
        [Route("invoices/{invoiceId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UndeleteAsync(Guid invoiceId)
        {
            try
            {
                Invoice invoice = await _invoiceRepo.GetInvoiceByIdAsync(invoiceId);
                invoice.Deleted = false;
                int result = await _invoiceRepo.UpdateInvoiceAsync(invoice);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
