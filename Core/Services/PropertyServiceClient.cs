using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace CoreAPI.Services;

public class PropertyServiceClient : IPropertyServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<PropertyServiceClient> _logger;

    public PropertyServiceClient(IHttpContextAccessor contextAccessor, ILogger<PropertyServiceClient> logger)
    {
        _logger = logger;
        _contextAccessor = contextAccessor;
        _httpClient = new HttpClient();
#if DEVELOPMENT
        _httpClient.BaseAddress = new Uri("https://localhost:7012/"); // Replace with the base URL of the ownership service
#elif PRODUCTION
        _httpClient.BaseAddress = new Uri("https://plattesapis.net/properties/"); // Replace with the base URL of the ownership service
#else
        _httpClient.BaseAddress = new Uri("https://stage.plattesapis.net/properties/");
#endif

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string?> GetPropertyByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"properties/{id}");
        // Add authorization token to the request headers
        var _auth = _contextAccessor?.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (_auth != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.Split(' ')[1]);
        }

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            return content; /*JsonSerializer.Deserialize<string>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });*/
        }


        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        throw new Exception($"Failed to get ownership by ID. Status code: {response.StatusCode}");
    }

    public async Task<string?> CreateProperty(string requestBody)
    {
        _logger.LogInformation($"PropertyServiceClient - CreateProperty - Begin request - BaseAddress: {_httpClient.BaseAddress}\nBody: {Uri.UnescapeDataString(requestBody)}");


        using var request = new HttpRequestMessage(HttpMethod.Post, "properties");

        // Add authorization token to the request headers
        var _auth = _contextAccessor?.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (_auth != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.Split(' ')[1]);
        }

        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        _logger.LogDebug($"PropertyServiceClient - Result from CreateProperty API call: {content}");

        if (response.IsSuccessStatusCode)
        {
            return string.IsNullOrEmpty(content) ? null : content;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        throw new Exception($"Failed to create property. Status code: {response.StatusCode}. {content}");
    }

    public async Task<string?> UpdatePropertyArchive(string propertyId, string archiveId)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, $"properties/{propertyId}/{archiveId}");

        _logger.LogInformation($"PropertyServiceClient - UpdatePropertyArchive - Begin request - PropId: {propertyId} | ArchId: {archiveId} | BaseAddress: {_httpClient.BaseAddress} | Uri: {request.RequestUri}");


        // Add authorization token to the request headers
        var _auth = _contextAccessor?.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (_auth != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.Split(' ')[1]);
        }

        var response = await _httpClient.SendAsync(request);

        _logger.LogInformation($"PropertyServiceClient - UpdatePropertyArchive - Response: {response.StatusCode} - Content: {response.Content.ReadAsStringAsync().Result}");


        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            return content;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new Exception($"Failed to update property archive by ID. Status code: {response.StatusCode}");

            return null;
        }

        throw new Exception($"Failed to update property archive by ID. Status code: {response.StatusCode}");
    }

}