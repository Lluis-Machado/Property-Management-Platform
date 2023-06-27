using AccountingAPI.DTOs;
using AccountingAPI.Models;
using AccountingAPI.Services;
using AccountingAPI.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountingAPI.Controllers
{
    [Authorize]
    public class ExpenseCategoriesController : Controller
    {
        private readonly IExpenseCategoryService _expenseCategoryService;
        private readonly ILogger<ExpenseCategoriesController> _logger;

        public ExpenseCategoriesController(IExpenseCategoryService expenseCategoryRepository, ILogger<ExpenseCategoriesController> logger)
        {
            _expenseCategoryService = expenseCategoryRepository;
            _logger = logger;
        }

        // POST: Create expenseCategory
        [HttpPost]
        [Route("expenseCategories")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ExpenseCategoryDTO>> CreateExpenseCategoryAsync([FromBody] CreateExpenseCategoryDTO createExpenseCategoryDTO)
        {
            // request validations
            if (createExpenseCategoryDTO == null) return BadRequest("Incorrect body format");

            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            ExpenseCategoryDTO expenseCategoryDTO = await _expenseCategoryService.CreateExpenseCategoryAsync(createExpenseCategoryDTO, userName);

            return Created($"expenseCategories/{expenseCategoryDTO.Id}", expenseCategoryDTO);
        }

        // GET: Get expenseCategory(ies)
        [HttpGet]
        [Route("expenseCategories")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ExpenseCategory>>> GetExpenseCategoriesAsync(Guid tenantId, [FromQuery] bool includeDeleted = false)
        {
            return Ok(await _expenseCategoryService.GetExpenseCategoriesAsync(includeDeleted));
        }

        // PATCH: Update expenseCategory
        [HttpPatch]
        [Route("expenseCategories/{expenseCategoryId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ExpenseCategoryDTO>> UpdateExpenseCategoryAsync(Guid tenantId, Guid expenseCategoryId, [FromBody] UpdateExpenseCategoryDTO updateExpenseCategoryDTO)
        {
            // request validations
            if (updateExpenseCategoryDTO == null) return BadRequest("Incorrect body format");

            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            ExpenseCategoryDTO expenseCategoryDTO = await _expenseCategoryService.UpdateExpenseCategoryAsync(expenseCategoryId, updateExpenseCategoryDTO, userName);

            return Ok(expenseCategoryDTO);
        }

        // DELETE: delete expenseCategory
        [HttpDelete]
        [Route("expenseCategories/{expenseCategoryId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteExpenseCategoryAsync(Guid expenseCategoryId)
        {
            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            await _expenseCategoryService.SetDeletedExpenseCategoryAsync(expenseCategoryId, true, userName);

            return NoContent();
        }

        // POST: undelete expenseCategory
        [HttpPost]
        [Route("expenseCategories/{expenseCategoryId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteExpenseCategoryAsync(Guid expenseCategoryId)
        {        
            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            await _expenseCategoryService.SetDeletedExpenseCategoryAsync(expenseCategoryId, false, userName);

            return NoContent();
        }

    }
}
