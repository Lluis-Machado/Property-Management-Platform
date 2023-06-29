using MongoDB.Bson.Serialization.Attributes;

namespace PropertyManagementAPI.Models
{
    public enum TypeOfUse
    {
        Private,
        VacationalRent,
        LongTermRent
    }

    public class Property : BaseModel
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public TypeOfUse[]? TypeOfUse { get; set; }
        public Address? Address { get; set; }
        public Cadastre? Cadastre { get; set; }
        public string? Comments { get; set; }
    }
}
