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
    public class ExpenseCategoriesController : Controller
    {
        private readonly IExpenseCategoryRepository _expenseCategoryRepo;
        private readonly IValidator<ExpenseCategory> _expenseCategoryValidator;
        private readonly ILogger<ExpenseCategoriesController> _logger;

        public ExpenseCategoriesController(IExpenseCategoryRepository expenseCategoryRepository, IValidator<ExpenseCategory> expenseCategoryValidator, ILogger<ExpenseCategoriesController> logger)
        {
            _expenseCategoryRepo = expenseCategoryRepository;
            _expenseCategoryValidator = expenseCategoryValidator;
            _logger = logger;
        }

        // POST: Create expenseCategory
        [HttpPost]
        [Route("expenseCategories")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] ExpenseCategory expenseCategory)
        {
            // request validations
            if (expenseCategory == null) return BadRequest("Incorrect body format");
            if (expenseCategory.Id != Guid.Empty) return BadRequest("ExpenseType Id field must be empty");

            // expenseCategory validation
            ValidationResult validationResult = await _expenseCategoryValidator.ValidateAsync(expenseCategory);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            expenseCategory = await _expenseCategoryRepo.InsertExpenseCategoryAsync(expenseCategory);
            return Created($"expenseCategories/{expenseCategory.Id}", expenseCategory);
        }

        // GET: Get expenseCategory(s)
        [HttpGet]
        [Route("expenseCategories")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ExpenseCategory>>> GetAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _expenseCategoryRepo.GetExpenseCategoriesAsync(includeDeleted));
            return Ok(await _expenseCategoryRepo.GetExpenseCategoriesAsync(includeDeleted));
        }

        // PATCH: update expenseCategory
        [HttpPatch]
        [Route("expenseCategories/{expenseCategoryId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAsync([FromBody] ExpenseCategory expenseCategory, Guid expenseCategoryId)
        {
            // request validations
            if (expenseCategory == null) return BadRequest("Incorrect body format");
            if (expenseCategory.Id != expenseCategoryId) return BadRequest("ExpenseType Id from body incorrect");

            // expenseCategory validation
            ValidationResult validationResult = await _expenseCategoryValidator.ValidateAsync(expenseCategory);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            expenseCategory.Id = expenseCategoryId; // copy id to ExpenseType object

            int result = await _expenseCategoryRepo.UpdateExpenseCategoryAsync(expenseCategory);
            if (result == 0) return NotFound("ExpenseType not found");
            return NoContent();
        }

        // DELETE: delete expenseCategory
        [HttpDelete]
        [Route("expenseCategories/{expenseCategoryId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid expenseCategoryId)
        {
            int result = await _expenseCategoryRepo.SetDeleteExpenseCategoryAsync(expenseCategoryId, true);
            if (result == 0) return NotFound("ExpenseType not found");
            return NoContent();
        }

        // POST: undelete expenseCategory
        [HttpPost]
        [Route("expenseCategories/{expenseCategoryId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid expenseCategoryId)
        {
            int result = await _expenseCategoryRepo.SetDeleteExpenseCategoryAsync(expenseCategoryId, false);
            if (result == 0) return NotFound("ExpenseType not found");
            return NoContent();
        }

    }
}
