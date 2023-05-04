using Accounting.Models;
using Accounting.Repositories;
using Accounting.Security;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class InvoiceLinesController : Controller
    {
        private readonly IInvoiceLineRepository _invoiceLineRepo;
        private readonly IValidator<InvoiceLine> _invoiceLineValidator;
        private readonly ILogger<InvoiceLinesController> _logger;

        public InvoiceLinesController(IInvoiceLineRepository invoiceLineRepository, IValidator<InvoiceLine> invoiceLineValidator, ILogger<InvoiceLinesController> logger)
        {
            _invoiceLineRepo = invoiceLineRepository;
            _invoiceLineValidator = invoiceLineValidator;
            _logger = logger;
        }

        // POST: Create invoiceLine
        [Authorize]
        [HttpPost]
        [Route("invoiceLines")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] InvoiceLine invoiceLine)
        {
            // request validations
            if (invoiceLine == null) return BadRequest("Incorrect body format");
            if (invoiceLine.Id != Guid.Empty) return BadRequest("invoiceLine Id field must be empty");

            // invoiceLine validation
            ValidationResult validationResult = await _invoiceLineValidator.ValidateAsync(invoiceLine);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            await _invoiceLineValidator.ValidateAndThrowAsync(invoiceLine);

            invoiceLine = await _invoiceLineRepo.InsertInvoiceLineAsync(invoiceLine);
            return Created($"invoiceLines/{invoiceLine.Id}", invoiceLine);
        }

        // GET: Get invoiceLine(s)
        [HttpGet]
        [Route("invoiceLines")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<InvoiceLine>>> GetAsync()
        {
            return Ok(await _invoiceLineRepo.GetInvoiceLinesAsync());
        }

        // POST: update invoiceLine
        [Authorize]
        [HttpPost]
        [Route("invoiceLines/{invoiceLineId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAsync([FromBody] InvoiceLine invoiceLine, Guid invoiceLineId)
        {
            // request validations
            if (invoiceLine == null) return BadRequest("Incorrect body format");
            if (invoiceLine.Id != invoiceLineId) return BadRequest("invoiceLine Id from body incorrect");

            // invoiceLine validation
            ValidationResult validationResult = await _invoiceLineValidator.ValidateAsync(invoiceLine);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            invoiceLine.Id = invoiceLineId; // copy id to invoiceLine object

            int result = await _invoiceLineRepo.UpdateInvoiceLineAsync(invoiceLine);
            if (result == 0) return NotFound("InvoiceLine not found");
            return NoContent();
        }

        // DELETE: delete invoiceLine
        [Authorize]
        [HttpDelete]
        [Route("invoiceLines/{invoiceLineId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid invoiceLineId)
        {
            int result = await _invoiceLineRepo.SetDeleteInvoiceLineAsync(invoiceLineId, true);
            if (result == 0) return NotFound("InvoiceLine not found");
            return NoContent();
        }

        // POST: undelete invoiceLine
        [Authorize]
        [HttpPost]
        [Route("invoiceLines/{invoiceLineId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid invoiceLineId)
        {
            int result = await _invoiceLineRepo.SetDeleteInvoiceLineAsync(invoiceLineId, false);
            if (result == 0) return NotFound("InvoiceLine not found");
            return NoContent();
        }
    }
}
