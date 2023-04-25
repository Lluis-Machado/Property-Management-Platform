using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaxManagement.Models;
using TaxManagement.Repositories;
using TaxManagement.Security;

namespace TaxManagement.Controllers
{
    public class DeclarationsController : Controller
    {
        private readonly ILogger<DeclarationsController> _logger;
        private readonly IDeclarationRepository _declarationRepo;
        private readonly IDeclarantRepository _declarantRepo;
        private readonly IValidator<Declaration> _declarationValidator;
        public DeclarationsController(IDeclarationRepository declarationRepo, IDeclarantRepository declarantRepo, IValidator<Declaration> declarationValidator, ILogger<DeclarationsController> logger)
        {
            _declarationRepo = declarationRepo;
            _declarantRepo = declarantRepo;
            _declarationValidator = declarationValidator;
            _logger = logger;   
        }

        // POST: Create declaration
        [Authorize]
        [HttpPost]
        [Route("{declarantId}/declarations")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] Declaration declaration, Guid declarantId)
        {
            // request validations
            if (declaration == null) return BadRequest("Incorrect body format");
            if (declaration.Id != Guid.Empty) return BadRequest("Id fild must be empty");
            if (declaration.DeclarantId != declarantId) return BadRequest("Incorrect declarant Id in body");

            // declaration validation
            ValidationResult validationResult = await _declarationValidator.ValidateAsync(declaration);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // declarant validation
            if(!await DeclarantExists(declarantId)) return NotFound("Declarant not found");
            Guid declarationId = await _declarationRepo.InsertDeclarationAsync(declaration);
            return Ok(declarationId);
        }

        // GET: Get declaration(s)
        [Authorize]
        [HttpGet]
        [Route("{declarantId}/declarations")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<Declaration>>> GetDeclarationsAsync(Guid declarantId)
        {
            // declarant validation
            if (!await DeclarantExists(declarantId)) return NotFound("Declarant not found");

            return Ok(await _declarationRepo.GetDeclarationsAsync(declarantId));
        }

        // POST: update declarantion
        [Authorize]
        [HttpPost]
        [Route("{declarantId}/declarations/{declarationId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAsync(Guid declarantId, Guid declarationId, [FromBody] Declaration declaration)
        {
            // validations
            if (declaration == null) return BadRequest("Incorrect body format");
            if (declaration.Id != declarationId) return BadRequest("Declaration Id from body incorrect");

            // declaration validation
            ValidationResult validationResult = await _declarationValidator.ValidateAsync(declaration);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            // declarant validation
            if (!await DeclarantExists(declarantId)) return NotFound("Declarant not found");

            declaration.Id = declarationId;

            int result = await _declarationRepo.UpdateDeclarationAsync(declaration);
            if (result == 0) return NotFound("Declaration not found");
            return Ok();
        }

        // DELETE: delete declarantion
        [Authorize]
        [HttpDelete]
        [Route("{declarantId}/declarations/{declarationId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid declarantId, Guid declarationId, string user)
        {
            try
            {
                // toDo --> check declarantId
                int result = await _declarationRepo.SetDeletedDeclarationAsync(declarationId, user, true);
                if (result == 0) return NotFound("Declaration not found");
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
        [Route("{declarantId}/declarations/{declarationId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid declarantId, Guid declarationId, string user)
        {
            try
            {
                // toDo --> check declarantId
                int result = await _declarationRepo.SetDeletedDeclarationAsync(declarationId, user, false);
                if (result == 0) return NotFound("Declaration not found");
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private async Task<bool> DeclarantExists(Guid declarantId)
        {
            Declarant declarant = await _declarantRepo.GetDeclarantByIdAsync(declarantId);
            return (declarant != null);
        }
    }
}
