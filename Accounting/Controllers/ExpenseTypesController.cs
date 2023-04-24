using Accounting.Models;
using Accounting.Repositories;
using Accounting.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class ExpenseTypesController : Controller
    {
        private readonly IExpenseTypeRepository _expenseTypeRepo;
        private readonly IValidator<ExpenseType> _expenseTypeValidator;
        public ExpenseTypesController(IExpenseTypeRepository expenseTypeRepository, IValidator<ExpenseType> expenseTypeValidator) 
        {
            _expenseTypeRepo = expenseTypeRepository;
            _expenseTypeValidator = expenseTypeValidator;
        }

        // POST: Create expenseType
        [HttpPost]
        [Route("expenseTypes")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAsync([FromBody] ExpenseType expenseType)
        {
            try
            {
                // validations
                if (expenseType == null) return BadRequest("Incorrect body format");
                if (expenseType.Id != Guid.Empty) return BadRequest("Id field must be empty");

                await _expenseTypeValidator.ValidateAndThrowAsync(expenseType);

                Guid expenseTypeId = await _expenseTypeRepo.InsertExpenseTypeAsync(expenseType);
                return Ok(expenseTypeId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: Get expenseType(s)
        [HttpGet]
        [Route("expenseTypes")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<ExpenseType>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                IEnumerable<ExpenseType> expenseTypes = await _expenseTypeRepo.GetExpenseTypesAsync();
                return Ok(expenseTypes.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: update expenseType
        [HttpPost]
        [Route("expenseTypes/{expenseTypeId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAsync([FromBody] ExpenseType expenseType, Guid expenseTypeId)
        {
            try
            {
                // validations
                if (expenseType == null) return BadRequest("Incorrect body format");
                if (expenseType.Id != expenseTypeId) return BadRequest("expenseTypeId from body incorrect");
                expenseType.Id = expenseTypeId;

                await _expenseTypeValidator.ValidateAndThrowAsync(expenseType);

                int result = await _expenseTypeRepo.UpdateExpenseTypeAsync(expenseType);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: delete expenseType
        [HttpDelete]
        [Route("expenseTypes/{expenseTypeId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync(Guid expenseTypeId)
        {
            try
            {
                ExpenseType expenseType = await _expenseTypeRepo.GetExpenseTypeByIdAsync(expenseTypeId);
                expenseType.Deleted = true;
                int result = await _expenseTypeRepo.UpdateExpenseTypeAsync(expenseType);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: undelete expenseType
        [HttpPost]
        [Route("expenseTypes/{expenseTypeId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UndeleteAsync(Guid expenseTypeId)
        {
            try
            {
                ExpenseType expenseType = await _expenseTypeRepo.GetExpenseTypeByIdAsync(expenseTypeId);
                expenseType.Deleted = false;
                int result = await _expenseTypeRepo.UpdateExpenseTypeAsync(expenseType);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
