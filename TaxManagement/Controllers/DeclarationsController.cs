using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaxManagement.Models;
using TaxManagement.Repositories;
using TaxManagement.Security;

namespace TaxManagement.Controllers
{
    public class DeclarationsController : Controller
    {
        private readonly IDeclarationRepository _declarationRepo;
        private readonly IDeclarantRepository _declarantRepo;
        private readonly IValidator<Declaration> _declarationValidator;
        public DeclarationsController(IDeclarationRepository declarationRepo, IDeclarantRepository declarantRepo, IValidator<Declaration> declarationValidator)
        {
            _declarationRepo = declarationRepo;
            _declarantRepo = declarantRepo;
            _declarationValidator = declarationValidator;
        }

        // POST: Create declaration
        [Authorize]
        [HttpPost]
        [Route("{declarantId}/declarations")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAsync([FromBody] Declaration declaration, Guid declarantId)
        {
            try
            {
                // validations
                if (declaration == null) return BadRequest("Incorrect body format");
                if (declaration.Id != Guid.Empty) return BadRequest("Id fild must be empty");
                if (declaration.DeclarantId != declarantId) return BadRequest("Incorrect declarant Id in body");

                await _declarationValidator.ValidateAndThrowAsync(declaration);

                try {
                    await _declarantRepo.GetDeclarantByIdAsync(declarantId);
                } 
                catch
                {
                    return BadRequest("Declarant not found");
                }

                Guid declarationId = await _declarationRepo.InsertDeclarationAsync(declaration);
                return Ok(declarationId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: Get declaration(s)
        [Authorize]
        [HttpGet]
        [Route("{declarantId}/declarations")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<Declaration>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDeclarationsAsync(Guid declarantId)
        {
            try
            {
                var declarations = await _declarationRepo.GetDeclarationsAsync(declarantId);
                return Ok(declarations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: update declarantion
        [Authorize]
        [HttpPost]
        [Route("{declarantId}/declarations/{declarationId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAsync(Guid declarantId, Guid declarationId, [FromBody] Declaration declaration)
        {
            try
            {
                // validations
                if (declaration == null) return BadRequest("Incorrect body format");
                await _declarationRepo.GetDeclarationByIdAsync(declarationId, declarantId);
                if (declaration.Id != declarationId) return BadRequest("Declaration Id from body incorrect");

                declaration.Id = declarationId;

                await _declarationValidator.ValidateAndThrowAsync(declaration);

                int result = await _declarationRepo.UpdateDeclarationAsync(declaration);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: delete declarantion
        [Authorize]
        [HttpDelete]
        [Route("{declarantId}/declarations/{declarationId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync(Guid declarantId, Guid declarationId)
        {
            try
            {
                Declaration declaration = await _declarationRepo.GetDeclarationByIdAsync(declarationId, declarantId);
                declaration.Deleted = true;
                int result = await _declarationRepo.UpdateDeclarationAsync(declaration);
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
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UndeleteAsync(Guid declarantId, Guid declarationId)
        {
            try
            {
                Declaration declaration = await _declarationRepo.GetDeclarationByIdAsync(declarationId, declarantId);
                declaration.Deleted = false;
                int result = await _declarationRepo.UpdateDeclarationAsync(declaration);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
