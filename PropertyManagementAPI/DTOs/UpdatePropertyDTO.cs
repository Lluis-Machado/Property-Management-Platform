using MongoDB.Bson.Serialization.Attributes;
using PropertiesAPI.Models;

namespace PropertiesAPI.DTOs
{
    public class UpdatePropertyDTO
    {
        public string Name { get; set; } = string.Empty; //required 
        public string? Type { get; set; }
        public TypeOfUse[]? TypeOfUse { get; set; }
        public AddressDTO? Address { get; set; }
        public string? CadastreRef { get; set; }
        public string? Comments { get; set; }

        public Guid? ParentPropertyId { get; set; }
        public List<PropertyOwnershipDto> Ownerships { get; set; } = new List<PropertyOwnershipDto>(); //required
        public List<Guid?> ChildProperties { get; set; } = new();

    }
}
