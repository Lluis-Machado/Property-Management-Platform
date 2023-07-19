using PropertiesAPI.Models;

namespace PropertiesAPI.Dtos
{
    public class PropertyDto
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public TypeOfUse[]? TypeOfUse { get; set; }
        public AddressDto Address { get; set; } = new();
        public string? CadastreRef { get; set; }
        public string? CadastreUrl { get; set; }
        public string? Comments { get; set; }

        public OwnerDto MainOwner { get; set; } = new OwnerDto();
        public BasicPropertyDto ParentProperty { get; set; } = new();


        // Base Model
        public Guid Id { get; set; }

    }
}
