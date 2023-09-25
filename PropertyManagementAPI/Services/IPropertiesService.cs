using Microsoft.AspNetCore.Mvc;
using PropertiesAPI.Dtos;

namespace PropertiesAPI.Services
{
    public interface IPropertiesService
    {
        Task<ActionResult<PropertyDetailedDto>> CreateProperty(CreatePropertyDto propertyDto, string lastUser);
        Task<ActionResult<PropertyDetailedDto>> UpdateProperty(UpdatePropertyDto propertyDto, string lastUser, Guid propertyId);
        Task<IActionResult> UpdatePropertyArchiveIdAsync(Guid propertyId, Guid archiveId, string username);
        Task<IActionResult> UpdatePropertyNameAsync(Guid propertyId, string name, string username);
        Task<ActionResult<PropertyDetailedDto>> GetProperty(Guid propertyId);
        Task<ActionResult<IEnumerable<PropertyDto>>> GetProperties(bool includeDeleted = false);
        Task<IActionResult> DeleteProperty(Guid propertyId, string lastUser);
        Task<IActionResult> UndeleteProperty(Guid propertyId, string lastUser);
        Task<bool> PropertyExists(Guid propertyId);
    }
}
