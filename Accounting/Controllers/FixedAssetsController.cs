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
    public class FixedAssetsController : Controller
    {
        private readonly IFixedAssetRepository _fixedAssetRepo;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IDepreciationConfigRepository _depreciationConfigRepo;
        private readonly IDepreciationRepository _depreciationRepo;
        private readonly IValidator<FixedAsset> _fixedAssetValidator;
        private readonly ILogger<FixedAssetsController> _logger;

        public FixedAssetsController(IFixedAssetRepository fixedAssetRepo, IInvoiceRepository invoiceRepository, IDepreciationConfigRepository depreciationConfigRepository, IDepreciationRepository depreciationRepository, IValidator<FixedAsset> fixedAssetValidator, ILogger<FixedAssetsController> logger)
        {
            _fixedAssetRepo = fixedAssetRepo;
            _invoiceRepo = invoiceRepository;
            _depreciationConfigRepo = depreciationConfigRepository;
            _depreciationRepo = depreciationRepository;
            _fixedAssetValidator = fixedAssetValidator;
            _logger = logger;
        }

        // POST: Create fixedAsset

        [HttpPost]
        [Route("fixedAssets")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] FixedAsset fixedAsset, Guid invoiceId, Guid depreciationConfigId)
        {
            // request validations
            if (fixedAsset == null) return BadRequest("Incorrect body format");
            if (fixedAsset.Id != Guid.Empty) return BadRequest("FixedAsset Id field must be empty");
            if (fixedAsset.InvoiceId != invoiceId) return BadRequest("Incorrect Invoice Id in body");

            // fixedAsset validation
            ValidationResult validationResult = await _fixedAssetValidator.ValidateAsync(fixedAsset);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // invoice validation
            if (!await InvoiceExists(invoiceId)) return NotFound("Invoice not found");

            // depreciationConfig validation
            if (!await DepreciationConfigExists(depreciationConfigId)) return NotFound("DepreciationConfig not found");

            fixedAsset = await _fixedAssetRepo.InsertFixedAssetAsync(fixedAsset);
            return Created($"fixedAssets/{fixedAsset.Id}", fixedAsset);
        }

        // GET: Get fixedAsset(s)

        [HttpGet]
        [Route("fixedAssets")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<FixedAsset>>> GetAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _fixedAssetRepo.GetFixedAssetsAsync(includeDeleted));
        }

        // GET: Get fixedAsset(s) with depreciations

        [HttpGet]
        [Route("fixedAssets/getWithDepreciations")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<FixedAsset>>> GetAsyncWithDepreciations([FromQuery] bool includeDeleted = false)
        {
            var fixedAssets = await _fixedAssetRepo.GetFixedAssetsAsync(includeDeleted);

            foreach (var fixedAsset in fixedAssets)
            {
                var foundDepreciations = await _depreciationRepo.GetDepreciationByFAandPeriodAsync(fixedAsset.Id, includeDeleted, null, null);
                fixedAsset.Depreciations = foundDepreciations.ToArray();
                await _depreciationRepo.UpdateTotalDepreciationForFixedAsset(fixedAsset.Id);
            }

            return Ok(fixedAssets);

        }

        // PATCH: update fixedAsset

        [HttpPatch]
        [Route("fixedAssets/{fixedAssetId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAsync([FromBody] FixedAsset fixedAsset, Guid invoiceId, Guid depreciationConfigId, Guid fixedAssetId)
        {
            // request validations
            if (fixedAsset == null) return BadRequest("Incorrect body format");
            if (fixedAsset.Id != fixedAssetId) return BadRequest("FixedAsset Id from body incorrect");
            if (fixedAsset.InvoiceId != invoiceId) return BadRequest("Incorrect Invoice Id in body");

            // fixedAsset validation
            ValidationResult validationResult = await _fixedAssetValidator.ValidateAsync(fixedAsset);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // invoice validation
            if (!await InvoiceExists(invoiceId)) return NotFound("Invoice not found");

            // depreciationConfig validation
            if (!await DepreciationConfigExists(depreciationConfigId)) return NotFound("DepreciationConfig not found");

            fixedAsset.Id = fixedAssetId; // copy id to fixedAsset object

            await _fixedAssetValidator.ValidateAndThrowAsync(fixedAsset);

            int result = await _fixedAssetRepo.UpdateFixedAssetAsync(fixedAsset);
            if (result == 0) return NotFound("FixedAsset not found");
            return NoContent();
        }

        // DELETE: delete fixedAsset

        [HttpDelete]
        [Route("fixedAssets/{fixedAssetId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid fixedAssetId)
        {
            int result = await _fixedAssetRepo.SetDeleteFixedAssetAsync(fixedAssetId, true);
            if (result == 0) return NotFound("FixedAsset not found");
            return NoContent();
        }

        // POST: undelete fixedAsset

        [HttpPost]
        [Route("fixedAssets/{fixedAssetId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid fixedAssetId)
        {
            int result = await _fixedAssetRepo.SetDeleteFixedAssetAsync(fixedAssetId, false);
            if (result == 0) return NotFound("FixedAsset not found");
            return NoContent();
        }

        private async Task<bool> InvoiceExists(Guid invoiceId)
        {
            Invoice? invoice = await _invoiceRepo.GetInvoiceByIdAsync(invoiceId);
            return (invoice != null);
        }

        private async Task<bool> DepreciationConfigExists(Guid depreciationConfigId)
        {
            DepreciationConfig? depreciationConfig = await _depreciationConfigRepo.GetDepreciationConfigByIdAsync(depreciationConfigId);
            return (depreciationConfig != null && depreciationConfig?.Deleted == false);
        }
    }
}
