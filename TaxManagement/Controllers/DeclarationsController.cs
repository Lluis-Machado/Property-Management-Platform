using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaxManagementAPI.DTOs;
using TaxManagementAPI.Services;

namespace TaxManagement.Controllers
{
#if PRODUCTION
    [Authorize]
#endif
    public class DeclarationsController : Controller
    {
        private readonly ILogger<DeclarationsController> _logger;
        private readonly IMapper _mapper;
        private readonly IDeclarationService _declarationService;
        private readonly IDeclarantService _declarantService;

        public DeclarationsController(IDeclarantService declarantService,
            IDeclarationService declarationService,
            ILogger<DeclarationsController> logger,
            IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _declarationService = declarationService;
            _declarantService = declarantService;
        }

        // POST: Create declaration
        [HttpPost]
        [Route("{declarantId}/declarations")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<DeclarationDTO>> CreateAsync([FromBody] CreateDeclarationDTO createDeclarationDTO, Guid declarantId)
        {
            // request validations
            if (createDeclarationDTO == null) return BadRequest("Incorrect body format");
            if (createDeclarationDTO.DeclarantId != declarantId) return BadRequest("Incorrect declarant Id in body");

            string userName = User?.Identity?.Name ?? "na";

            var result = await _declarationService.CreateDeclarationAsync(createDeclarationDTO, declarantId, userName);

            return result;
        }

        // GET: Get declaration(s)
        [HttpGet]
        [Route("{declarantId}/declarations")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<DeclarationDTO>>> GetAsync(Guid declarantId, bool includeDeleted = false)
        {
            return Ok(await _declarationService.GetDeclarationsAsync(declarantId, includeDeleted));
        }

        // POST: update declaration
        [HttpPatch]
        [Route("{declarantId}/declarations/{declarationId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAsync(Guid declarantId, Guid declarationId, [FromBody] UpdateDeclarationDTO updateDeclarationDTO)
        {
            // validations
            if (updateDeclarationDTO == null) return BadRequest("Incorrect body format");
            //if (updateDeclarationDTO.Id != declarationId) return BadRequest("Declaration Id from body incorrect");

            updateDeclarationDTO.LastUpdateByUser = User?.Identity?.Name ?? "na";
            updateDeclarationDTO.LastUpdateAt = DateTime.Now;

            var result = await _declarationService.UpdateDeclarationAsync(updateDeclarationDTO, declarantId, declarationId);

            return Ok(result);
        }

        // DELETE: delete declarantion
        [HttpDelete]
        [Route("{declarantId}/declarations/{declarationId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync(Guid declarantId, Guid declarationId)
        {
            string userName = User?.Identity?.Name ?? "na";
            var result = await _declarationService.SetDeletedDeclarationAsync(declarantId, declarationId, true, userName);

            return Ok(result);
        }

        // POST: undelete declarant
        [HttpPatch]
        [Route("{declarantId}/declarations/{declarationId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid declarantId, Guid declarationId)
        {
            string userName = User?.Identity?.Name ?? "na";
            var result = await _declarationService.SetDeletedDeclarationAsync(declarantId, declarationId, false, userName);

            return Ok(result);
        }

    }
}
