using Accounting.Models;
using Accounting.Repositories;
using Accounting.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class FixedAssetsController : Controller
    {
        private readonly IFixedAssetRepository _fixedAssetRepo;
        private readonly IValidator<FixedAsset> _fixedAssetValidator; 
        public FixedAssetsController(IFixedAssetRepository fixedAssetRepo, IValidator<FixedAsset> fixedAssetValidator) 
        {
            _fixedAssetRepo = fixedAssetRepo;
            _fixedAssetValidator = fixedAssetValidator;
        }

        // POST: Create fixedAsset
        [HttpPost]
        [Route("fixedAssets")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAsync([FromBody] FixedAsset fixedAsset)
        {
            try
            {
                // validations
                if (fixedAsset == null) return BadRequest("Incorrect body format");
                if (fixedAsset.Id != Guid.Empty) return BadRequest("Id field must be empty");

                await _fixedAssetValidator.ValidateAndThrowAsync(fixedAsset);

                Guid fixedAssetId = await _fixedAssetRepo.InsertFixedAssetAsync(fixedAsset);
                return Ok(fixedAssetId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: Get fixedAsset(s)
        [HttpGet]
        [Route("fixedAssets")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<FixedAsset>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                IEnumerable<FixedAsset> fixedAssets = await _fixedAssetRepo.GetFixedAssetsAsync();
                return Ok(fixedAssets.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: update fixedAsset
        [HttpPost]
        [Route("fixedAssets/{fixedAssetId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAsync([FromBody] FixedAsset fixedAsset, Guid fixedAssetId)
        {
            try
            {
                // validations
                if (fixedAsset == null) return BadRequest("Incorrect body format");
                if (fixedAsset.Id != fixedAssetId) return BadRequest("fixedAssetId from body incorrect");
                fixedAsset.Id = fixedAssetId;

                await _fixedAssetValidator.ValidateAndThrowAsync(fixedAsset);

                int result = await _fixedAssetRepo.UpdateFixedAssetAsync(fixedAsset);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: delete fixedAsset
        [HttpDelete]
        [Route("fixedAssets/{fixedAssetId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync(Guid fixedAssetId)
        {
            try
            {
                FixedAsset fixedAsset = await _fixedAssetRepo.GetFixedAssetByIdAsync(fixedAssetId);
                fixedAsset.Deleted = true;
                int result = await _fixedAssetRepo.UpdateFixedAssetAsync(fixedAsset);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: undelete fixedAsset
        [HttpPost]
        [Route("fixedAssets/{fixedAssetId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UndeleteAsync(Guid fixedAssetId)
        {
            try
            {
                FixedAsset fixedAsset = await _fixedAssetRepo.GetFixedAssetByIdAsync(fixedAssetId);
                fixedAsset.Deleted = false;
                int result = await _fixedAssetRepo.UpdateFixedAssetAsync(fixedAsset);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
