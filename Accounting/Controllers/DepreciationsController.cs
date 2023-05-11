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
        private readonly IFixedAssetRepository _fixedAssetRepo;
        private readonly IValidator<Depreciation> _depreciationValidator;
        private readonly ILogger<DepreciationsController> _logger;
        public DepreciationsController(IDepreciationRepository depreciationRepo, IFixedAssetRepository fixedAssetRepository, IValidator<Depreciation> depreciationValidator, ILogger<DepreciationsController> logger)
        {
            _depreciationRepo = depreciationRepo;
            _fixedAssetRepo = fixedAssetRepository;
            _depreciationValidator = depreciationValidator;
            _logger = logger;
        }

        // POST: Create depreciation
        [Authorize]
        [HttpPost]
        [Route("depreciations")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] Depreciation depreciation, Guid fixedAssetId)
        {
            // request validations
            if (depreciation == null) return BadRequest("Incorrect body format");
            if (depreciation.Id != Guid.Empty) return BadRequest("Depreciation Id field must be empty");
            if (depreciation.FixedAssetId != fixedAssetId) return BadRequest("Incorrect FixedAsset id in body");

            // depreciation validation
            ValidationResult validationResult = await _depreciationValidator.ValidateAsync(depreciation);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // fixedAsset validation
            if (!await FixedAssetExists(fixedAssetId)) return NotFound("FixedAsset not found");

            await _depreciationValidator.ValidateAndThrowAsync(depreciation);

            depreciation = await _depreciationRepo.InsertDepreciationAsync(depreciation);
            return Created($"depreciations/{depreciation.Id}", depreciation);
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
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAsync([FromBody] Depreciation depreciation, Guid fixedAssetId, Guid depreciationId)
        {
            // request validations
            if (depreciation == null) return BadRequest("Incorrect body format");
            if (depreciation.Id != depreciationId) return BadRequest("Depreciation Id from body incorrect");

            // depreciation validation
            ValidationResult validationResult = await _depreciationValidator.ValidateAsync(depreciation);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // fixedAsset validation
            if (!await FixedAssetExists(fixedAssetId)) return NotFound("FixedAsset not found");

            depreciation.Id = depreciationId; // copy id to depreciation object

            int result = await _depreciationRepo.UpdateDepreciationAsync(depreciation);
            if (result == 0) return NotFound("Depreciation not found");
            return NoContent();
        }

        // DELETE: delete depreciation
        [HttpDelete]
        [Route("depreciations/{depreciationId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid depreciationId)
        {
            int result = await _depreciationRepo.SetDeleteDepreciationAsync(depreciationId, true);
            if (result == 0) return NotFound("Depreciation not found");
            return NoContent();
        }

        // POST: undelete depreciation
        [HttpPost]
        [Route("depreciations/{depreciationId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid depreciationId)
        {
            int result = await _depreciationRepo.SetDeleteDepreciationAsync(depreciationId, false);
            if (result == 0) return NotFound("Depreciation not found");
            return NoContent();
        }

        private async Task<bool> FixedAssetExists(Guid fixedAssetId)
        {
            FixedAsset? fixedAsset = await _fixedAssetRepo.GetFixedAssetByIdAsync(fixedAssetId);
            return (fixedAsset != null);
        }
    }
}
