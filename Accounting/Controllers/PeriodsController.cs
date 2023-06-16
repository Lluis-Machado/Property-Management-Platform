using AccountingAPI.DTOs;
using AccountingAPI.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountingAPI.Controllers
{
    [Authorize]
    public class PeriodsController : Controller
    {
        private readonly IPeriodService _periodService;
        private readonly IValidator<CreatePeriodDTO> _periodDTOValidator;
        private readonly ILogger<PeriodsController> _logger;

        public PeriodsController(IPeriodService periodService, IValidator<CreatePeriodDTO> periodDTOValidator, ILogger<PeriodsController> logger)
        {
            _periodService = periodService;
            _periodDTOValidator = periodDTOValidator;
            _logger = logger;
        }

        // POST: Create period
        [HttpPost]
        [Route("tenants/{tenantId}/periods")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PeriodDTO>> CreatePeriodAsync([FromBody] CreatePeriodDTO createPeriodDTO, Guid tenantId)
        {
            // request validations
            if (createPeriodDTO == null) return BadRequest("Incorrect body format");

            ValidationResult validationResult = await _periodDTOValidator.ValidateAsync(createPeriodDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // check if already exists
            if (await _periodService.CheckIfPeriodExistsAsync(tenantId, createPeriodDTO.Year, createPeriodDTO.Month))
            {
                return Conflict("Period already exists");
            }

            PeriodDTO periodDTO = await _periodService.CreatePeriodAsync(createPeriodDTO, tenantId, User?.Identity?.Name);

            return Created($"periods/{periodDTO.Id}", periodDTO);
        }

        // GET: Get period(s)
        [HttpGet]
        [Route("tenants/{tenantId}/periods")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<PeriodDTO>>> GetPeriodsAsync(Guid tenantId, [FromQuery] bool includeDeleted = false)
        {
            return Ok(await _periodService.GetPeriodsAsync(tenantId,includeDeleted));
        }

        // PATCH: update period
        [HttpPatch]
        [Route("tenants/{tenantId}/periods/{periodId}/status={status}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PeriodDTO>> ChangePeriodStatusAsync(Guid tenantId, Guid periodId, string status)
        {
            // check if exists
            if (!await _periodService.CheckIfPeriodExistsByIdAsync(periodId)) return NotFound("Period not found");

            return Ok(await _periodService.UpdatePeriodStatusAsync(status, User?.Identity?.Name, periodId));
        }

        // DELETE: delete period
        [HttpDelete]
        [Route("tenants/{tenantId}/periods/{periodId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid tenantId ,Guid periodId)
        {
            // check if exists
            if (!await _periodService.CheckIfPeriodExistsByIdAsync(periodId)) return NotFound("Period not found");

            await _periodService.SetDeletedPeriodAsync(periodId, true);

            return NoContent();
        }

        // POST: undelete period
        [HttpPost]
        [Route("tenants/{tenantId}/periods/{periodId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid tenantId, Guid periodId)
        {
            // check if exists
            if (!await _periodService.CheckIfPeriodExistsByIdAsync(periodId)) return NotFound("Period not found");

            await _periodService.SetDeletedPeriodAsync(periodId, false);

            return NoContent();
        }
    }
}
