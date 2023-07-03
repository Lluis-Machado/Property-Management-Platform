namespace PropertiesAPI.DTOs
{
    public class ContactDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        
        public Guid Id { get; set; } 
    }
}
