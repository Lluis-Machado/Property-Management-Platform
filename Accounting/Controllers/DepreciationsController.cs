using Accounting.Models;
using Accounting.Repositories;
using Accounting.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class DepreciationsController : Controller
    {
        private readonly IDepreciationRepository _DepreciationRepo;
        private readonly IValidator<Depreciation> _DepreciationValidator;
        public DepreciationsController(IDepreciationRepository DepreciationRepo, IValidator<Depreciation> DepreciationValidator)
        {
            _DepreciationRepo = DepreciationRepo;
            _DepreciationValidator = DepreciationValidator;
        }

        // POST: Create Depreciation
        [HttpPost]
        [Route("Depreciations")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAsync([FromBody] Depreciation Depreciation)
        {
            try
            {
                // validations
                if (Depreciation == null) return BadRequest("Incorrect body format");
                if (Depreciation.Id != Guid.Empty) return BadRequest("Id field must be empty");

                await _DepreciationValidator.ValidateAndThrowAsync(Depreciation);

                Guid DepreciationId = await _DepreciationRepo.InsertDepreciationAsync(Depreciation);
                return Ok(DepreciationId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: Get Depreciation(s)
        [HttpGet]
        [Route("Depreciations")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<Depreciation>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                IEnumerable<Depreciation> Depreciations = await _DepreciationRepo.GetDepreciationsAsync();
                return Ok(Depreciations.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: update Depreciation
        [HttpPost]
        [Route("Depreciations/{DepreciationId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAsync([FromBody] Depreciation Depreciation, Guid DepreciationId)
        {
            try
            {
                // validations
                if (Depreciation == null) return BadRequest("Incorrect body format");
                if (Depreciation.Id != DepreciationId) return BadRequest("DepreciationId from body incorrect");
                Depreciation.Id = DepreciationId;

                await _DepreciationValidator.ValidateAndThrowAsync(Depreciation);

                int result = await _DepreciationRepo.UpdateDepreciationAsync(Depreciation);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: delete Depreciation
        [HttpDelete]
        [Route("Depreciations/{DepreciationId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync(Guid DepreciationId)
        {
            try
            {
                Depreciation Depreciation = await _DepreciationRepo.GetDepreciationByIdAsync(DepreciationId);
                Depreciation.Deleted = true;
                int result = await _DepreciationRepo.UpdateDepreciationAsync(Depreciation);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: undelete Depreciation
        [HttpPost]
        [Route("Depreciations/{DepreciationId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UndeleteAsync(Guid DepreciationId)
        {
            try
            {
                Depreciation Depreciation = await _DepreciationRepo.GetDepreciationByIdAsync(DepreciationId);
                Depreciation.Deleted = false;
                int result = await _DepreciationRepo.UpdateDepreciationAsync(Depreciation);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
