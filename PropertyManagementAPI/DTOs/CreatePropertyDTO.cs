using PropertiesAPI.Models;

namespace PropertiesAPI.DTOs
{
    public class CreatePropertyDto
    {
        public string Name { get; set; } = string.Empty; //required 
        public string? Type { get; set; }
        public TypeOfUse[]? TypeOfUse { get; set; }
        public AddressDTO? Address { get; set; }
        public string? CadastreRef { get; set; }
        public string? Comments { get; set; }

        public Guid? ParentPropertyId { get; set; }
        public List<CreatePropertyOwnershipDto> Ownerships { get; set; }  = new List<CreatePropertyOwnershipDto>(); //required
        public List<Guid?> ChildProperties { get; set; } = new();
    }
}
