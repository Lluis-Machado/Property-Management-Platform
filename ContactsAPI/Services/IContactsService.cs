using ContactsAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ContactsAPI.Services
{
    public interface IContactsService
    {
        Task<ActionResult<ContactDetailedDto>> CreateAsync(CreateContactDto contact, string lastUser);
        Task<ActionResult<ContactDetailedDto>> UpdateContactAsync(Guid contactId, UpdateContactDTO contact, string lastUser);
        Task<ActionResult<IEnumerable<ContactDTO>>> GetAsync(bool includeDeleted = false);
        Task<ActionResult<IEnumerable<ContactDTO>>> SearchAsync(string query);
        Task<ContactDetailedDto> GetByIdAsync(Guid contactId);
        Task<ContactDetailsDTO> GetWithProperties(Guid contactId);
        Task<IActionResult> DeleteContactAsync(Guid contactId, string lastUser);
        Task<IActionResult> UndeleteContactAsync(Guid contactId, string lastUser);
        Task<IEnumerable<ContactDTO>> GetPaginatedContactsAsync(int pageNumber, int pageSize);

    }
}