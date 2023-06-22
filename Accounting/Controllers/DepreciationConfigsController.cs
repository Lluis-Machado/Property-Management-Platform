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
    public class DepreciationConfigsController : Controller
    {
        private readonly IDepreciationConfigRepository _depreciationCongifRepo;
        private readonly IValidator<DepreciationConfig> _depreciationConfigValidator;
        private readonly ILogger<DepreciationConfigsController> _logger;
        public DepreciationConfigsController(IDepreciationConfigRepository depreciationCongifRepo, IValidator<DepreciationConfig> depreciationConfigValidator, ILogger<DepreciationConfigsController> logger)
        {
            _depreciationCongifRepo = depreciationCongifRepo;
            _depreciationConfigValidator = depreciationConfigValidator;
            _logger = logger;
        }

        // POST: Create depreciationConfig

        [HttpPost]
        [Route("depreciationConfigs")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] DepreciationConfig depreciationConfig)
        {
            // request validations
            if (depreciationConfig == null) return BadRequest("Incorrect body format");
            if (depreciationConfig.Id != Guid.Empty) return BadRequest("DepreciationConfig Id field must be empty");

            // depreciationConfig validation
            ValidationResult validationResult = await _depreciationConfigValidator.ValidateAsync(depreciationConfig);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            depreciationConfig = await _depreciationCongifRepo.InsertDepreciationConfigAsync(depreciationConfig);
            return Created($"depreciationConfigs/{depreciationConfig.Id}", depreciationConfig);
        }

        // GET: Get depreciationConfig(s)

        [HttpGet]
        [Route("depreciationConfigs")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<DepreciationConfig>>> GetAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _depreciationCongifRepo.GetDepreciationConfigsAsync(includeDeleted));
        }

        // PATCH: update depreciationConfig

        [HttpPatch]
        [Route("depreciationConfigs/{depreciationConfigId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAsync([FromBody] DepreciationConfig depreciationConfig, Guid depreciationConfigId)
        {
            // request validations
            if (depreciationConfig == null) return BadRequest("Incorrect body format");
            if (depreciationConfig.Id != depreciationConfigId) return BadRequest("DepreciationConfig Id from body incorrect");

            // depreciationConfig validation
            ValidationResult validationResult = await _depreciationConfigValidator.ValidateAsync(depreciationConfig);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            depreciationConfig.Id = depreciationConfigId; // cpoy id to depreciationConfig object

            int result = await _depreciationCongifRepo.UpdateDepreciationConfigAsync(depreciationConfig);
            if (result == 0) return NotFound("DepreciationConfig not found");
            return NoContent();
        }

        // DELETE: delete depreciationConfig

        [HttpDelete]
        [Route("depreciationConfigs/{depreciationConfigId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid depreciationConfigId)
        {
            int result = await _depreciationCongifRepo.SetDeleteDepreciationConfigAsync(depreciationConfigId, true);
            if (result == 0) return NotFound("DepreciationConfig not found");
            return NoContent();
        }

        // POST: undelete depreciationConfig

        [HttpPost]
        [Route("depreciationConfigs/{depreciationConfigId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid depreciationConfigId)
        {
            int result = await _depreciationCongifRepo.SetDeleteDepreciationConfigAsync(depreciationConfigId, false);
            if (result == 0) return NotFound("DepreciationConfig not found");
            return NoContent();
        }
    }
}
