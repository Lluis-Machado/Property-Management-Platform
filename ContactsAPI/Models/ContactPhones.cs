namespace ContactsAPI.Models
{
    public class ContactPhones
    {
        public int PhoneType { get; set; }
        public int Type { get; set; }
        public int CountryMaskId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string ShortComment { get; set; } = string.Empty;


    }
}
