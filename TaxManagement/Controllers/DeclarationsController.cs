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
    [Authorize]
    public class DeclarationsController : Controller
    {
        private readonly ILogger<DeclarationsController> _logger;
        private readonly IValidator<DeclarationDTO> _declarationValidator;
        private readonly IValidator<CreateDeclarationDTO> _createDeclarationValidator;
        private readonly IValidator<UpdateDeclarationDTO> _updateDeclarationValidator;
        private readonly IMapper _mapper;
        private readonly IDeclarationService _declarationService;
        private readonly IDeclarantService _declarantService;

        public DeclarationsController(IDeclarantService declarantService,
            IDeclarationService declarationService,
            IValidator<DeclarationDTO> declarationValidator,
            IValidator<UpdateDeclarationDTO> updateDeclarationValidator,
            IValidator<CreateDeclarationDTO> createDeclarationValidator,
            ILogger<DeclarationsController> logger,
            IMapper mapper)
        {

            _declarationValidator = declarationValidator;
            _createDeclarationValidator = createDeclarationValidator;
            _updateDeclarationValidator = updateDeclarationValidator;
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

            // declaration validation
            ValidationResult validationResult = await _createDeclarationValidator.ValidateAsync(createDeclarationDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));
            // declarant validation
            var oldDeclarant = await _declarantService.DeclarantExists(declarantId);
            if (oldDeclarant is null) return NotFound("Declarant not found");

            string userName = User?.Identity?.Name ?? "na";

            var result = await _declarationService.CreateDeclarationAsync(createDeclarationDTO, userName);

            return result;
        }

        // GET: Get declaration(s)
        [HttpGet]
        [Route("{declarantId}/declarations")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<DeclarationDTO>>> GetAsync(Guid declarantId, bool includeDeleted = false)
        {
            // declarant validation
            var oldDeclarant = await _declarantService.DeclarantExists(declarantId);
            if (oldDeclarant is null) return NotFound("Declarant not found");

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

            // declarant validation
            var oldDeclarant = await _declarantService.DeclarantExists(declarantId);
            if (oldDeclarant is null) return NotFound("Declarant not found");

            // declaration validation
            ValidationResult validationResult = await _updateDeclarationValidator.ValidateAsync(updateDeclarationDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));
            if (!await _declarationService.DeclarationExists(declarationId)) return NotFound("Declaration not found");

            updateDeclarationDTO.LastUpdateByUser = User?.Identity?.Name ?? "na";
            updateDeclarationDTO.LastUpdateAt = DateTime.Now;

            var result = await _declarationService.UpdateDeclarationAsync(updateDeclarationDTO);

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
            // declarant validation
            var oldDeclarant = await _declarantService.DeclarantExists(declarantId);
            if (oldDeclarant is null) return NotFound("Declarant not found");

            // declaration validation
            if (!await _declarationService.DeclarationExists(declarationId)) return NotFound("Declaration not found");

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
            // declarant validation
            // declarant validation
            var oldDeclarant = await _declarantService.DeclarantExists(declarantId);
            if (oldDeclarant is null) return NotFound("Declarant not found");

            // declaration validation
            if (!await _declarationService.DeclarationExists(declarationId)) return NotFound("Declaration not found");

            string userName = User?.Identity?.Name ?? "na";
            var result = await _declarationService.SetDeletedDeclarationAsync(declarantId, declarationId, false, userName);

            return Ok(result);
        }


    }
}
