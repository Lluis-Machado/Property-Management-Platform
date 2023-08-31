namespace PropertiesAPI.Dtos
{
    public class PropertyDetailedDto : PropertyDto
    {
        public List<PropertyDto> ChildProperties { get; set; } = new List<PropertyDto>();
    }
}
