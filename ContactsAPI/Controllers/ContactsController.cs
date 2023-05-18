using ContactsAPI.Models;
using ContactsAPI.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ContactsAPI.Controllers
{
    public class ContactsController : Controller
    {
        //private readonly ILogger<ContactsController> _logger;
        private readonly IContactsRepository _contactsRepo;
        private readonly IValidator<Contact> _contactValidator;
        private readonly IValidator<Address> _addressValidator;
        public ContactsController(IContactsRepository contactsRepo, IValidator<Contact> contactValidator, IValidator<Address> addressValidator)
        {
            _contactsRepo = contactsRepo;
            _contactValidator = contactValidator;
            _addressValidator = addressValidator;
            //_logger = logger;
        }
        [HttpPost]
        [Route("contacts")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Contact>> CreateAsync([FromBody] Contact contact)
        {
            // validations
            if (contact == null) return BadRequest("Incorrect body format");
            if (contact._id != Guid.Empty) return BadRequest("Id fild must be empty");

            // contact validation
            //ValidationResult validationResult = await ValidateProperty(contact);
            //if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));
            contact = await _contactsRepo.InsertOneAsync(contact);
            return Created($"contacts/{contact._id}", contact);
        }

        // GET: Get contacts(s)
        [HttpGet]
        [Route("contacts")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]

        public async Task<ActionResult<IEnumerable<Contact>>> GetAsync()
        {
            return Ok(await _contactsRepo.GetAsync());
        }

        // POST: update contact
        [HttpPatch]
        [Route("contacts/{contactId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> UpdateAsync([FromBody] Contact contact, Guid contactId)
        {
            // validations
            if (contact == null) return BadRequest("Incorrect body format");
            if (contact._id != contactId) return BadRequest("contact Id from body incorrect");

            // contact validation
            ValidationResult validationResult = await ValidateProperty(contact);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            contact._id = contactId; // copy id to pr object

            var UpdateResult = await _contactsRepo.UpdateAsync(contact);
            if (!UpdateResult.IsAcknowledged) return NotFound("contact not found");
            return NoContent();
        }

        // DELETE: delete contact
        [HttpDelete]
        [Route("contacts/{contactId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid contactId)
        {
            var updateResult = await _contactsRepo.SetDeleteAsync(contactId, true);
            if (!updateResult.IsAcknowledged) return NotFound("contact not found");
            return NoContent();
        }

        // POST: undelete contact
        [HttpPatch]
        [Route("contacts/{contactId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid contactId)
        {
            var updateResult = await _contactsRepo.SetDeleteAsync(contactId, false);
            if (!updateResult.IsAcknowledged) return NotFound("contact not found");
            return NoContent();
        }

        private async Task<ValidationResult> ValidateProperty(Contact contact)
        {
            // contact validation
            ValidationResult validationResult = await _contactValidator.ValidateAsync(contact);
            if (!validationResult.IsValid) return validationResult;

            // address validation
            if (contact.Address != null) validationResult = await _addressValidator.ValidateAsync(contact.Address);

            return validationResult;
        }

    }
}
