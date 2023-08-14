using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using PropertiesAPI.Dtos;

namespace PropertiesAPI.Services;

public class OwnershipServiceClient
{
    private readonly HttpClient _httpClient;

    public OwnershipServiceClient()
    {
        _httpClient = new HttpClient();

#if DEVELOPMENT
        _httpClient.BaseAddress = new Uri("https://localhost:7007/"); // Replace with the base URL of the ownership service
#elif STAGE
        _httpClient.BaseAddress = new Uri("https://stage.plattesapis.net/ownership/"); // Replace with the base URL of the ownership service
#else
        _httpClient.BaseAddress = new Uri("https://plattesapis.net/ownership/"); // Replace with the base URL of the ownership service
#endif

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<OwnershipDto>?> GetOwnershipsByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"ownership/{id}/property");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (content == null)
                return null;
            return JsonSerializer.Deserialize<List<OwnershipDto>>(content, new JsonSerializerOptions
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

    public async Task<OwnershipDto?> CreateOwnershipAsync(OwnershipDto ownership)
    {
        var json = JsonSerializer.Serialize(ownership);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("ownership", content);

        if (response.IsSuccessStatusCode)
        {
            var createdContent = await response.Content.ReadAsStringAsync();
            var obj = JsonSerializer.Deserialize<OwnershipDto>(createdContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return obj;
        }


        throw new Exception($"Failed to create ownership. Status code: {response.StatusCode}\nDEBUG: Request uri: {_httpClient.BaseAddress}\nMessage received: {response.Content.ReadAsStringAsync().Result}");
    }
    public async Task<bool> DeleteOwnershipAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"ownership/{id}");
        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        return false;

        throw new Exception($"Failed to delete ownership. Status code: {response.StatusCode}");
    }
}