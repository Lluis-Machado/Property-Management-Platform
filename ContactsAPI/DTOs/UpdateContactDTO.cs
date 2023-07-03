namespace ContactsAPI.DTOs
{
    public class UpdateContactDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; } 
        public DateOnly? BirthDay { get; set; }
        public string? NIE { get; set; } 
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MobilePhoneNumber { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
    }
}
