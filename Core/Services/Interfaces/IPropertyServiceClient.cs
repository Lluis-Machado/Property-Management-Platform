using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreAPI.Services
{
    public interface IPropertyServiceClient
    {
        Task<JsonDocument?> GetPropertyByIdAsync(Guid id);
        Task<JsonDocument?> CreatePropertyAsync(string requestBody);
        Task<JsonDocument?> UpdatePropertyArchiveAsync(string propertyId, string archiveId);
        Task<JsonDocument?> UpdatePropertyAsync(Guid propertyId, string requestBody);
        Task DeletePropertyAsync(Guid propertyId);
    }
}
