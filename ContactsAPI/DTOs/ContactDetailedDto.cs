namespace ContactsAPI.DTOs
{
    public class ContactDetailedDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string LastName { get; set; } = string.Empty; //required
        public DateOnly? BirthDay { get; set; }
        public string Nif { get; set; } = string.Empty; 
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MobilePhoneNumber { get; set; }
        public AddressDto? Address { get; set; }
    }
}
