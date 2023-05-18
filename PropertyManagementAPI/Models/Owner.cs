using MongoDB.Bson.Serialization.Attributes;

namespace PropertyManagementAPI.Models
{
    public class Owner
    {
        [BsonId]
        public Guid _id { get; set; }
    }
}
