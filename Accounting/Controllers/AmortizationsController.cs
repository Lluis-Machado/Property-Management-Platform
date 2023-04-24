using Accounting.Models;
using Accounting.Repositories;
using Accounting.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    public class AmortizationsController : Controller
    {
        private readonly IAmortizationRepository _amortizationRepo;
        private readonly IValidator<Amortization> _amortizationValidator;
        public AmortizationsController(IAmortizationRepository amortizationRepo, IValidator<Amortization> amortizationValidator)
        {
            _amortizationRepo = amortizationRepo;
            _amortizationValidator = amortizationValidator;
        }

        // POST: Create amortization
        [HttpPost]
        [Route("amortizations")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAsync([FromBody] Amortization amortization)
        {
            try
            {
                // validations
                if (amortization == null) return BadRequest("Incorrect body format");
                if (amortization.Id != Guid.Empty) return BadRequest("Id field must be empty");

                await _amortizationValidator.ValidateAndThrowAsync(amortization);

                Guid amortizationId = await _amortizationRepo.InsertAmortizationAsync(amortization);
                return Ok(amortizationId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: Get amortization(s)
        [HttpGet]
        [Route("amortizations")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<Amortization>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                IEnumerable<Amortization> amortizations = await _amortizationRepo.GetAmortizationsAsync();
                return Ok(amortizations.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: update amortization
        [HttpPost]
        [Route("amortizations/{amortizationId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAsync([FromBody] Amortization amortization, Guid amortizationId)
        {
            try
            {
                // validations
                if (amortization == null) return BadRequest("Incorrect body format");
                if (amortization.Id != amortizationId) return BadRequest("amortizationId from body incorrect");
                amortization.Id = amortizationId;

                await _amortizationValidator.ValidateAndThrowAsync(amortization);

                int result = await _amortizationRepo.UpdateAmortizationAsync(amortization);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: delete amortization
        [HttpDelete]
        [Route("amortizations/{amortizationId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync(Guid amortizationId)
        {
            try
            {
                Amortization amortization = await _amortizationRepo.GetAmortizationByIdAsync(amortizationId);
                amortization.Deleted = true;
                int result = await _amortizationRepo.UpdateAmortizationAsync(amortization);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: undelete amortization
        [HttpPost]
        [Route("amortizations/{amortizationId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UndeleteAsync(Guid amortizationId)
        {
            try
            {
                Amortization amortization = await _amortizationRepo.GetAmortizationByIdAsync(amortizationId);
                amortization.Deleted = false;
                int result = await _amortizationRepo.UpdateAmortizationAsync(amortization);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
