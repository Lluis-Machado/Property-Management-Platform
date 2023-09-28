using System.Net.Http.Headers;
using System.Text.Json;

namespace CoreAPI.Services;

public class PropertyServiceClient : IPropertyServiceClient
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<PropertyServiceClient> _logger;
    private readonly IBaseClientService _baseClient;

    public PropertyServiceClient(IHttpContextAccessor contextAccessor, ILogger<PropertyServiceClient> logger, IBaseClientService baseClient)
    {
        _logger = logger;
        _contextAccessor = contextAccessor;
        _baseClient = baseClient;
    }

    public async Task<JsonDocument?> GetPropertyByIdAsync(Guid id)
    {
        return await _baseClient.ReadAsync($"properties/properties/{id}");
    }


    public async Task<JsonDocument?> CreateProperty(string requestBody)
    {
        return await _baseClient.CreateAsync($"properties/properties", requestBody);
    }

    public async Task<JsonDocument?> UpdatePropertyArchive(string propertyId, string archiveId)
    {
        return await _baseClient.UpdateAsync($"properties/properties/{propertyId}/{archiveId}");
    }

    // Get property, check if updated object has different name
    // If name is different, update the archive as well
    public async Task<JsonDocument?> UpdateProperty(Guid propertyId, string requestBody)
    {
        // Body validation
        if (string.IsNullOrEmpty(requestBody)) throw new BadHttpRequestException("Request body format not valid");

        // Get pre-update property information
        JsonDocument? currentProperty = await GetPropertyByIdAsync(propertyId);
        if (currentProperty is null) throw new Exception($"Update failed - Property with id {propertyId} not found");
        string currentName = currentProperty.RootElement.GetProperty("name").GetString() ?? "";

        // Get post-update property name
        JsonDocument body = JsonSerializer.Deserialize<JsonDocument>(requestBody);
        string requestName = body.RootElement.GetProperty("name").GetString() ?? "";

        // Perform property update
        var propertyUpdate = await _baseClient.UpdateAsync($"companies/companies/{propertyId}", requestBody);

        // If the name has changed, perform Archive name change
        if (currentName != requestName)
        {
            await _baseClient.UpdateAsync($"documents/archives/{currentProperty.RootElement.GetProperty("archiveId").GetString()}&newName={Uri.EscapeDataString(requestName)}");
        }

        return propertyUpdate;
    }

}