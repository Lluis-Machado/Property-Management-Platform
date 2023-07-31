using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaxManagementAPI.DTOs;
using TaxManagementAPI.Services;

namespace TaxManagement.Controllers
{
#if PRODUCTION
    [Authorize]
#endif
    public class DeclarantsController : Controller
    {
        private readonly IDeclarantService _declarantService;

        private readonly ILogger<DeclarantsController> _logger;

        public DeclarantsController(IDeclarantService declarantService, ILogger<DeclarantsController> logger)
        {
            _declarantService = declarantService;
            _logger = logger;
        }

        // POST: Create declarant
        [HttpPost]
        [Route("declarants")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<DeclarantDTO>> CreateAsync([FromBody] CreateDeclarantDTO createDeclarantDTO)
        {
            string userName = User?.Identity?.Name ?? "na";

            DeclarantDTO createdDeclarant = await _declarantService.CreateDeclarantAsync(createDeclarantDTO, userName);

            return Created($"declarants/{createdDeclarant.Id}", createdDeclarant);
        }

        // GET: Get declarants(s)
        [HttpGet]
        [Route("declarants")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<DeclarantDTO>>> GetAsync(bool includeDeleted = false)
        {
            IEnumerable<DeclarantDTO> declarantsDTO = await _declarantService.GetDeclarantsAsync(includeDeleted);
            return Ok(declarantsDTO);
        }

        [HttpGet]
        [Route("declarants/paginated")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<DeclarantDTO>>> GetPaginatedAsync(int pageNumber, int pageSize)
        {

            IEnumerable<DeclarantDTO> paginatedDeclarants = await _declarantService.GetPaginatedDeclarantsAsync(pageNumber, pageSize);
            return Ok(paginatedDeclarants);
        }


        // POST: update declarant
        [HttpPatch]
        [Route("declarants/{declarantId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<DeclarantDTO>> UpdateAsync([FromBody] UpdateDeclarantDTO declarantDTO, Guid declarantId)
        {
            // Validations
            if (declarantDTO == null)
                return new BadRequestObjectResult("Incorrect body format");

            if (declarantDTO.Id != declarantId)
                return new BadRequestObjectResult("Declarant Id from body incorrect");

            string userName = User?.Identity?.Name ?? "na";

            var result = await _declarantService.UpdateDeclarantAsync(declarantDTO, declarantId, userName);

            return Ok(result);
        }

        // DELETE: delete declarant
        [HttpDelete]
        [Route("declarants/{declarantId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid declarantId)
        {
            string lastUserName = User?.Identity?.Name ?? "na";

            var result = await _declarantService.DeleteDeclarantAsync(declarantId, lastUserName);

            return Ok(result);
        }

        // POST: undelete declarant
        [HttpPatch]
        [Route("declarants/{declarantId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid declarantId)
        {

            string lastUserName = User?.Identity?.Name ?? "na";

            var result = await _declarantService.UndeleteDeclarantAsync(declarantId, lastUserName);

            return Ok(result);
        }
    }
}