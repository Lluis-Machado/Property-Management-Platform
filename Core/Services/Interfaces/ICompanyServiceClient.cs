using System.Text.Json;

namespace CoreAPI.Services
{
    public interface ICompanyServiceClient
    {
        Task<JsonDocument> CreateCompanyASync(string requestBody);
        Task<JsonDocument?> GetCompanyByIdAsync(Guid id);
        Task<JsonDocument?> UpdateCompanyArchiveAsync(string companyId, string archiveId);
        Task<JsonDocument?> UpdateCompanyAsync(Guid companyId, string requestBody);
        Task DeleteCompanyAsync(Guid companyId);
    }
}


