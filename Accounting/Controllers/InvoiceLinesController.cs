using Accounting.Models;
using Accounting.Repositories;
using Accounting.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class InvoiceLinesController : Controller
    {
        private readonly IInvoiceLineRepository _invoiceLineRepo;
        private readonly IValidator<InvoiceLine> _invoiceLineValidator;
        public InvoiceLinesController(IInvoiceLineRepository invoiceLineRepository, IValidator<InvoiceLine> invoiceLineValidator) 
        {
            _invoiceLineRepo = invoiceLineRepository;
            _invoiceLineValidator = invoiceLineValidator;
        } 

        // POST: Create invoiceLine
        [HttpPost]
        [Route("invoiceLines")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAsync([FromBody] InvoiceLine invoiceLine)
        {
            try
            {
                // validations
                if (invoiceLine == null) return BadRequest("Incorrect body format");
                if (invoiceLine.Id != Guid.Empty) return BadRequest("Id field must be empty");

                await _invoiceLineValidator.ValidateAndThrowAsync(invoiceLine);

                Guid businessPartnerId = await _invoiceLineRepo.InsertInvoiceLineAsync(invoiceLine);
                return Ok(businessPartnerId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: Get invoiceLine(s)
        [HttpGet]
        [Route("invoiceLines")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<InvoiceLine>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                IEnumerable<InvoiceLine> invoiceLines = await _invoiceLineRepo.GetInvoiceLinesAsync();
                return Ok(invoiceLines.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: update invoiceLine
        [HttpPost]
        [Route("invoiceLines/{invoiceLineId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAsync([FromBody] InvoiceLine invoiceLine, Guid invoiceLineId)
        {
            try
            {
                // validations
                if (invoiceLine == null) return BadRequest("Incorrect body format");
                if (invoiceLine.Id != invoiceLineId) return BadRequest("invoiceLineId from body incorrect");
                invoiceLine.Id = invoiceLineId;

                await _invoiceLineValidator.ValidateAndThrowAsync(invoiceLine);

                int result = await _invoiceLineRepo.UpdateInvoiceLineAsync(invoiceLine);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: delete invoiceLine
        [HttpDelete]
        [Route("invoiceLines/{invoiceLineId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync(Guid invoiceLineId)
        {
            try
            {
                InvoiceLine invoiceLine = await _invoiceLineRepo.GetInvoiceLineByIdAsync(invoiceLineId);
                invoiceLine.Deleted = true;
                int result = await _invoiceLineRepo.UpdateInvoiceLineAsync(invoiceLine);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: undelete invoiceLine
        [HttpPost]
        [Route("invoiceLines/{invoiceLineId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UndeleteAsync(Guid invoiceLineId)
        {
            try
            {
                InvoiceLine invoiceLine = await _invoiceLineRepo.GetInvoiceLineByIdAsync(invoiceLineId);
                invoiceLine.Deleted = false;
                int result = await _invoiceLineRepo.UpdateInvoiceLineAsync(invoiceLine);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
