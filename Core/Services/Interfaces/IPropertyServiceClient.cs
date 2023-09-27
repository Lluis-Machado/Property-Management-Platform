using System;
using System.Threading.Tasks;

namespace CoreAPI.Services
{
    public interface IPropertyServiceClient
    {
        Task<string?> GetPropertyByIdAsync(Guid id);
        Task<string?> CreateProperty(string requestBody);
        Task<string?> UpdatePropertyArchive(string propertyId, string archiveId);
    }
}
