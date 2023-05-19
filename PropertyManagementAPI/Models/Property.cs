using MongoDB.Bson.Serialization.Attributes;

namespace PropertyManagementAPI.Models
{
    public enum TypeOfUse
    {
        Private,
        VacationalRent,
        LongTermRent
    }

    public class Property
    {
        [BsonId]
        public Guid _id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public TypeOfUse[]? TypeOfUse { get; set; }
        public Address? Address { get; set; }
        public Cadastre? Cadastre { get; set; }
        public List<Owner> Owners { get; set; }
        public string? Comments { get; set; }
        public bool Deleted { get; set; }

        public Property()
        {
            Owners = new List<Owner>();
        }
    }
}
