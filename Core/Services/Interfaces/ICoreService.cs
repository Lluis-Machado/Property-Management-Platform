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


        Task<JsonDocument?> GetContact(Guid Id);
        Task<JsonDocument?> GetProperty(Guid Id);
        Task<JsonDocument?> GetCompany(Guid Id);
        Task<JsonDocument[]?> GetContacts(bool includeDeleted = false);
        Task<JsonDocument[]?> GetProperties(bool includeDeleted = false);
        Task<JsonDocument[]?> GetCompanies(bool includeDeleted = false);

        // TODO: Delete methods

    }
}