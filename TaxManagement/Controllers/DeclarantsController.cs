using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaxManagement.Models;
using TaxManagement.Repositories;
using FluentValidation;
using TaxManagement.Security;
using FluentValidation.Results;

namespace TaxManagement.Controllers
{
    public class DeclarantsController : Controller
    {
        private readonly ILogger<DeclarantsController> _logger;
        private readonly IDeclarantRepository _declarantRepo;
        private readonly IValidator<Declarant> _declarantValidator;
        public DeclarantsController(IDeclarantRepository declarantRepo, IValidator<Declarant> declarantValidator, ILogger<DeclarantsController> logger)
        {
            _declarantRepo = declarantRepo;
            _declarantValidator = declarantValidator;
            _logger = logger;  
        }

        // POST: Create declarant
        [Authorize]
        [HttpPost]
        [Route("declarants")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]

        public async Task<ActionResult<Declarant>> CreateAsync([FromBody] Declarant declarant)
        {
            // request validations
            if (declarant == null) return BadRequest("Incorrect body format");
            if (declarant.Id != Guid.Empty) return BadRequest("Id fild must be empty");

            // declarant validation
            ValidationResult validationResult  = await _declarantValidator.ValidateAsync(declarant);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            declarant = await _declarantRepo.InsertDeclarantAsync(declarant);
            return Created($"declarants/{declarant.Id}", declarant);
        }

        // GET: Get declarants(s)
        [Authorize]
        [HttpGet]
        [Route("declarants")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]

        public async Task<ActionResult<IEnumerable<Declarant>>> GetAsync()
        {
            return Ok(await _declarantRepo.GetDeclarantsAsync());
        }

        // POST: update declarant
        [Authorize]
        [HttpPatch]
        [Route("declarants/{declarantId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> UpdateAsync([FromBody] Declarant declarant, Guid declarantId)
        {
            // validations
            if (declarant == null) return BadRequest("Incorrect body format");
            if (declarant.Id != declarantId) return BadRequest("Declarant Id from body incorrect");

            // declarant validation
            ValidationResult validationResult = await _declarantValidator.ValidateAsync(declarant);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            declarant.Id = declarantId; // copy id to declarant object

            int result = await _declarantRepo.UpdateDeclarantAsync(declarant);
            if(result == 0) return NotFound("Declarant not found");
            return NoContent();
        }

        // DELETE: delete declarant
        [Authorize]
        [HttpDelete]
        [Route("declarants/{declarantId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid declarantId)
        {
            int result = await _declarantRepo.SetDeleteDeclarantAsync(declarantId, true);
            if (result == 0) return NotFound("Declarant not found");
            return NoContent();
        }

        // POST: undelete declarant
        [Authorize]
        [HttpPatch]
        [Route("declarants/{declarantId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid declarantId)
        {
            int result = await _declarantRepo.SetDeleteDeclarantAsync(declarantId, false);
            if (result == 0) return NotFound("Declarant not found");
            return NoContent();
        }
    }
}
