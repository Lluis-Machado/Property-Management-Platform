using ContactsAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ContactsAPI.Services
{
    public interface IContactsService
    {
        Task<ActionResult<ContactDetailedDTO>> CreateContactAsync(CreateContactDTO contact, string lastUser);
        Task<ActionResult<ContactDetailedDTO>> UpdateContactAsync(Guid contactId, UpdateContactDTO contact, string lastUser);
        Task<ActionResult<IEnumerable<ContactDTO>>> GetContactsAsync(bool includeDeleted = false);
        Task<ContactDetailedDTO> GetContactByIdAsync(Guid contactId);
        Task<ContactDetailsDTO> GetContactWithProperties(Guid contactId);
        Task<IActionResult> DeleteContactAsync(Guid contactId, string lastUser);
        Task<IActionResult> UndeleteContactAsync(Guid contactId, string lastUser);
        Task<IEnumerable<ContactDTO>> GetPaginatedContactsAsync(int pageNumber, int pageSize);

    }
}
