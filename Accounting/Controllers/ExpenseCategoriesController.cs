using AccountingAPI.DTOs;
using AccountingAPI.Models;
using AccountingAPI.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountingAPI.Controllers
{
    [Authorize]
    public class ExpenseCategoriesController : Controller
    {
        private readonly IExpenseCategoryService _expenseCategoryService;
        private readonly IValidator<CreateExpenseCategoryDTO> _expenseCategoryValidator;
        private readonly ILogger<ExpenseCategoriesController> _logger;

        public ExpenseCategoriesController(IExpenseCategoryService expenseCategoryRepository, IValidator<CreateExpenseCategoryDTO> expenseCategoryValidator, ILogger<ExpenseCategoriesController> logger)
        {
            _expenseCategoryService = expenseCategoryRepository;
            _expenseCategoryValidator = expenseCategoryValidator;
            _logger = logger;
        }

        // POST: Create expenseCategory
        [HttpPost]
        [Route("tenants/{tenantId}/expenseCategories")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ExpenseCategoryDTO>> CreateExpenseCategoryAsync([FromBody] CreateExpenseCategoryDTO createExpenseCategoryDTO)
        {
            // request validations
            if (createExpenseCategoryDTO == null) return BadRequest("Incorrect body format");

            // expenseCategory validation
            ValidationResult validationResult = await _expenseCategoryValidator.ValidateAsync(createExpenseCategoryDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            ExpenseCategoryDTO expenseCategoryDTO = await _expenseCategoryService.CreateExpenseCategoryAsync(createExpenseCategoryDTO, User?.Identity?.Name);

            return Created($"expenseCategories/{expenseCategoryDTO.Id}", expenseCategoryDTO);
        }

        // GET: Get expenseCategory(s)
        [HttpGet]
        [Route("tenants/{tenantId}/expenseCategories")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ExpenseCategory>>> GetExpenseCategoriesAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _expenseCategoryService.GetExpenseCategoriesAsync(includeDeleted));
        }

        // PATCH: update expenseCategory
        [HttpPatch]
        [Route("tenants/{tenantId}/expenseCategories/{expenseCategoryId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ExpenseCategoryDTO>> UpdateExpenseCategoryAsync([FromBody] CreateExpenseCategoryDTO createExpenseCategoryDTO, Guid expenseCategoryId)
        {
            // request validations
            if (createExpenseCategoryDTO == null) return BadRequest("Incorrect body format");

            ValidationResult validationResult = await _expenseCategoryValidator.ValidateAsync(createExpenseCategoryDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // check if exists
            if (!await _expenseCategoryService.CheckIfExpenseCategoryExistsAsync(expenseCategoryId)) return NotFound("Expense Category not found");

            ExpenseCategoryDTO expenseCategoryDTO = await _expenseCategoryService.UpdateExpenseCategoryAsync(createExpenseCategoryDTO, User?.Identity?.Name, expenseCategoryId);

            return Ok(expenseCategoryDTO);
        }

        // DELETE: delete expenseCategory
        [HttpDelete]
        [Route("tenants/{tenantId}/expenseCategories/{expenseCategoryId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteExpenseCategoryAsync(Guid expenseCategoryId)
        {
            // check if exists
            if (!await _expenseCategoryService.CheckIfExpenseCategoryExistsAsync(expenseCategoryId)) return NotFound("Expense Category not found");

            await _expenseCategoryService.SetDeletedExpenseCategoryAsync(expenseCategoryId, true);

            return NoContent();
        }

        // POST: undelete expenseCategory
        [HttpPost]
        [Route("tenants/{tenantId}/expenseCategories/{expenseCategoryId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteExpenseCategoryAsync(Guid expenseCategoryId)
        {
            // check if exists
            if (!await _expenseCategoryService.CheckIfExpenseCategoryExistsAsync(expenseCategoryId)) return NotFound("Expense Category not found");

            await _expenseCategoryService.SetDeletedExpenseCategoryAsync(expenseCategoryId, false);

            return NoContent();
        }

    }
}
