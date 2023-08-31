namespace ContactsAPI.Models
{
    public class ContactAddress
    {
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public int? State { get; set; }
        public string? PostalCode { get; set; }
        public string? ShortComment { get; set; }

        public int? Country { get; set; }
        public int? AddressType { get; set; }
    }
}
