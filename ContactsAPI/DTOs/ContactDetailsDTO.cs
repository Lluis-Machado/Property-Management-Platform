using ContactsAPI.DTOs;
using ContactsAPI.Models;

namespace ContactsAPI.Models
{
    public class ContactDetailsDTO : ContactDTO 
    {
        public List<OwnershipInfoDTO>? OwnershipInfo { get; set; } = new List<OwnershipInfoDTO>();
    }

    
}
