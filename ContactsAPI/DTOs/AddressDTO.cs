namespace ContactsAPI.DTOs
{
    public class AddressDto
    {
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public int State { get; set; }
        public string? PostalCode { get; set; }
        public int Country { get; set; }
    }
}