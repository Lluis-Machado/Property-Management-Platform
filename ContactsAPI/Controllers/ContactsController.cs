using ContactsAPI.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ContactsAPI.DTOs;

namespace ContactsAPI.Controllers
{
    //[Authorize]
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
        public async Task<ActionResult<ContactDetailedDTO>> CreateAsync([FromBody] CreateContactDTO contactDTO)
        {
            // validations
            if (contactDTO == null) return new BadRequestObjectResult("Incorrect body format");
            // contact validation
            ValidationResult validationResult = await _createContactValidator.ValidateAsync(contactDTO);
            if (!validationResult.IsValid) return new BadRequestObjectResult(validationResult.ToString("~"));

            var lastUser = "test";

            return await _contactsService.CreateContactAsync(contactDTO, lastUser);
        }

        [HttpPatch]
        [Route("{contactId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ContactDetailedDTO>> UpdateAsync(Guid contactId, [FromBody] UpdateContactDTO contactDTO)
        {
            // validations
            if (contactDTO == null) return new BadRequestObjectResult("Incorrect body format");

            // contact validation
            ValidationResult validationResult = await _updateContactValidator.ValidateAsync(contactDTO);
            if (!validationResult.IsValid) return new BadRequestObjectResult(validationResult.ToString("~"));

            var lastUser = "test";

            return await _contactsService.UpdateContactAsync(contactId, contactDTO, lastUser);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ContactDTO>>> GetAsync(bool includeDeteted = false)
        {
            return await _contactsService.GetContactsAsync(includeDeteted);
        }

        [HttpGet]
        [Route("declarants/paginated")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ContactDTO>>> GetPaginatedAsync(int pageNumber, int pageSize)
        {
            try
            {
                IEnumerable<ContactDTO> paginatedContacts = await _contactsService.GetPaginatedContactsAsync(pageNumber, pageSize);
                return Ok(paginatedContacts);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ContactDetailedDTO>> GetByIdAsync(Guid id)
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
            var lastUser = "test";

            return await _contactsService.DeleteContactAsync(contactId, lastUser);
        }

        [HttpPatch]
        [Route("{contactId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid contactId)
        {
            var lastUser = "test";

            return await _contactsService.UndeleteContactAsync(contactId, lastUser);
        }
    }
}
