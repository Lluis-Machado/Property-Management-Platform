using MongoDB.Bson.Serialization.Attributes;

namespace PropertiesAPI.Models
{
    public class Property : BaseModel
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public TypeOfUse[]? TypeOfUse { get; set; }
        public PropertyAddress PropertyAddress { get; set; } = new PropertyAddress();
        
        public string? CadastreRef { get; set; }
        public string? Comments { get; set; }

        public Guid? ParentPropertyId { get; set; }
        public Guid MainContactId { get; set; }

    }

    public enum TypeOfUse
    {
        Private,
        VacationalRent,
        LongTermRent
    }
}
