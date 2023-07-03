namespace ContactsAPI.DTOs
{
    public class ContactOwnershipInfoDTO
    {
        public string PropertyName { get; set; } = string.Empty;
        public decimal Share { get; set; } = 0;

        public ContactOwnershipInfoDTO(string pName, decimal share)
        {
            PropertyName = pName;
            Share = share;
        }
        public ContactOwnershipInfoDTO()
        {
            Share = 0;
            PropertyName = string.Empty;
        }
    }
}
