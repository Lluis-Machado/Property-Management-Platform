using Accounting.Models;
using Accounting.Repositories;
using Accounting.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class DepreciationConfigsController : Controller
    {
        private readonly IDepreciationCongifRepository _DepreciationCongifRepo;
        private readonly IValidator<DepreciationConfig> _DepreciationConfigValidator;
        public DepreciationConfigsController(IDepreciationCongifRepository DepreciationCongifRepo, IValidator<DepreciationConfig> DepreciationConfigValidator)
        {
            _DepreciationCongifRepo = DepreciationCongifRepo;
            _DepreciationConfigValidator = DepreciationConfigValidator;
        }

        // POST: Create DepreciationConfig
        [HttpPost]
        [Route("DepreciationConfigs")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAsync([FromBody] DepreciationConfig DepreciationConfig)
        {
            try
            {
                // validations
                if (DepreciationConfig == null) return BadRequest("Incorrect body format");
                if (DepreciationConfig.Id != Guid.Empty) return BadRequest("Id field must be empty");

                await _DepreciationConfigValidator.ValidateAndThrowAsync(DepreciationConfig);

                Guid DepreciationConfigId = await _DepreciationCongifRepo.InsertDepreciationConfigAsync(DepreciationConfig);
                return Ok(DepreciationConfigId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: Get DepreciationConfig(s)
        [HttpGet]
        [Route("DepreciationConfigs")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<DepreciationConfig>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                IEnumerable<DepreciationConfig> DepreciationConfigs = await _DepreciationCongifRepo.GetDepreciationConfigsAsync();
                return Ok(DepreciationConfigs.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: update DepreciationConfig
        [HttpPost]
        [Route("DepreciationConfigs/{DepreciationConfigId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAsync([FromBody] DepreciationConfig DepreciationConfig, Guid DepreciationConfigId)
        {
            try
            {
                // validations
                if (DepreciationConfig == null) return BadRequest("Incorrect body format");
                if (DepreciationConfig.Id != DepreciationConfigId) return BadRequest("DepreciationConfigId from body incorrect");
                DepreciationConfig.Id = DepreciationConfigId;

                await _DepreciationConfigValidator.ValidateAndThrowAsync(DepreciationConfig);

                int result = await _DepreciationCongifRepo.UpdateDepreciationConfigAsync(DepreciationConfig);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: delete DepreciationConfig
        [HttpDelete]
        [Route("DepreciationConfigs/{DepreciationConfigId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync(Guid DepreciationConfigId)
        {
            try
            {
                DepreciationConfig DepreciationConfig = await _DepreciationCongifRepo.GetDepreciationConfigByIdAsync(DepreciationConfigId);
                DepreciationConfig.Deleted = true;
                int result = await _DepreciationCongifRepo.UpdateDepreciationConfigAsync(DepreciationConfig);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: undelete DepreciationConfig
        [HttpPost]
        [Route("DepreciationConfigs/{DepreciationConfigId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UndeleteAsync(Guid DepreciationConfigId)
        {
            try
            {
                DepreciationConfig DepreciationConfig = await _DepreciationCongifRepo.GetDepreciationConfigByIdAsync(DepreciationConfigId);
                DepreciationConfig.Deleted = false;
                int result = await _DepreciationCongifRepo.UpdateDepreciationConfigAsync(DepreciationConfig);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
