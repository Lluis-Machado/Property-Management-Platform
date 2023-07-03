using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertiesAPI.DTOs;
using PropertiesAPI.Models;
using PropertiesAPI.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using PropertiesAPI.Validators;

namespace PropertiesAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("properties")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertiesService _propertiesService;

        public PropertiesController(IPropertiesService propertiesService)
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
            if (propertyDto is null)    return BadRequest("Incorrect body format");

            // Check user
            var userName = "user";// UserNameValidator.GetValidatedUserName(User?.Identity?.Name);
            
            var result =  await _propertiesService.CreateProperty(propertyDto, userName);
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
        public async Task<ActionResult<PropertyDto>> UpdateAsync([FromBody] UpdatePropertyDTO propertyDTO, Guid propertyId)
        {
            // Validations
            if (propertyDTO is null) return new BadRequestObjectResult("Incorrect body format");

            string userName = "user";// UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            var result = await _propertiesService.UpdateProperty(propertyDTO, userName, propertyId);
            return Ok(result);         
        }

        // DELETE: Delete property
        [HttpDelete("{propertyId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid propertyId)
        {
            string userName = "user";// UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            var result = await _propertiesService.DeleteProperty(propertyId, userName);
            return result;
        }

        // PATCH: Undelete property
        [HttpPatch("{propertyId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid propertyId)
        {
            string userName = "user";// UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            var result = await _propertiesService.UndeleteProperty(propertyId, userName);
            return result;
        }
    }
}
