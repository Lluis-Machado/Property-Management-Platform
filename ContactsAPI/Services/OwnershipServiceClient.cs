using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

public class OwnershipServiceClient
{
    private readonly HttpClient _httpClient;

    public OwnershipServiceClient()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://localhost:7000"); // Replace with the base URL of the ownership service
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<Ownership>?> GetOwnershipByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/ownership/{id}/contact");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (content == null)
                return null;
            return JsonSerializer.Deserialize<List<Ownership>>(content, new JsonSerializerOptions
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
