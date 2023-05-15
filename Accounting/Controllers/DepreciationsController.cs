using Accounting.Models;
using Accounting.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;

namespace Accounting.Controllers
{
    [Authorize]
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

        [HttpGet]
        [Route("depreciations")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Depreciation>>> GetAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _depreciationRepo.GetDepreciationsAsync(includeDeleted));
        }

        // PATCH: update depreciation

        [HttpPatch]
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


        // POST: Upsert depreciations

        [HttpPost]
        [Route("depreciations/upsert")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Depreciation>>> UpsertDepreciation([FromBody] Depreciation depreciation)
        {
            // Request validations
            if (depreciation == null) return BadRequest("Incorrect body format");
            if (depreciation.PeriodStart != null && depreciation.PeriodEnd != null && depreciation.PeriodEnd < depreciation.PeriodStart)
                return BadRequest("Period end cannot be before period start");

            // depreciation validation
            ValidationResult validationResult = await _depreciationValidator.ValidateAsync(depreciation);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // fixedAsset validation
            if (!await FixedAssetExists(depreciation.FixedAssetId)) return NotFound("FixedAsset not found");

            await _depreciationValidator.ValidateAndThrowAsync(depreciation);

            _logger.Log(LogLevel.Debug, $"UpsertDepreciation - Validation OK");

            DateTime periodStart = depreciation.PeriodStart;
            DateTime periodEnd = depreciation.PeriodEnd;

            // Strip hours info if present
            periodStart = DateTime.ParseExact(periodStart.ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.InvariantCulture);
            periodEnd = DateTime.ParseExact(periodEnd.ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.InvariantCulture);

            // Check if the depreciation exists
            var foundDepreciations =
                (await _depreciationRepo.GetDepreciationByFAandPeriodAsync(
                    depreciation.FixedAssetId,
                    includeDeleted: false,
                    periodStart.AddMonths(-6),
                    periodEnd.AddMonths(6))).ToArray();

            Depreciation depToUpdate = null;

            foreach (Depreciation dep in foundDepreciations)
            {

                // Strip hours info
                DateTime dep_periodStart = DateTime.ParseExact(dep.PeriodStart.ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.InvariantCulture);
                DateTime dep_periodEnd = DateTime.ParseExact(dep.PeriodEnd.ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.InvariantCulture);

                if (dep_periodStart == periodStart && dep_periodEnd == periodEnd)
                {
                    // If an exact match is found, return the current depreciation
                    _logger.Log(LogLevel.Debug, $"UpsertDepreciation - Found an exact match!");
                    return Ok(dep);
                }
                //else if (
                //    (depreciation.PeriodStart >= dep.PeriodStart && depreciation.PeriodStart <= dep.PeriodEnd && depreciation.PeriodEnd >= dep.PeriodEnd && depreciation.PeriodEnd >= dep.PeriodStart) ||
                //    (depreciation.PeriodStart <= dep.PeriodStart && depreciation.PeriodEnd <= dep.PeriodEnd && depreciation.PeriodEnd >= dep.PeriodStart && depreciation.PeriodStart <= dep.PeriodEnd) ||
                //    (depreciation.PeriodStart <= dep.PeriodStart && depreciation.PeriodStart <= dep.PeriodEnd && depreciation.PeriodEnd >= dep.PeriodStart && depreciation.PeriodEnd >= dep.PeriodEnd))
                //{

                // chatgpt to the rescue
                else if (depreciation.PeriodStart <= dep.PeriodEnd && depreciation.PeriodEnd >= dep.PeriodStart)
                {
                    depToUpdate = dep;
                    _logger.Log(LogLevel.Debug, $"UpsertDepreciation - Found a depreciation with overlap!");
                }
            }

            if (depToUpdate == null) _logger.Log(LogLevel.Debug, $"UpsertDepreciation - No overlapping depreciation found");

            // Calculate depreciation during this period if not present
            FixedAsset? fa = await _fixedAssetRepo.GetFixedAssetByIdAsync(depreciation.FixedAssetId);
            if (fa == null) throw new Exception($"Could not find fixed asset w/ ID {depreciation.FixedAssetId}");
            DepreciationConfig? dc = await _depreciationConfigRepo.GetDepreciationConfigByIdAsync(fa.DepreciationConfigId);

            if (fa.ActivationDate > periodStart) throw new Exception($"Depreciation period start cannot be before asset activation date");

            // Check if fixed asset data contains depreciation config info
            // If not, get them from the depreciation config entry
            if (fa.DepreciationAmountPercent == 0 && fa.DepreciationMaxYears == 0)
            {
                if (dc != null && dc.DepreciationPercent == 0 && dc.MaxYears == 0)
                {
                    throw new Exception($"Error calculating depreciation for fixed asset w/ ID {depreciation.FixedAssetId} - Depreciation settings found, but invalid");
                }
                else
                {
                    if (dc == null) throw new Exception($"Error calculating depreciation for fixed asset w/ ID {depreciation.FixedAssetId} - No depreciation settings found in database");

                    // Update data in the fixed asset entry
                    fa.DepreciationAmountPercent = dc.DepreciationPercent;
                    fa.DepreciationMaxYears = dc.MaxYears;

                    await _fixedAssetRepo.UpdateFixedAssetAsync(fa);
                }
            }

            _logger.Log(LogLevel.Debug, $"UpsertDepreciation - Total fixed asset value: {fa.ActivationAmount}");

            // Create depreciation object
            // Update PeriodStart and PeriodEnd if they extend outwards compared to the found depreciation
#pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
            Depreciation d = new Depreciation()
            {
                PeriodStart = depToUpdate != null ? (depToUpdate.PeriodStart > periodStart ? periodStart : depToUpdate.PeriodStart) : periodStart,
                PeriodEnd = depToUpdate != null ? (depToUpdate.PeriodEnd < periodEnd ? periodEnd : depToUpdate.PeriodEnd) : periodEnd,
                FixedAssetId = depreciation.FixedAssetId
                //,LastModificationByUser = User?.Identity.Name // TODO: Añadir cuando se implemente cambio en Authorize
                //,LastModificationDate = DateTime.Now
            };
#pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.

            // If a dates overlap was found in the existing entries in the database, set the Id so that we update instead of insert
            if (depToUpdate != null) d.Id = depToUpdate.Id;

            double depreciationPerDay;

            // Check whether to use percent or years lineal depreciation
            if (fa.DepreciationAmountPercent != 0)
            {
                _logger.Log(LogLevel.Debug, $"UpsertDepreciation - Using lineal calculation per PERCENTAGE ({fa.DepreciationAmountPercent}%)");
                // Depreciation per percentage
                double yearlyDepreciation = fa.ActivationAmount * fa.DepreciationAmountPercent / 100;
                depreciationPerDay = yearlyDepreciation / 365;
                int daysAmount = (d.PeriodEnd - d.PeriodStart).Days;

                _logger.Log(LogLevel.Debug, $"UpsertDepreciation - Days between {d.PeriodStart.ToString("dd/MM/yyyy")} and {d.PeriodEnd.ToString("dd/MM/yyyy")}: {daysAmount}");

                d.Amount = depreciationPerDay * daysAmount;
            }
            else
            {
                _logger.Log(LogLevel.Debug, $"UpsertDepreciation - Using lineal calculation per USEFUL LIFE YEARS ({fa.DepreciationMaxYears} yrs)");
                // Depreciation per useful life years
                // Check if useful life already ran out

                DateTime usefulLifeEnd = fa.ActivationDate.AddYears(fa.DepreciationMaxYears);
                if (usefulLifeEnd < d.PeriodStart)
                {
                    d.Amount = 0;
                    _logger.Log(LogLevel.Debug, $"UpsertDepreciation - Depreciation is gonna be zero because the useful life ended before the period started");
                }
                else
                {
                    double yearlyDepreciation = fa.ActivationAmount / fa.DepreciationMaxYears;
                    depreciationPerDay = yearlyDepreciation / 365;

                    int daysAmount;
                    if (usefulLifeEnd >= d.PeriodStart && usefulLifeEnd <= d.PeriodEnd)
                        daysAmount = (d.PeriodEnd - usefulLifeEnd).Days;
                    else
                        daysAmount = (d.PeriodEnd - d.PeriodStart).Days;
                    d.Amount = depreciationPerDay * daysAmount;

                    _logger.Log(LogLevel.Debug, $"UpsertDepreciation - Days between {d.PeriodStart.ToString("dd/MM/yyyy")} and {d.PeriodEnd.ToString("dd/MM/yyyy")}: {daysAmount}");
                }

            }

            _logger.Log(LogLevel.Debug, $"UpsertDepreciation - Total depreciation amount: {d.Amount} euros");

            // Save depreciation and return

            if (depToUpdate != null)
            {
                _logger.Log(LogLevel.Debug, $"UpsertDepreciation - Updating depreciation w/ ID: {depToUpdate.Id}");
                await _depreciationRepo.UpdateDepreciationAsync(d);
            }
            else
            {
                _logger.Log(LogLevel.Debug, $"UpsertDepreciation - Inserting new depreciation");
                await _depreciationRepo.InsertDepreciationAsync(d);
            }

            // Update FixedAsset
            await _depreciationRepo.UpdateTotalDepreciationForFixedAsset(fa.Id);

            return Ok(d);
        }

        // GET: Depreciations by Fixed Asset and Period

        [HttpGet]
        [Route("{fixedAssetId}/depreciations")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Depreciation>>> GetByFAandPeriodAsync(Guid fixedAssetId, bool includeDeleted, DateTime? periodStart, DateTime? periodEnd)
        {
            // Checks before starting
            if (periodStart != null && periodEnd != null && periodEnd < periodStart)
            {
                return BadRequest("Period end cannot be before period start");
            }

            return Ok(await _depreciationRepo.GetDepreciationByFAandPeriodAsync(fixedAssetId, includeDeleted, periodStart, periodEnd));
        }

        private async Task<bool> FixedAssetExists(Guid fixedAssetId)
        {
            FixedAsset? fixedAsset = await _fixedAssetRepo.GetFixedAssetByIdAsync(fixedAssetId);
            return (fixedAsset != null && fixedAsset?.Deleted == false);
        }

    }
}
