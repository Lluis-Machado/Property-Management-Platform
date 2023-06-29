using MongoDB.Bson.Serialization.Attributes;
using PropertyManagementAPI.Models;

namespace PropertyManagementAPI.DTOs
{
    public class CreatePropertyDTO
    {
        // Property
        public string? Name { get; set; }
        public string? Type { get; set; }
        public TypeOfUse[]? TypeOfUse { get; set; }
        public Address? Address { get; set; }
        public Cadastre? Cadastre { get; set; }
        public string? Comments { get; set; }

        // Base Model
        public Guid TenantId { get; set; }

    }
}
