using PropertiesAPI.Models;

namespace PropertiesAPI.Dtos
{
    public class CreatePropertyDto
    {
        public string Name { get; set; } = string.Empty; //required 
        public string? Type { get; set; }
        public TypeOfUse[]? TypeOfUse { get; set; }
        public AddressDto? Address { get; set; }
        public string? CadastreRef { get; set; }
        public string? Comments { get; set; }

        public Guid MainOwnerId { get; set; }
        public string MainOwnerType { get; set; } = string.Empty;

        public List<Guid?> ChildProperties { get; set; } = new();
        public Guid? ParentPropertyId { get; set; }
        
    }
}
