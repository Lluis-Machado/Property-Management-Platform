using MongoDB.Bson.Serialization.Attributes;
using PropertiesAPI.Models;

namespace PropertiesAPI.DTOs
{
    public class PropertyDetailedDto : PropertyDto
    {
        public List<PropertyOwnershipDto> Ownerships { get; set; } = new();
        public List<BasicPropertyDto> ChildProperties { get; set; } = new();

    }
}
