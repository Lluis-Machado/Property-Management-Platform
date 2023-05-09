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
        private readonly IDepreciationConfigRepository _depreciationConfigRepo;
        private readonly IFixedAssetRepository _fixedAssetRepo;
        private readonly IValidator<Depreciation> _depreciationValidator;
        private readonly ILogger<DepreciationsController> _logger;

        public DepreciationsController(IDepreciationRepository depreciationRepo, IDepreciationConfigRepository depreciationConfigRepo, IFixedAssetRepository fixedAssetRepo, IValidator<Depreciation> depreciationValidator, ILogger<DepreciationsController> logger)
        {
            _depreciationRepo = depreciationRepo;
            _depreciationConfigRepo = depreciationConfigRepo;
            _fixedAssetRepo = fixedAssetRepo;
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
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] Depreciation depreciation)
        {
            // request validations
            if (depreciation == null) return BadRequest("Incorrect body format");
            if (depreciation.Id != Guid.Empty) return BadRequest("Depreciation Id field must be empty");

            // depreciation validation
            ValidationResult validationResult = await _depreciationValidator.ValidateAsync(depreciation);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));


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
        [HttpPatch]
        [Route("depreciations/{depreciationId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAsync([FromBody] Depreciation depreciation, Guid depreciationId)
        {
            // request validations
            if (depreciation == null) return BadRequest("Incorrect body format");
            if (depreciation.Id != depreciationId) return BadRequest("Depreciation Id from body incorrect");

            // depreciation validation
            ValidationResult validationResult = await _depreciationValidator.ValidateAsync(depreciation);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

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


        // GET: Depreciations by Fixed Asset and Period
        [Authorize]
        [HttpGet]
        [Route("depreciations")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Depreciation>>> GetByFAandPeriodAsync(bool fillMissing, Guid fixedAssetId, DateTime? periodStart, DateTime? periodEnd)
        {

            // Checks before starting
            if (periodStart != null && periodEnd != null && periodEnd < periodStart)
            {
                throw new Exception("Period end cannot be before period start");
            }

            var foundDepreciations = await _depreciationRepo.GetDepreciationByFAandPeriodAsync(fixedAssetId, periodStart, periodEnd);
            foundDepreciations = foundDepreciations.ToArray();

            if (!fillMissing)
                return Ok(await _depreciationRepo.GetDepreciationByFAandPeriodAsync(fixedAssetId, periodStart, periodEnd));
            else
            {
                // Check if the depreciation exists. If found, return the array

                bool skipCheck = periodStart == null || periodEnd == null;

                if (periodStart == null) periodStart = DateTime.MinValue;
                if (periodEnd == null) periodEnd = DateTime.MaxValue;

                // We skip checking since one of the two parameters was null
                if (!skipCheck)
                {
                    bool found = false;
                    foreach (Depreciation dep in foundDepreciations)
                    {
                        if (dep.PeriodStart == periodStart && dep.PeriodEnd == periodEnd)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        return Ok(foundDepreciations);
                    }
                }

                // Calculate depreciation during this period if not present
                FixedAsset fa = await _fixedAssetRepo.GetFixedAssetByIdAsync(fixedAssetId);
                DepreciationConfig dc = await _depreciationConfigRepo.GetDepreciationConfigByIdAsync(fa.DepreciationConfigId);

                // Check if fixed asset data contains depreciation config info
                // If not, get them from the depreciation config entry
                if (fa.DepreciationAmountPercent == 0 && fa.DepreciationMaxYears == 0)
                {
                    if (dc.DepreciationPercent == 0 && dc.MaxYears == 0)
                    {
                        throw new Exception($"Error calculating depreciation for fixed asset w/ ID {fixedAssetId} - No depreciation settings found");
                    }
                    else
                    {
                        // Update data in the fixed asset entry
                        fa.DepreciationAmountPercent = dc.DepreciationPercent;
                        fa.DepreciationMaxYears = dc.MaxYears;

                        _ = _fixedAssetRepo.UpdateFixedAssetAsync(fa).Result;
                    }
                }

                Depreciation depreciation = new Depreciation()
                {
                    PeriodStart = (DateTime)periodStart,
                    PeriodEnd = (DateTime)periodEnd,
                    FixedAssetId = fixedAssetId
                };
                double depreciationPerDay;

                // Check whether to use percent or years lineal depreciation
                if (fa.DepreciationAmountPercent != 0)
                {
                    // Depreciation per percentage
                    double yearlyDepreciation = fa.ActivationAmount * fa.DepreciationAmountPercent / 100;
                    depreciationPerDay = yearlyDepreciation / 365;
                    int daysAmount = (depreciation.PeriodEnd - depreciation.PeriodStart).Days;

                    depreciation.Amount = depreciationPerDay * daysAmount;
                }
                else
                {
                    // Depreciation per useful life years
                    // Check if useful life already ran out

                    DateTime usefulLifeEnd = fa.ActivationDate.AddYears(fa.DepreciationMaxYears);
                    if (usefulLifeEnd < periodStart)
                    {
                        depreciation.Amount = 0;
                    } else 
                    {
                        double yearlyDepreciation = fa.ActivationAmount / fa.DepreciationMaxYears;
                        depreciationPerDay = yearlyDepreciation / 365;

                        int daysAmount;
                        if (usefulLifeEnd >= periodStart && usefulLifeEnd <= periodEnd)
                            daysAmount = (depreciation.PeriodEnd - usefulLifeEnd).Days;
                        else
                            daysAmount = (depreciation.PeriodEnd - depreciation.PeriodStart).Days;
                        depreciation.Amount = depreciationPerDay * daysAmount;
                    }

                }

                // Save depreciation and return
                await _depreciationRepo.InsertDepreciationAsync(depreciation);

                return Ok(depreciation);
            }
        }

    }
}
