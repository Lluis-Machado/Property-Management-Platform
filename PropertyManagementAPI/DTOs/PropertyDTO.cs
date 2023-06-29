using MongoDB.Bson.Serialization.Attributes;
using PropertyManagementAPI.Models;

namespace PropertyManagementAPI.DTOs
{
    public class PropertyDTO
    {
        // Property
        public string? Name { get; set; }
        public string? Type { get; set; }
        public TypeOfUse[]? TypeOfUse { get; set; }
        public Address? Address { get; set; }
        public Cadastre? Cadastre { get; set; }
        public string? Comments { get; set; }

        // Base Model
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string? CreatedByUser { get; set; }
        public string? LastUpdateByUser { get; set; }
        public bool Deleted { get; set; }

    }
}
