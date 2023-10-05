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


    public async Task<JsonDocument?> CreatePropertyAsync(string requestBody)
    {
        return await _baseClient.CreateAsync($"properties/properties", requestBody);
    }

    public async Task<JsonDocument?> UpdatePropertyArchiveAsync(string propertyId, string archiveId)
    {
        return await _baseClient.UpdateAsync($"properties/properties/{propertyId}/{archiveId}");
    }

    // Get property, check if updated object has different name
    // If name is different, update the archive as well
    public async Task<JsonDocument?> UpdatePropertyAsync(Guid propertyId, string requestBody)
    {
        // Body validation
        if (string.IsNullOrEmpty(requestBody)) throw new BadHttpRequestException("Request body format not valid");

        // Get pre-update property information
        JsonDocument? currentProperty = await GetPropertyByIdAsync(propertyId);
        if (currentProperty is null) throw new Exception($"Update failed - Property with id {propertyId} not found");
        string currentName = currentProperty.RootElement.GetProperty("name").GetString() ?? "";

        // Get post-update property name
        JsonDocument body = JsonSerializer.Deserialize<JsonDocument>(requestBody);
        string requestName = CoreService.GetPropertyFromJson(body, "name") ?? "";

        // Perform property update
        var propertyUpdate = await _baseClient.UpdateAsync<string>($"properties/properties/{propertyId}", requestBody);

        // If the name has changed, perform Archive name change
        if (currentName != requestName)
        {
            await _baseClient.UpdateAsync($"documents/archives/{CoreService.GetPropertyFromJson(currentProperty, "archiveId")}?newName={Uri.EscapeDataString(requestName)}");
        }

        return propertyUpdate;
    }

    public async Task DeletePropertyAsync(Guid propertyId)
    {
        bool res = await _baseClient.DeleteAsync($"properties/properties/{propertyId}");
        if (!res) throw new Exception($"Unknown error deleting property {propertyId}");
    }


}