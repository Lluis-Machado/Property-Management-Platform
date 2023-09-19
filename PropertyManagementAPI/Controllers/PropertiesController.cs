using Microsoft.AspNetCore.Mvc;
using PropertiesAPI.Dtos;
using PropertiesAPI.Services;
using System.Net;

namespace PropertiesAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("properties")]
    public class PropertyApi : ControllerBase
    {
        private readonly IPropertiesService _propertiesService;

        public PropertyApi(IPropertiesService propertiesService)
        {
            _propertiesService = propertiesService;
        }

        // POST: Create property
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PropertyDetailedDto>> CreateAsync([FromBody] CreatePropertyDto propertyDto)
        {
            // validations
            if (propertyDto is null) return BadRequest("Incorrect body format");

            // Check user
            var username = "user";// UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            var result = await _propertiesService.CreateProperty(propertyDto, username);
            return result;
        }

        // GET: Get properties(s)
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetAsync()
        {
            var results = await _propertiesService.GetProperties();
            return (results);
        }

        // GET: Get properties(s) by contactId
        [HttpGet("{propertyId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PropertyDetailedDto>> GetAsync(Guid propertyId)
        {
            var result = await _propertiesService.GetProperty(propertyId);
            return result;

        }

        // PATCH: Update property
        [HttpPatch("{propertyId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<PropertyDetailedDto>> UpdateAsync([FromBody] UpdatePropertyDto propertyDTO, Guid propertyId)
        {
            // Validations
            if (propertyDTO is null) return new BadRequestObjectResult("Incorrect body format");

            string username = User?.Identity?.Name ?? "na";

            var result = await _propertiesService.UpdateProperty(propertyDTO, username, propertyId);
            return result;
        }

        // PATCH: Update property archive id
        [HttpPatch("{propertyId}/{archiveId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdatePropertyArchiveIdAsync(Guid propertyId, Guid archiveId)
        {
            //string username = "user";// UserNameValidator.GetValidatedUserName(User?.Identity?.Name);
            string username = User?.Identity?.Name ?? "na";

            return await _propertiesService.UpdatePropertyArchiveIdAsync(propertyId, archiveId, username);
        }

        // DELETE: Delete property
        [HttpDelete("{propertyId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid propertyId)
        {
            string username = User?.Identity?.Name ?? "na";

            var result = await _propertiesService.DeleteProperty(propertyId, username);
            return result;
        }

        // PATCH: Undelete property
        [HttpPatch("{propertyId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid propertyId)
        {
            string username = User?.Identity?.Name ?? "na";

            var result = await _propertiesService.UndeleteProperty(propertyId, username);
            return result;
        }
    }
}
