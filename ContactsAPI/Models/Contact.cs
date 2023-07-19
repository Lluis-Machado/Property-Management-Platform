namespace ContactsAPI.Models
{
    public class Contact : BaseModel
    {
        public string? FirstName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public DateOnly? BirthDay { get; set; }
        public string Nif { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MobilePhoneNumber { get; set; }
        public ContactAddress Address { get; set; } 
    }
}
