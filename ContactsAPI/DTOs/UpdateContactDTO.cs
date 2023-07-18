namespace ContactsAPI.DTOs
{
    public class UpdateContactDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; } 
        public DateOnly? BirthDay { get; set; }
        public string? Nif { get; set; } 
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MobilePhoneNumber { get; set; }
        public AddressDto Address { get; set; }

    }
}