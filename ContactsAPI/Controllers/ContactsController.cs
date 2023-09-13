using ContactsAPI.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ContactsAPI.DTOs;
using AuthorizeAPI;

namespace ContactsAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("contacts")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactsService _contactsService;
        private readonly IValidator<CreateContactDto> _createContactValidator;
        private readonly IValidator<UpdateContactDTO> _updateContactValidator;

        public ContactsController(IContactsService contactsService
                          , IValidator<CreateContactDto> createContactValidator
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
        public async Task<ActionResult<ContactDetailedDto>> CreateAsync([FromBody] CreateContactDto contactDTO)
        {
            // validations
            if (contactDTO == null) return new BadRequestObjectResult("Incorrect body format");
            // contact validation
            ValidationResult validationResult = await _createContactValidator.ValidateAsync(contactDTO);
            if (!validationResult.IsValid) return new BadRequestObjectResult(validationResult.ToString("~"));

            var lastUser = User?.Identity?.Name;

            return await _contactsService.CreateAsync(contactDTO, lastUser);
        }

        [HttpPatch]
        [Route("{contactId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ContactDetailedDto>> UpdateAsync(Guid contactId, [FromBody] UpdateContactDTO contactDTO)
        {
            // validations
            if (contactDTO == null) return new BadRequestObjectResult("Incorrect body format");

            // contact validation
            ValidationResult validationResult = await _updateContactValidator.ValidateAsync(contactDTO);
            if (!validationResult.IsValid) return new BadRequestObjectResult(validationResult.ToString("~"));

            var lastUser = User?.Identity?.Name;

            return await _contactsService.UpdateContactAsync(contactId, contactDTO, lastUser);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ContactDTO>>> GetAsync(bool includeDeleted = false)
        {
            return await _contactsService.GetAsync(includeDeleted);
        }

        [HttpGet]
        [Route("search")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ContactDTO>>> SearchAsync(string query)
        {
            return await _contactsService.SearchAsync(query);
        }

        [HttpGet]
        [Route("paginated")]
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
        public async Task<ActionResult<ContactDetailedDto>> GetByIdAsync(Guid id)
        {
            var contact = await _contactsService.GetByIdAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

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
