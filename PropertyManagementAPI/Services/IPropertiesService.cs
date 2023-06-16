using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.DTOs;
using PropertyManagementAPI.Models;

namespace PropertyManagementAPI.Services
{
    public interface IPropertiesService
    {
        Task<ActionResult<PropertyDTO>> CreateProperty(CreatePropertyDTO propertyDTO, string lastUser);
        Task<ActionResult<PropertyDTO>> UpdateProperty(Guid propertyId, UpdatePropertyDTO propertyDTO, string lastUser);
        Task<ActionResult<PropertyDTO>> GetProperty(Guid propertyId);
        Task<ActionResult<IEnumerable<PropertyDTO>>> GetProperties();
        Task<IActionResult> DeleteProperty(Guid propertyId, string lastUser);
        Task<IActionResult> UndeleteProperty(Guid propertyId, string lastUser);
        Task<bool> PropertyExists(Guid propertyId);
    }
}
