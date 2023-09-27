using System.Text.Json;

namespace CoreAPI.Services
{
    public interface ICoreService
    {
        Task<string> CreateProperty(string requestBody);
        Task<string> CreateCompany(string requestBody);
        Task<string> CreateContact(string requestBody);
        Task<JsonDocument> CreateArchive(string requestBody, string? type, string? id);

        Task<string> UpdateProperty(string requestBody);
        Task<string> UpdateCompany(string requestBody);
        Task<string> UpdateContact(string requestBody);


        Task<JsonDocument?> GetContact(Guid Id);
        Task<JsonDocument?> GetProperty(Guid Id);
        Task<JsonDocument?> GetCompany(Guid Id);
        Task<JsonDocument[]?> GetContacts(bool includeDeleted = false);
        Task<JsonDocument[]?> GetProperties(bool includeDeleted = false);
        Task<JsonDocument[]?> GetCompanies(bool includeDeleted = false);

        // TODO: Delete methods

    }
}