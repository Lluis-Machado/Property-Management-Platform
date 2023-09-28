using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreAPI.Services
{
    public interface IPropertyServiceClient
    {
        Task<JsonDocument?> GetPropertyByIdAsync(Guid id);
        Task<JsonDocument?> CreateProperty(string requestBody);
        Task<JsonDocument?> UpdatePropertyArchive(string propertyId, string archiveId);
        Task<JsonDocument?> UpdateProperty(Guid propertyId, string requestBody);
    }
}
