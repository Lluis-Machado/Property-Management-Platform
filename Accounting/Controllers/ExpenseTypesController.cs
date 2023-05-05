using Accounting.Models;
using Accounting.Repositories;
using Accounting.Security;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class ExpenseTypesController : Controller
    {
        private readonly IExpenseTypeRepository _expenseTypeRepo;
        private readonly IValidator<ExpenseType> _expenseTypeValidator;
        private readonly ILogger<ExpenseTypesController> _logger;

        public ExpenseTypesController(IExpenseTypeRepository expenseTypeRepository, IValidator<ExpenseType> expenseTypeValidator, ILogger<ExpenseTypesController> logger)
        {
            _expenseTypeRepo = expenseTypeRepository;
            _expenseTypeValidator = expenseTypeValidator;
            _logger = logger;
        }

        // POST: Create expenseType
        [Authorize]
        [HttpPost]
        [Route("expenseTypes")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] ExpenseType expenseType)
        {
            // request validations
            if (expenseType == null) return BadRequest("Incorrect body format");
            if (expenseType.Id != Guid.Empty) return BadRequest("ExpenseType Id field must be empty");

            // expenseType validation
            ValidationResult validationResult = await _expenseTypeValidator.ValidateAsync(expenseType);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            expenseType = await _expenseTypeRepo.InsertExpenseTypeAsync(expenseType);
            return Created($"expenseTypes/{expenseType.Id}", expenseType);
        }

        // GET: Get expenseType(s)
        [Authorize]
        [HttpGet]
        [Route("expenseTypes")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ExpenseType>>> GetAsync()
        {
            return Ok(await _expenseTypeRepo.GetExpenseTypesAsync());
        }

        // POST: update expenseType
        [Authorize]
        [HttpPost]
        [Route("expenseTypes/{expenseTypeId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAsync([FromBody] ExpenseType expenseType, Guid expenseTypeId)
        {
            // request validations
            if (expenseType == null) return BadRequest("Incorrect body format");
            if (expenseType.Id != expenseTypeId) return BadRequest("ExpenseType Id from body incorrect");

            // expenseType validation
            ValidationResult validationResult = await _expenseTypeValidator.ValidateAsync(expenseType);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            expenseType.Id = expenseTypeId; // copy id to ExpenseType object

            int result = await _expenseTypeRepo.UpdateExpenseTypeAsync(expenseType);
            if (result == 0) return NotFound("ExpenseType not found");
            return NoContent();
        }

        // DELETE: delete expenseType
        [Authorize]
        [HttpDelete]
        [Route("expenseTypes/{expenseTypeId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid expenseTypeId)
        {
            int result = await _expenseTypeRepo.SetDeleteExpenseTypeAsync(expenseTypeId, true);
            if (result == 0) return NotFound("ExpenseType not found");
            return NoContent();
        }

        // POST: undelete expenseType
        [Authorize]
        [HttpPost]
        [Route("expenseTypes/{expenseTypeId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid expenseTypeId)
        {
            int result = await _expenseTypeRepo.SetDeleteExpenseTypeAsync(expenseTypeId, false);
            if (result == 0) return NotFound("ExpenseType not found");
            return NoContent();
        }

    }
}
