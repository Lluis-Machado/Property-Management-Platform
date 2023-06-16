using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.DTOs;
using PropertyManagementAPI.Models;
using PropertyManagementAPI.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("properties")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertiesService _propertiesService;
        private readonly IValidator<CreatePropertyDTO> _createPropertyValidator;
        private readonly IValidator<UpdatePropertyDTO> _updatePropertyValidator;
        private readonly IValidator<PropertyDTO> _propertyValidator;


        public PropertiesController(IPropertiesService propertiesService
            , IValidator<CreatePropertyDTO> createPropertyValidator
            , IValidator<UpdatePropertyDTO> updatePropertyValidator
            , IValidator<PropertyDTO> propertyValidator)
        {
            _propertiesService = propertiesService;
            _propertyValidator = propertyValidator;
            _createPropertyValidator = createPropertyValidator;
            _updatePropertyValidator = updatePropertyValidator;
        }

        // POST: Create property
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PropertyDTO>> CreateAsync([FromBody] CreatePropertyDTO propertyDTO)
        {
            // validations
            if (propertyDTO == null)    return BadRequest("Incorrect body format");
            // property validation
            ValidationResult validationResult = await _createPropertyValidator.ValidateAsync(propertyDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            string userName = "user";

            return await _propertiesService.CreateProperty(propertyDTO, userName);          
        }

        // GET: Get properties(s)
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<PropertyDTO>>> GetAsync()
        {
            var properties = await _propertiesService.GetProperties();
            return (properties);          
        }

        // GET: Get properties(s) by contactId
        [HttpGet("{propertyId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PropertyDTO>> GetContactProperties(Guid propertyId)
        {
            var property = await _propertiesService.GetProperty(propertyId);
            return property;

        }

        // PATCH: Update property
        [HttpPatch("{propertyId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<PropertyDTO>> UpdateAsync(Guid propertyId, [FromBody] UpdatePropertyDTO propertyDTO)
        {
            // Validations
            if (propertyDTO == null)
                return new BadRequestObjectResult("Incorrect body format");
            if (propertyDTO.Id != propertyId)
                return new BadRequestObjectResult("Declarant Id from body incorrect");

            // property validation
            ValidationResult validationResult = await _updatePropertyValidator.ValidateAsync(propertyDTO);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            if (!await _propertiesService.PropertyExists(propertyId)) return NotFound("Property not found");

            string lastUpdateByUser = "a";// User?.Identity?.Name;

            var result = await _propertiesService.UpdateProperty(propertyId, propertyDTO, lastUpdateByUser);
            return Ok(result);         
        }

        // DELETE: Delete property
        [HttpDelete("{propertyId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid propertyId)
        {
            if (!await _propertiesService.PropertyExists(propertyId)) return NotFound("Property not found");

            string lastUserName = "aa"; // User?.Identity?.Name

            return await _propertiesService.DeleteProperty(propertyId, lastUserName);
        }

        // PATCH: Undelete property
        [HttpPatch("{propertyId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid propertyId)
        {
            if (!await _propertiesService.PropertyExists(propertyId)) return NotFound("Property not found");

            string lastUserName = "aa"; // User?.Identity?.Name

            return await _propertiesService.UndeleteProperty(propertyId, lastUserName);
        }
    }
}
