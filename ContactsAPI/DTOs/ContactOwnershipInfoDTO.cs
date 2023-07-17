namespace ContactsAPI.DTOs
{
    public class ContactOwnershipInfoDTO
    {
        public string PropertyName { get; set; } = string.Empty;
        public Guid PropertyId { get; set; }

        public decimal Share { get; set; } = 0;

        public ContactOwnershipInfoDTO(string pName, decimal share, Guid propertyId)
        {
            PropertyName = pName;
            Share = share;
            PropertyId = propertyId;
        }
        public ContactOwnershipInfoDTO()
        {
            Share = 0;
            PropertyName = string.Empty;
        }
    }
}
