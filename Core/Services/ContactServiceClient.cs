using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CoreAPI.Services;

public class ContactServiceClient
{
    private readonly HttpClient _httpClient;

    public ContactServiceClient()
    {
        _httpClient = new HttpClient();
#if DEVELOPMENT
        _httpClient.BaseAddress = new Uri("https://localhost:7142/"); // Replace with the base URL of the ownership service
#elif PRODUCTION
        _httpClient.BaseAddress = new Uri("https://plattesapis.net/contacts/"); // Replace with the base URL of the ownership service
#else
        _httpClient.BaseAddress = new Uri("https://stage.plattesapis.net/contacts/"); // Replace with the base URL of the ownership service
#endif

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string?> GetCompanyByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"companies/{id}");
        var _authorizationToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IjZ1TTcwYmU1OElvMjNNRUZELWh1SSJ9.eyJpc3MiOiJodHRwczovL3N0YWdlLXBsYXR0ZXMuZXUuYXV0aDAuY29tLyIsInN1YiI6ImF1dGgwfDY0YzBiZmIzMGI3ZWI3YzU3OTcyNjlkYyIsImF1ZCI6Ind1Zi1hdXRoMC1hcGkiLCJpYXQiOjE2OTM1NjY1MjAsImV4cCI6MTY5NDg2MjUyMCwiYXpwIjoiNkVaRGNDbGk1TDA4Z1d2czB5NmtLY2NQcW5GTHNVQzIiLCJndHkiOiJwYXNzd29yZCIsInBlcm1pc3Npb25zIjpbImFkbWluIiwicmVhZCIsInJlYWQ6ZG9jdW1lbnRzIiwid3JpdGUiLCJ3cml0ZTpkb2N1bWVudHMiXX0.j7ZyOPAbbSry2ivVNbDJqdNhLJg0T6m-uVsGmMEjnbB9D4V98HJh-wYmgPpuKL-H9olI5aCfRyRRoA4t-n3LNwtX4Vln9geyU_WTLWdwYzjI2NniCKdAdb87xsuCdfcTLSmTz8L5rGwECXZzd1SlaV7gRl48YAClBC3C7ZloQX2WBkx1ykmhYNHD-BD3w4uplDQbkyZ0ohnjSYD7XAgR2Ne4b8NrwTNIvb2gOG3gU-_84UYFIm5xPkCRYVHjuaMpYdh1xtJAXJ03jy9uyf9rrOvZ6V2yuO8J9TW2E8GDuWrnz7kBQo2U66vYVrPW_svkFgYH8Kuem3HTXgvyoxYrfQ";
        // Add authorization token to the request headers
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authorizationToken);

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
}