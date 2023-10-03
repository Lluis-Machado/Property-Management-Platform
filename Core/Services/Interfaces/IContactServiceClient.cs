using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreAPI.Services
{
    public interface IContactServiceClient
    {

        Task<JsonDocument> CreateContactAsync(string requestBody);
        Task<JsonDocument?> GetContactByIdAsync(Guid id);
        Task<JsonDocument?> UpdateContactArchiveAsync(string contactId, string archiveId);
        Task<JsonDocument?> UpdateContactAsync(Guid contactId, string requestBody);
        Task DeleteContactAsync(Guid contactId);
    }
}
