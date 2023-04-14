using Auth.Models;
using System.Text.Json;

namespace Auth.Services.Auth0
{
    public class UsersAPI
    {
        private readonly HttpClient _httpClient;
        private readonly Auth0Settings _auth0Settings;

        public UsersAPI(HttpClient httpClient, Auth0Settings auth0Settings)
        {
            _httpClient = httpClient;
            _auth0Settings = auth0Settings;
        }
        public async Task<List<object>> GetUserListAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_auth0Settings.BaseUrl}/api/v2/users");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) return JsonSerializer.Deserialize<List<object>>(responseContent);

            throw new Exception(responseContent);
        }

        public async Task<object> GetUserAsync(string userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_auth0Settings.BaseUrl}/api/v2/users/{userId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) return JsonSerializer.Deserialize<object>(responseContent);

            throw new Exception(responseContent);
        }

        public async Task<object> CreateUserAsync(Auth0User auth0User)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_auth0Settings.BaseUrl}/api/v2/users")
            {
                Content = JsonContent.Create(auth0User)
            };

            // Add the access token to the request headers
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) return JsonSerializer.Deserialize<object>(responseContent);

            throw new Exception(responseContent);
        }

        public async Task<object> UpdateUserAsync(string userId, object userUpdate)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{_auth0Settings.BaseUrl}/api/v2/users/{userId}")
            {
                Content = JsonContent.Create(userUpdate)
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) return JsonSerializer.Deserialize<object>(responseContent);

            throw new Exception(responseContent);
        }

        public async Task DeleteUserAsync(string userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_auth0Settings.BaseUrl}/api/v2/users/{userId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth0Settings.ManagementApiToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new Exception(responseContent);
            }
        }
    }
}