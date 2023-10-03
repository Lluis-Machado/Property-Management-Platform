using System.Text.Json;

namespace CoreAPI.Services
{
    public interface ICompanyServiceClient
    {
        Task<JsonDocument> CreateCompany(string requestBody);
        Task<JsonDocument?> GetCompanyByIdAsync(Guid id);
        Task<JsonDocument?> UpdateCompanyArchive(string companyId, string archiveId);
        Task<JsonDocument?> UpdateCompany(Guid companyId, string requestBody);
        Task DeleteCompany(Guid companyId);
    }
}


