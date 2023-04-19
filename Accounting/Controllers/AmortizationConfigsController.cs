using Accounting.Models;
using Accounting.Repositories;
using Accounting.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class AmortizationConfigsController : Controller
    {
        private readonly IAmortizationCongifRepository _amortizationCongifRepo;
        private readonly IValidator<AmortizationConfig> _amortizationConfigValidator;
        public AmortizationConfigsController(IAmortizationCongifRepository amortizationCongifRepo, IValidator<AmortizationConfig> amortizationConfigValidator)
        {
            _amortizationCongifRepo = amortizationCongifRepo;
            _amortizationConfigValidator = amortizationConfigValidator;
        }

        // POST: Create amortizationConfig
        [HttpPost]
        [Route("amortizationConfigs")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAsync([FromBody] AmortizationConfig amortizationConfig)
        {
            try
            {
                // validations
                if (amortizationConfig == null) return BadRequest("Incorrect body format");
                if (amortizationConfig.Id != Guid.Empty) return BadRequest("Id field must be empty");

                await _amortizationConfigValidator.ValidateAndThrowAsync(amortizationConfig);

                Guid amortizationConfigId = await _amortizationCongifRepo.InsertAmortizationConfigAsync(amortizationConfig);
                return Ok(amortizationConfigId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: Get amortizationConfig(s)
        [HttpGet]
        [Route("amortizationConfigs")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<AmortizationConfig>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                IEnumerable<AmortizationConfig> amortizationConfigs = await _amortizationCongifRepo.GetAmortizationConfigsAsync();
                return Ok(amortizationConfigs.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: update amortizationConfig
        [HttpPost]
        [Route("amortizationConfigs/{amortizationConfigId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAsync([FromBody] AmortizationConfig amortizationConfig, Guid amortizationConfigId)
        {
            try
            {
                // validations
                if (amortizationConfig == null) return BadRequest("Incorrect body format");
                if (amortizationConfig.Id != amortizationConfigId) return BadRequest("amortizationConfigId from body incorrect");
                amortizationConfig.Id = amortizationConfigId;

                await _amortizationConfigValidator.ValidateAndThrowAsync(amortizationConfig);

                int result = await _amortizationCongifRepo.UpdateAmortizationConfigAsync(amortizationConfig);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: delete amortizationConfig
        [HttpDelete]
        [Route("amortizationConfigs/{amortizationConfigId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync(Guid amortizationConfigId)
        {
            try
            {
                AmortizationConfig amortizationConfig = await _amortizationCongifRepo.GetAmortizationConfigByIdAsync(amortizationConfigId);
                amortizationConfig.Deleted = true;
                int result = await _amortizationCongifRepo.UpdateAmortizationConfigAsync(amortizationConfig);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: undelete amortizationConfig
        [HttpPost]
        [Route("amortizationConfigs/{amortizationConfigId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UndeleteAsync(Guid amortizationConfigId)
        {
            try
            {
                AmortizationConfig amortizationConfig = await _amortizationCongifRepo.GetAmortizationConfigByIdAsync(amortizationConfigId);
                amortizationConfig.Deleted = false;
                int result = await _amortizationCongifRepo.UpdateAmortizationConfigAsync(amortizationConfig);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
