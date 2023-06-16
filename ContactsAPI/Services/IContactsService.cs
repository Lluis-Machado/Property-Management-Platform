using ContactsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContactsAPI.Services
{
    public interface IContactsService
    {
        Task<ActionResult<ContactDTO>> CreateContactAsync(CreateContactDTO contact);
        Task<ActionResult<ContactDTO>> UpdateContactAsync(UpdateContactDTO contact, Guid contactId);
        Task<ActionResult<IEnumerable<ContactDTO>>> GetContactsAsync();
        Task<ContactDTO> GetContactByIdAsync(Guid id);
        Task<ContactDetailsDTO> GetContactWithProperties(Guid contactId);
        Task<IActionResult> DeleteContactAsync(Guid contactId);
        Task<IActionResult> UndeleteContactAsync(Guid contactId);
    }
}
