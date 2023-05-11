using MongoDB.Bson.Serialization.Attributes;

namespace PropertyManagementAPI.Models
{
    public class Property: CoreObject
    {
        [BsonId]
        public Guid _id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public Address? Address { get; set; }
        public bool Deleted { get; set; }
    }
}
