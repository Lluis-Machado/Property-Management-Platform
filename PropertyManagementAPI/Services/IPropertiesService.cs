using Microsoft.AspNetCore.Mvc;
using PropertiesAPI.DTOs;
using PropertiesAPI.Models;

namespace PropertiesAPI.Services
{
    public interface IPropertiesService
    {
        Task<ActionResult<PropertyDetailedDto>> CreateProperty(CreatePropertyDto propertyDto, string lastUser);
        Task<ActionResult<PropertyDetailedDto>> UpdateProperty(UpdatePropertyDTO propertyDto, string lastUser, Guid propertyId);
        Task<ActionResult<PropertyDetailedDto>> GetProperty(Guid propertyId);
        Task<ActionResult<IEnumerable<PropertyDto>>> GetProperties(bool includeDeleted = false);
        Task<IActionResult> DeleteProperty(Guid propertyId, string lastUser);
        Task<IActionResult> UndeleteProperty(Guid propertyId, string lastUser);
        Task<bool> PropertyExists(Guid propertyId);
    }
}
