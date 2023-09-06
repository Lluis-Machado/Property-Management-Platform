using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using OwnershipAPI.DTOs;

namespace OwnershipAPI.Services;

public class CompanyServiceClient
{
    private readonly HttpClient _httpClient;

    public CompanyServiceClient()
    {
        _httpClient = new HttpClient();
#if DEVELOPMENT
        _httpClient.BaseAddress = new Uri("https://localhost:7142/"); // Replace with the base URL of the ownership service
#elif STAGE
        _httpClient.BaseAddress = new Uri("https://stage.plattesapis.net/company/"); // Replace with the base URL of the ownership service
#else
        _httpClient.BaseAddress = new Uri("https://plattesapis.net/company/"); // Replace with the base URL of the ownership service
#endif
        _httpClient.BaseAddress = new Uri("https://stage.plattesapis.net/companies/"); // Replace with the base URL of the ownership service

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<CompanyDto?> GetCompanyByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"companies/{id}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            return JsonSerializer.Deserialize<CompanyDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }


        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        throw new Exception($"Failed to get ownership by ID. Status code: {response.StatusCode}");
    }
}