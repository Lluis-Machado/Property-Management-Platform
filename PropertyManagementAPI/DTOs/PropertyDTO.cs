using MongoDB.Bson.Serialization.Attributes;
using PropertiesAPI.Models;

namespace PropertiesAPI.DTOs
{
    public class PropertyDto
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public TypeOfUse[]? TypeOfUse { get; set; }
        public AddressDTO Address { get; set; } = new();
        public string? CadastreRef { get; set; }
        public string? Comments { get; set; }

        public ContactDto MainContact { get; set; } = new ContactDto();
        public BasicPropertyDto? ParentProperty { get; set; }

        // Base Model
        public Guid Id { get; set; }

    }
}
