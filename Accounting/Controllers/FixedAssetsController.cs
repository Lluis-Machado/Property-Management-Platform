using Accounting.Models;
using Accounting.Repositories;
using Accounting.Security;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class FixedAssetsController : Controller
    {
        private readonly IFixedAssetRepository _fixedAssetRepo;
        private readonly IValidator<FixedAsset> _fixedAssetValidator;
        private readonly ILogger<FixedAssetsController> _logger;

        public FixedAssetsController(IFixedAssetRepository fixedAssetRepo, IValidator<FixedAsset> fixedAssetValidator, ILogger<FixedAssetsController> logger)
        {
            _fixedAssetRepo = fixedAssetRepo;
            _fixedAssetValidator = fixedAssetValidator;
            _logger = logger;
        }

        // POST: Create fixedAsset
        [Authorize]
        [HttpPost]
        [Route("fixedAssets")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] FixedAsset fixedAsset)
        {
            // request validations
            if (fixedAsset == null) return BadRequest("Incorrect body format");
            if (fixedAsset.Id != Guid.Empty) return BadRequest("FixedAsset Id field must be empty");

            // fixedAsset validation
            ValidationResult validationResult = await _fixedAssetValidator.ValidateAsync(fixedAsset);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            fixedAsset = await _fixedAssetRepo.InsertFixedAssetAsync(fixedAsset);
            return Created($"fixedAssets/{fixedAsset.Id}", fixedAsset);
        }

        // GET: Get fixedAsset(s)
        [Authorize]
        [HttpGet]
        [Route("fixedAssets")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<FixedAsset>>> GetAsync()
        {
            return Ok(await _fixedAssetRepo.GetFixedAssetsAsync());
        }

        // POST: update fixedAsset
        [Authorize]
        [HttpPost]
        [Route("fixedAssets/{fixedAssetId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAsync([FromBody] FixedAsset fixedAsset, Guid fixedAssetId)
        {
            // request validations
            if (fixedAsset == null) return BadRequest("Incorrect body format");
            if (fixedAsset.Id != fixedAssetId) return BadRequest("fixedAsset Id from body incorrect");

            // fixedAsset validation
            ValidationResult validationResult = await _fixedAssetValidator.ValidateAsync(fixedAsset);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            fixedAsset.Id = fixedAssetId; // copy id to fixedAsset object

            await _fixedAssetValidator.ValidateAndThrowAsync(fixedAsset);

            int result = await _fixedAssetRepo.UpdateFixedAssetAsync(fixedAsset);
            if (result == 0) return BadRequest("fixedAsset not found");
            return NoContent();
        }

        // DELETE: delete fixedAsset
        [HttpDelete]
        [Authorize]
        [Route("fixedAssets/{fixedAssetId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid fixedAssetId)
        {
            int result = await _fixedAssetRepo.SetDeleteFixedAssetAsync(fixedAssetId, true);
            if (result == 0) return NotFound("fixedAsset not found");
            return NoContent();
        }

        // POST: undelete fixedAsset
        [Authorize]
        [HttpPost]
        [Route("fixedAssets/{fixedAssetId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid fixedAssetId)
        {
            int result = await _fixedAssetRepo.SetDeleteFixedAssetAsync(fixedAssetId, false);
            if (result == 0) return NotFound("fixedAsset not found");
            return NoContent();
        }
    }
}
