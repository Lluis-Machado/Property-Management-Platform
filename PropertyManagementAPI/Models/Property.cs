namespace PropertiesAPI.Models
{
    public class Property : BaseModel
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public TypeOfUse[]? TypeOfUse { get; set; }

        public string? CadastreRef { get; set; }
        public string? CadastreUrl { get; set; }
        public string? Comments { get; set; }

        public PropertyAddress PropertyAddress { get; set; } = new PropertyAddress();
        
        public Guid? ParentPropertyId { get; set; }

        public string MainOwnerType { get; set; } = string.Empty;
        public Guid MainOwnerId { get; set; }

    }

    public enum TypeOfUse
    {
        Private,
        VacationalRent,
        LongTermRent
    }
}