namespace ContactsAPI.DTOs
{
    public class ContactDetailsDTO : ContactDTO
    {
        public List<ContactOwnershipInfoDTO>? OwnershipInfo { get; set; } = new List<ContactOwnershipInfoDTO>();
    }


}
