using System.Net;
using System.Net.Http.Headers;

namespace CoreAPI.Services;

public class CompanyServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _contextAccessor;

    public CompanyServiceClient(IHttpContextAccessor contextAccessor)
    {
        _httpClient = new HttpClient();
#if DEVELOPMENT
        _httpClient.BaseAddress = new Uri("https://localhost:7142/"); // Replace with the base URL of the ownership service
#elif PRODUCTION
        _httpClient.BaseAddress = new Uri("https://plattesapis.net/companies/"); // Replace with the base URL of the ownership service
#else
        _httpClient.BaseAddress = new Uri("https://stage.plattesapis.net/companies/"); // Replace with the base URL of the ownership service
#endif

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _contextAccessor = contextAccessor;
    }

    public async Task<string?> GetCompanyByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"companies/{id}");

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

    public async Task<string?> UpdateCompanyArchive(string companyId, string archiveId)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, $"{companyId}/{archiveId}");

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
            return content;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        throw new Exception($"Failed to update company archive by IDD. Status code: {response.StatusCode}");
    }
}