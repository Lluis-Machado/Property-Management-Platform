using Accounting.Models;
using Accounting.Repositories;
using Accounting.Security;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class DepreciationsController : Controller
    {
        private readonly IDepreciationRepository _depreciationRepo;
        private readonly IValidator<Depreciation> _depreciationValidator;
        private readonly ILogger<DepreciationsController> _logger;
        public DepreciationsController(IDepreciationRepository depreciationRepo, IValidator<Depreciation> depreciationValidator, ILogger<DepreciationsController> logger)
        {
            _depreciationRepo = depreciationRepo;
            _depreciationValidator = depreciationValidator;
            _logger = logger;
        }

        // POST: Create depreciation
        [Authorize]
        [HttpPost]
        [Route("depreciations")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] Depreciation depreciation)
        {
            // request validations
            if (depreciation == null) return BadRequest("Incorrect body format");
            if (depreciation.Id != Guid.Empty) return BadRequest("depreciation Id field must be empty");

            // depreciation validation
            ValidationResult validationResult = await _depreciationValidator.ValidateAsync(depreciation);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));


            await _depreciationValidator.ValidateAndThrowAsync(depreciation);

            return Ok(await _depreciationRepo.InsertDepreciationAsync(depreciation));
        }

        // GET: Get depreciation(s)
        [Authorize]
        [HttpGet]
        [Route("depreciations")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Depreciation>>> GetAsync()
        {
            return Ok(await _depreciationRepo.GetDepreciationsAsync());
        }

        // POST: update depreciation
        [Authorize]
        [HttpPost]
        [Route("depreciations/{depreciationId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> UpdateAsync([FromBody] Depreciation depreciation, Guid depreciationId)
        {
            // request validations
            if (depreciation == null) return BadRequest("Incorrect body format");
            if (depreciation.Id != depreciationId) return BadRequest("depreciation Id from body incorrect");

            // depreciation validation
            ValidationResult validationResult = await _depreciationValidator.ValidateAsync(depreciation);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            depreciation.Id = depreciationId; // copy id to depreciation object

            int result = await _depreciationRepo.UpdateDepreciationAsync(depreciation);
            if (result == 0) return NotFound("Depreciation not found");
            return Ok();
        }

        // DELETE: delete depreciation
        [HttpDelete]
        [Route("depreciations/{depreciationId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync(Guid depreciationId)
        {
            int result = await _depreciationRepo.SetDeleteDepreciationAsync(depreciationId, true);
            if (result == 0) return NotFound("Depreciation not found");
            return Ok();
        }

        // POST: undelete depreciation
        [HttpPost]
        [Route("depreciations/{depreciationId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UndeleteAsync(Guid depreciationId)
        {
            int result = await _depreciationRepo.SetDeleteDepreciationAsync(depreciationId, false);
            if (result == 0) return NotFound("Depreciation not found");
            return Ok();
        }
    }
}
