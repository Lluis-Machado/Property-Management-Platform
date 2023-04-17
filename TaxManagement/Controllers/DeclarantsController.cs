using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaxManagement.Models;
using TaxManagement.Repositories;
using FluentValidation;
using TaxManagement.Security;

namespace TaxManagement.Controllers
{
    public class DeclarantsController : Controller
    {
        private readonly IDeclarantRepository _declarantRepo;
        private readonly IValidator<Declarant> _declarantValidator;
        public DeclarantsController(IDeclarantRepository declarantRepo, IValidator<Declarant> declarantValidator)
        {
            _declarantRepo = declarantRepo;
            _declarantValidator = declarantValidator;
        }

        // POST: Create declarant
        [Authorize]
        [HttpPost]
        [Route("declarants")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Guid),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAsync([FromBody] Declarant declarant)
        {
            try
            {
                // validations
                if (declarant == null) return BadRequest("Incorrect body format");
                if (declarant.Id != Guid.Empty) return BadRequest("Id fild must be empty");

                await _declarantValidator.ValidateAndThrowAsync(declarant);

                Guid declarantId = await _declarantRepo.InsertDeclarantAsync(declarant);
                return Ok(declarantId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: Get declarants(s)
        [Authorize]
        [HttpGet]
        [Route("declarants")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<Declarant>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                IEnumerable<Declarant> declarants = await _declarantRepo.GetDeclarantsAsync();
                return Ok(declarants.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: update declarant
        [Authorize]
        [HttpPost]
        [Route("declarants/{declarantId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAsync([FromBody] Declarant declarant, Guid declarantId)
        {
            try
            {
                // validations
                if (declarant == null) return BadRequest("Incorrect body format");
                if (declarant.Id != declarantId) return BadRequest("Declarant Id from body incorrect");
                declarant.Id = declarantId;

                await _declarantValidator.ValidateAndThrowAsync(declarant);

                int result = await _declarantRepo.UpdateDeclarantAsync(declarant);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: delete declarant
        [Authorize]
        [HttpDelete]
        [Route("declarants/{declarantId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync(Guid declarantId)
        {
            try
            {
                Declarant declarant = await _declarantRepo.GetDeclarantByIdAsync(declarantId);
                declarant.Deleted = true;
                int result =  await _declarantRepo.UpdateDeclarantAsync(declarant);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: undelete declarant
        [Authorize]
        [HttpPost]
        [Route("declarants/{declarantId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UndeleteAsync(Guid declarantId)
        {
            try
            {
                Declarant declarant = await _declarantRepo.GetDeclarantByIdAsync(declarantId);
                declarant.Deleted = false;
                int result = await _declarantRepo.UpdateDeclarantAsync(declarant);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
