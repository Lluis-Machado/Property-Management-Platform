using System.Text.Json;

namespace CoreAPI.Services
{
    public interface ICoreService
    {
        Task<JsonDocument> CreateProperty(string requestBody);
        Task<JsonDocument> CreateCompany(string requestBody);
        Task<JsonDocument> CreateContact(string requestBody);
        Task<JsonDocument> CreateArchive(string requestBody, string? type, string? id);

        Task<JsonDocument?> UpdateProperty(Guid propertyId, string requestBody);
        Task<JsonDocument?> UpdateCompany(Guid companyId, string requestBody);
        Task<JsonDocument?> UpdateContact(Guid contactId, string requestBody);

        Task DeleteProperty(Guid propertyId);
        Task DeleteCompany(Guid companyId);
        Task DeleteContact(Guid contactId);


    }
}