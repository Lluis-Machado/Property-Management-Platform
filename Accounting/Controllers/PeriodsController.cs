using AccountingAPI.DTOs;
using AccountingAPI.Services;
using AccountingAPI.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static AccountingAPI.DTOs.UpdatePeriodDTO;

namespace AccountingAPI.Controllers
{
    [Authorize]
    public class PeriodsController : Controller
    {
        private readonly IPeriodService _periodService;
        private readonly ILogger<PeriodsController> _logger;

        public PeriodsController(IPeriodService periodService, ILogger<PeriodsController> logger)
        {
            _periodService = periodService;
            _logger = logger;
        }

        // POST: Create period
        [HttpPost]
        [Route("tenants/{tenantId}/periods")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]

        public async Task<ActionResult<PeriodDTO>> CreatePeriodsAsync([FromBody] CreatePeriodDTO createPeriodDTO, Guid tenantId)
        {
            // request validations
            if (createPeriodDTO == null) return BadRequest("Incorrect body format");

            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            PeriodDTO periodDTO = await _periodService.CreatePeriodAsync(createPeriodDTO, tenantId, userName);

            return Created($"periods/{periodDTO.Id}", periodDTO);
        }

        // GET: Get period(s)
        [HttpGet]
        [Route("tenants/{tenantId}/periods")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<PeriodDTO>>> GetPeriodsAsync(Guid tenantId, [FromQuery] bool includeDeleted = false)
        {
            return Ok(await _periodService.GetPeriodsAsync(tenantId, includeDeleted));
        }

        // PATCH: Update period
        [HttpPatch]
        [Route("tenants/{tenantId}/periods/{periodId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PeriodDTO>> UpdatePeriodAsync(Guid tenantId,Guid periodId, UpdatePeriodDTO updatePeriodDTO)
        {
            // request validations
            if (updatePeriodDTO == null) return BadRequest("Incorrect body format");

            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            return Ok(await _periodService.UpdatePeriodStatusAsync(tenantId, periodId, updatePeriodDTO, userName));
        }

        // DELETE: delete period
        [HttpDelete]
        [Route("tenants/{tenantId}/periods/{periodId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid tenantId, Guid periodId)
        {
            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            await _periodService.SetDeletedPeriodAsync(tenantId, periodId, true, userName);

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
            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            await _periodService.SetDeletedPeriodAsync(tenantId, periodId, false, userName);

            return NoContent();
        }
    }
}
