using Accounting.Models;
using Accounting.Repositories;
using Accounting.Security;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class DepreciationConfigsController : Controller
    {
        private readonly IDepreciationConfigRepository _depreciationCongifRepo;
        private readonly IValidator<DepreciationConfig> _depreciationConfigValidator;
        private readonly ILogger<DepreciationConfig> _logger;
        public DepreciationConfigsController(IDepreciationConfigRepository depreciationCongifRepo, IValidator<DepreciationConfig> depreciationConfigValidator, ILogger<DepreciationConfig> logger)
        {
            _depreciationCongifRepo = depreciationCongifRepo;
            _depreciationConfigValidator = depreciationConfigValidator;
            _logger = logger;
        }

        // POST: Create depreciationConfig
        [Authorize]
        [HttpPost]
        [Route("depreciationConfigs")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] DepreciationConfig depreciationConfig)
        {
            // request validations
            if (depreciationConfig == null) return BadRequest("Incorrect body format");
            if (depreciationConfig.Id != Guid.Empty) return BadRequest("depreciationConfig Id field must be empty");

            // depreciationConfig validation
            ValidationResult validationResult = await _depreciationConfigValidator.ValidateAsync(depreciationConfig);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            return Ok(await _depreciationCongifRepo.InsertDepreciationConfigAsync(depreciationConfig));
        }

        // GET: Get depreciationConfig(s)
        [Authorize]
        [HttpGet]
        [Route("depreciationConfigs")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<DepreciationConfig>>> GetAsync()
        {
            return Ok(await _depreciationCongifRepo.GetDepreciationConfigsAsync());
        }

        // POST: update depreciationConfig
        [Authorize]
        [HttpPost]
        [Route("depreciationConfigs/{depreciationConfigId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> UpdateAsync([FromBody] DepreciationConfig depreciationConfig, Guid depreciationConfigId)
        {
            // request validations
            if (depreciationConfig == null) return BadRequest("Incorrect body format");
            if (depreciationConfig.Id != depreciationConfigId) return BadRequest("depreciationConfig Id from body incorrect");

            // depreciationConfig validation
            ValidationResult validationResult = await _depreciationConfigValidator.ValidateAsync(depreciationConfig);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            depreciationConfig.Id = depreciationConfigId; // cpoy id to depreciationConfig object

            int result = await _depreciationCongifRepo.UpdateDepreciationConfigAsync(depreciationConfig);
            if (result == 0) return NotFound("depreciationConfig not found");
            return Ok();
        }

        // DELETE: delete depreciationConfig
        [Authorize]
        [HttpDelete]
        [Route("depreciationConfigs/{depreciationConfigId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid depreciationConfigId)
        {
            int result = await _depreciationCongifRepo.SetDeleteDepereciationConfigAsync(depreciationConfigId, true);
            if (result == 0) return NotFound("depreciationConfig not found");
            return Ok();
        }

        // POST: undelete depreciationConfig
        [Authorize]
        [HttpPost]
        [Route("depreciationConfigs/{depreciationConfigId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid depreciationConfigId)
        {
            int result = await _depreciationCongifRepo.SetDeleteDepereciationConfigAsync(depreciationConfigId, false);
            if (result == 0) return NotFound("depreciationConfig not found");
            return Ok();
        }
    }
}
