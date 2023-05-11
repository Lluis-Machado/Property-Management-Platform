using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Models;
using PropertyManagementAPI.Repositories;
using System.Net;

namespace PropertyManagementAPI.Controllers
{
    [Authorize]
    public class PropertiesController : Controller
    {
        private readonly ILogger<PropertiesController> _logger;
        private readonly IPropertiesRepository _propertiesRepo;
        private readonly IValidator<Property> _propertyValidator;
        private readonly IValidator<Address> _addressValidator;
        public PropertiesController(IPropertiesRepository propertiesRepo, ILogger<PropertiesController> logger, IValidator<Property> propertyValidator, IValidator<Address> addressValidator)
        {
            _propertiesRepo = propertiesRepo;
            _propertyValidator = propertyValidator;
            _addressValidator = addressValidator;
            _logger = logger;
        }

        // POST: Create property
        [HttpPost]
        [Route("properties")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]

        public async Task<ActionResult<Property>> CreateAsync([FromBody] Property property)
        {
            // validations
            if (property == null) return BadRequest("Incorrect body format");

            // property validation
            ValidationResult validationResult = await ValidateProperty(property);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            return Ok(await _propertiesRepo.CreateAsync(property));
        }

        // GET: Get properties(s)
        [HttpGet]
        [Route("properties")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]

        public async Task<ActionResult<IEnumerable<Property>>> GetAsync()
        {
            return Ok(await _propertiesRepo.GetAsync());
        }

        // POST: update property
        [HttpPatch]
        [Route("properties/{propertyId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> UpdateAsync([FromBody] Property property, Guid propertyId)
        {
            // validations
            if (property == null) return BadRequest("Incorrect body format");
            if (property._id != propertyId) return BadRequest("Property Id from body incorrect");

            // property validation
            ValidationResult validationResult = await ValidateProperty(property);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            property._id = propertyId; // copy id to pr object

            var UpdateResult = await _propertiesRepo.UpdateAsync(property);
            if (!UpdateResult.IsAcknowledged) return NotFound("Property not found");
            return NoContent();
        }

        // DELETE: delete property
        [HttpDelete]
        [Route("properties/{propertyId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid propertyId)
        {
            var updateResult = await _propertiesRepo.SetDeleteDeclarantAsync(propertyId, true);
            if (!updateResult.IsAcknowledged) return NotFound("Property not found");
            return NoContent();
        }

        // POST: undelete property
        [HttpPatch]
        [Route("properties/{propertyId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid propertyId)
        {
            var updateResult = await _propertiesRepo.SetDeleteDeclarantAsync(propertyId, false);
            if (!updateResult.IsAcknowledged) return NotFound("Property not found");
            return NoContent();
        }

        private async Task<ValidationResult> ValidateProperty(Property property)
        {
            // property validation
            ValidationResult validationResult = await _propertyValidator.ValidateAsync(property);
            if (!validationResult.IsValid) return validationResult;

            // address validation
            if (property.Address != null) validationResult = await _addressValidator.ValidateAsync(property.Address);
            
            return validationResult;
        }


    }
}
