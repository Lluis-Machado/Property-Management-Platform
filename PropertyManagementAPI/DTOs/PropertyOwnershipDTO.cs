namespace PropertiesAPI.DTOs
{
    public class PropertyOwnershipDto
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public Guid PropertyId { get; set; }
        public decimal Share { get; set; } = 0;
        public bool MainOwnership { get; set; }
        public ContactDto ContactDetail { get; set; } = new();
        public bool Deleted { get; set; } = false;
        public PropertyOwnershipDto() { }
    }
}