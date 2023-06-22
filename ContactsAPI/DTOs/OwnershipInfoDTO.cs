namespace ContactsAPI.DTOs
{
    public class OwnershipInfoDTO
    {
        public string PropertyName { get; set; }
        public decimal Share { get; set; }

        public OwnershipInfoDTO(string pName, decimal share)
        {
            PropertyName = pName;
            Share = share;
        }
        public OwnershipInfoDTO()
        {
            Share = 0;
            PropertyName = string.Empty;
        }
    }
}
