using Accounting.Models;
using Accounting.Repositories;
using Accounting.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class AmortizationConfigsController : Controller
    {
        private readonly IAmortizationCongifRepository _amortizationCongifRepo;
        private readonly IValidator<AmortizationConfig> _amortizationConfigValidator;
        private readonly ILogger<AmortizationConfigsController> _logger;
        public AmortizationConfigsController(IAmortizationCongifRepository amortizationCongifRepo, IValidator<AmortizationConfig> amortizationConfigValidator, ILogger<AmortizationConfigsController> logger)
        {
            _amortizationCongifRepo = amortizationCongifRepo;
            _amortizationConfigValidator = amortizationConfigValidator;
            _logger = logger;
        }

        // POST: Create amortizationConfig
        [Authorize]
        [HttpPost]
        [Route("amortizationConfigs")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] AmortizationConfig amortizationConfig)
        {
            try
            {
                // request validations
                if (amortizationConfig == null) return BadRequest("Incorrect body format");
                if (amortizationConfig.Id != Guid.Empty) return BadRequest("Id field must be empty");

                // amortizationConfig validation
                ValidationResult validationResult = await _amortizationConfigValidator.ValidateAsync(amortizationConfig);
                if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

                return Ok(await _amortizationCongifRepo.InsertAmortizationConfigAsync(amortizationConfig));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: Get amortizationConfig(s)
        [Authorize]
        [HttpGet]
        [Route("amortizationConfigs")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<AmortizationConfig>>> GetAsync()
        {
            return Ok(await _amortizationCongifRepo.GetAmortizationConfigsAsync());
        }

        // POST: update amortizationConfig
        [Authorize]
        [HttpPost]
        [Route("amortizationConfigs/{amortizationConfigId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] AmortizationConfig amortizationConfig, Guid amortizationConfigId)
        {
            // request validations
            if (amortizationConfig == null) return BadRequest("Incorrect body format");
            if (amortizationConfig.Id != amortizationConfigId) return BadRequest("AmortizationConfig Id from body incorrect");

            // amortizationConfig validation
            ValidationResult validationResult = await _amortizationConfigValidator.ValidateAsync(amortizationConfig);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            amortizationConfig.Id = amortizationConfigId; // copy id to amortiaztionConfig object

            int result = await _amortizationCongifRepo.UpdateAmortizationConfigAsync(amortizationConfig);
            if (result == 0) return NotFound("AmortizationConfig not found");
            return Ok();
        }

        // DELETE: delete amortizationConfig
        [HttpDelete]
        [Route("amortizationConfigs/{amortizationConfigId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid amortizationConfigId)
        {
            int result = await _amortizationCongifRepo.SetDeleteAmortizationConfigAsync(amortizationConfigId, true);
            if (result == 0) return NotFound("AmortizationConfig not found");
            return Ok();
        }

        // POST: undelete amortizationConfig
        [Authorize]
        [HttpPost]
        [Route("amortizationConfigs/{amortizationConfigId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid amortizationConfigId)
        {
            int result = await _amortizationCongifRepo.SetDeleteAmortizationConfigAsync(amortizationConfigId, false);
            if (result == 0) return NotFound("AmortizationConfig not found");
            return Ok();
        }
    }
}
