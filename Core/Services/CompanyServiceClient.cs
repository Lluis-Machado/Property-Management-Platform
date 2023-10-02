using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoreAPI.Services;

public class CompanyServiceClient : ICompanyServiceClient
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IBaseClientService _baseClient;

    public CompanyServiceClient(IHttpContextAccessor contextAccessor, IBaseClientService baseClient)
    {
        _contextAccessor = contextAccessor;
        _baseClient = baseClient;
    }

    public async Task<JsonDocument?> GetCompanyByIdAsync(Guid id)
    {
        return await _baseClient.ReadAsync($"companies/companies/{id}");
    }

    public async Task<JsonDocument?> UpdateCompanyArchive(string companyId, string archiveId)
    {

        return await _baseClient.UpdateAsync($"companies/companies/{companyId}/{archiveId}");
    }

    // Get company, check if updated object has different name
    // If name is different, update the archive as well
    public async Task<JsonDocument?> UpdateCompany(Guid companyId, string requestBody)
    {
        // Body validation
        if (string.IsNullOrEmpty(requestBody)) throw new BadHttpRequestException("Request body format not valid");

        // Get pre-update company information
        JsonDocument? currentCompany = await GetCompanyByIdAsync(companyId);
        if (currentCompany is null) throw new Exception($"Update failed - Company with id {companyId} not found");
        string currentName = currentCompany.RootElement.GetProperty("name").GetString() ?? "";
        
        // Get post-update company name
        JsonDocument body = JsonSerializer.Deserialize<JsonDocument>(requestBody);
        string requestName = body.RootElement.GetProperty("name").GetString() ?? "";

        // Perform company update
        var companyUpdate = await _baseClient.UpdateAsync($"companies/companies/{companyId}", requestBody);

        // If the name has changed, perform Archive name change
        if (currentName != requestName)
        {
            await _baseClient.UpdateAsync($"documents/archives/{currentCompany.RootElement.GetProperty("archiveId").GetString()}&newName={Uri.EscapeDataString(requestName)}");
        }

        return companyUpdate;
    }

    public async Task DeleteCompany(Guid companyId)
    {
        bool res = await _baseClient.DeleteAsync($"companies/companies/{companyId}");
        if (!res) throw new Exception($"Unknown error deleting company {companyId}");
    }

}