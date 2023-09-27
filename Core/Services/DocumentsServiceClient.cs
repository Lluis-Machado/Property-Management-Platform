using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoreAPI.Services;

public class DocumentsServiceClient : IDocumentsServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<DocumentsServiceClient> _logger;

    public DocumentsServiceClient(IHttpContextAccessor contextAccessor, ILogger<DocumentsServiceClient> logger)
    {
        _httpClient = new HttpClient();
#if DEVELOPMENT
        _httpClient.BaseAddress = new Uri("http://localhost:7079/"); // Replace with the base URL of the ownership service
#elif PRODUCTION
        _httpClient.BaseAddress = new Uri("https://plattesapis.net/documents/"); // Replace with the base URL of the ownership service
#else
        _httpClient.BaseAddress = new Uri("https://stage.plattesapis.net/documents/"); // Replace with the base URL of the ownership service
#endif

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _logger = logger;
        _contextAccessor = contextAccessor;
    }


    public async Task<JsonDocument?> CreateArchive(string requestBody, string? type, string? id)
    {
        _logger.LogInformation($"DocumentServiceClient - CreateArchive - Begin request - BaseAddress: {_httpClient.BaseAddress}");

        string url = string.IsNullOrEmpty(type) ? string.Empty : $"/{type}/{id}";

        using var request = new HttpRequestMessage(HttpMethod.Post, "archives" + url);

        _logger.LogInformation($"DocumentServiceClient - CreateArchive - Request URL: archives" + url);

        // Add authorization token to the request headers
        var _auth = _contextAccessor?.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (_auth != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.Split(' ')[1]);
        }
        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

        _logger.LogInformation($"DocumentServiceClient - CreateArchive - Response: Code {(int)response.StatusCode} - {response.Content.ReadAsStringAsync().Result}");


        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return string.IsNullOrEmpty(content) ? null : JsonSerializer.Deserialize<JsonDocument>(content);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        throw new Exception($"Failed to create property. Status code: {response.StatusCode}");
    }

    public async Task<string?> CreateFolder(string requestBody, string archiveId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{archiveId}/folders");
        var _authorizationToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IjZ1TTcwYmU1OElvMjNNRUZELWh1SSJ9.eyJpc3MiOiJodHRwczovL3N0YWdlLXBsYXR0ZXMuZXUuYXV0aDAuY29tLyIsInN1YiI6ImF1dGgwfDY0YzBiZmIzMGI3ZWI3YzU3OTcyNjlkYyIsImF1ZCI6Ind1Zi1hdXRoMC1hcGkiLCJpYXQiOjE2OTM4OTc5NDQsImV4cCI6MTY5NTE5Mzk0NCwiYXpwIjoiNkVaRGNDbGk1TDA4Z1d2czB5NmtLY2NQcW5GTHNVQzIiLCJndHkiOiJwYXNzd29yZCIsInBlcm1pc3Npb25zIjpbImFkbWluIiwicmVhZCIsInJlYWQ6ZG9jdW1lbnRzIiwid3JpdGUiLCJ3cml0ZTpkb2N1bWVudHMiXX0.Dhx_--KxU6mjNeDVH1lXv2qezBpA70387rMMObouVkdoFphc2igAbFuSOADmKkeLasuccnbIyA9SClaCjlXCEkYDtTQ1byWsvben_r20S9qbDcZjqzLRLWXVMwR0DZh_o2A1RUiujBK_EsHs8noyMJiFb6uofpQNVu6GMNHAsxn9P6wB1T_rLiAbftfC8_tmRDV__EqsxmDd417tthQzaQ8ewWK6K1Gtnp81sB9LT4eLnUCHxH5f-Z9fDw5sv1gv6QYUoVv2iT61Fj2T8xZCt2VMvxCHyOStZeAOh2Xw-t-hdjXpgdjktv_66UB7EN6IUKXoOJrrlfaBnSGmaHnfAw";

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authorizationToken);
        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return string.IsNullOrEmpty(content) ? null : content;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        throw new Exception($"Failed to create property. Status code: {response.StatusCode}");
    }

}