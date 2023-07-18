using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using OwnershipAPI.Dtos;

namespace OwnershipAPI.Services;

public class PropertyServiceClient
{
    private readonly HttpClient _httpClient;

    public PropertyServiceClient()
    {
        _httpClient = new HttpClient();
#if DEVELOPMENT
        _httpClient.BaseAddress = new Uri("https://localhost:7228/"); // Replace with the base URL of the ownership service
#elif STAGE
        _httpClient.BaseAddress = new Uri("https://stage.plattesapis.net/properties/"); // Replace with the base URL of the ownership service
#else
        _httpClient.BaseAddress = new Uri("https://plattesapis.net/properties/"); // Replace with the base URL of the ownership service
#endif

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<PropertyDto?> GetPropertyByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"property/{id}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            return JsonSerializer.Deserialize<PropertyDto>(content, new JsonSerializerOptions
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