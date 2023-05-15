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
    public class InvoiceLinesController : Controller
    {
        private readonly IInvoiceLineRepository _invoiceLineRepo;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IExpenseTypeRepository _expenseTypeRepo;
        private readonly IValidator<InvoiceLine> _invoiceLineValidator;
        private readonly ILogger<InvoiceLinesController> _logger;

        public InvoiceLinesController(IInvoiceLineRepository invoiceLineRepository, IInvoiceRepository invoiceRepository, IExpenseTypeRepository expenseTypeRepository, IValidator<InvoiceLine> invoiceLineValidator, ILogger<InvoiceLinesController> logger)
        {
            _invoiceLineRepo = invoiceLineRepository;
            _invoiceRepo = invoiceRepository;
            _expenseTypeRepo = expenseTypeRepository;
            _invoiceLineValidator = invoiceLineValidator;
            _logger = logger;
        }

        // POST: Create invoiceLine

        [HttpPost]
        [Route("invoiceLines")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] InvoiceLine invoiceLine, Guid invoiceId, Guid expenseTypeId)
        {
            // request validations
            if (invoiceLine == null) return BadRequest("Incorrect body format");
            if (invoiceLine.Id != Guid.Empty) return BadRequest("InvoiceLine Id field must be empty");
            if (invoiceLine.InvoiceId != invoiceId) return BadRequest("Incorrect Invoice Id in body");
            if (invoiceLine.ExpenseTypeId != expenseTypeId) return BadRequest("Incorrect ExpenseType Id in body");

            // invoiceLine validation
            ValidationResult validationResult = await _invoiceLineValidator.ValidateAsync(invoiceLine);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // invoice validator
            if (!await InvoiceExists(invoiceId)) return NotFound("Invoice not found");

            // expenseType validator
            if (!await ExpenseTypeExists(expenseTypeId)) return NotFound("ExpenseType not found");

            invoiceLine = await _invoiceLineRepo.InsertInvoiceLineAsync(invoiceLine);
            return Created($"invoiceLines/{invoiceLine.Id}", invoiceLine);
        }

        // GET: Get invoiceLine(s)
        [HttpGet]
        [Route("invoiceLines")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<InvoiceLine>>> GetAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _invoiceLineRepo.GetInvoiceLinesAsync(includeDeleted));
        }

        // POST: update invoiceLine

        [HttpPost]
        [Route("invoiceLines/{invoiceLineId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAsync([FromBody] InvoiceLine invoiceLine, Guid invoiceId, Guid expenseTypeId, Guid invoiceLineId)
        {
            // request validations
            if (invoiceLine == null) return BadRequest("Incorrect body format");
            if (invoiceLine.Id != invoiceLineId) return BadRequest("InvoiceLine Id from body incorrect");

            // invoiceLine validation
            ValidationResult validationResult = await _invoiceLineValidator.ValidateAsync(invoiceLine);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // invoice validator
            if (!await InvoiceExists(invoiceId)) return NotFound("Invoice not found");

            // expenseType validator
            if (!await ExpenseTypeExists(expenseTypeId)) return NotFound("ExpenseType not found");

            invoiceLine.Id = invoiceLineId; // copy id to invoiceLine object

            int result = await _invoiceLineRepo.UpdateInvoiceLineAsync(invoiceLine);
            if (result == 0) return NotFound("InvoiceLine not found");
            return NoContent();
        }

        // DELETE: delete invoiceLine

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

        private async Task<bool> InvoiceExists(Guid invoiceId)
        {
            Invoice? invoice = await _invoiceRepo.GetInvoiceByIdAsync(invoiceId);
            return (invoice != null && invoice?.Deleted == false);
        }

        private async Task<bool> ExpenseTypeExists(Guid expenseTypeId)
        {
            ExpenseType? expenseType = await _expenseTypeRepo.GetExpenseTypeByIdAsync(expenseTypeId);
            return (expenseType != null && expenseType?.Deleted == false);
        }
    }
}
