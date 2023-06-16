using ContactsAPI.Models;
using ContactsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace ContactsAPI.Controllers
{
   // [Authorize]
    [ApiController]
    [Route("contacts")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactsService _contactsService;
        private readonly IValidator<CreateContactDTO> _createContactValidator;
        private readonly IValidator<UpdateContactDTO> _updateContactValidator;

        public ContactsController(IContactsService contactsService
                          , IValidator<CreateContactDTO> createContactValidator
                          , IValidator<UpdateContactDTO> updateContactValidator)
        {
            _contactsService = contactsService;
            _createContactValidator = createContactValidator;
            _updateContactValidator = updateContactValidator;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ContactDTO>> CreateAsync([FromBody] CreateContactDTO contactDTO)
        {
            // validations
            if (contactDTO == null) return new BadRequestObjectResult("Incorrect body format");
            // contact validation
            ValidationResult validationResult = await _createContactValidator.ValidateAsync(contactDTO);
            if (!validationResult.IsValid) return new BadRequestObjectResult(validationResult.ToString("~"));

            return await _contactsService.CreateContactAsync(contactDTO);
        }

        [HttpPatch]
        [Route("{contactId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ContactDTO>> UpdateAsync([FromBody] UpdateContactDTO contactDTO, Guid contactId)
        {
            // validations
            if (contactDTO == null) return new BadRequestObjectResult("Incorrect body format");
            if (contactDTO.Id != contactId) return new BadRequestObjectResult("Contact Id from body is incorrect");

            // contact validation
            ValidationResult validationResult = await _updateContactValidator.ValidateAsync(contactDTO);
            if (!validationResult.IsValid) return new BadRequestObjectResult(validationResult.ToString("~"));


            return await _contactsService.UpdateContactAsync(contactDTO, contactId);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ContactDTO>>> GetAsync()
        {
            return await _contactsService.GetContactsAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ContactDTO>> GetByIdAsync(Guid id)
        {
            var contact = await _contactsService.GetContactByIdAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }

        [HttpGet("{contactId}/properties")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ContactDetailsDTO>> GetContactWithProperties(Guid contactId)
        {
            var contact = await _contactsService.GetContactWithProperties(contactId);

            return contact;
        }

        [HttpDelete]
        [Route("{contactId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid contactId)
        {
            return await _contactsService.DeleteContactAsync(contactId);
        }

        [HttpPatch]
        [Route("{contactId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid contactId)
        {
            return await _contactsService.UndeleteContactAsync(contactId);
        }
    }
}
