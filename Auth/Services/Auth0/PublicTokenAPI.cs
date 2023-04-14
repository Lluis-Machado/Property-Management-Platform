using Auth.Models;
using System.Text.Json;

namespace Auth.Services.Auth0
{
    public class PublicTokenAPI
    {
        private readonly HttpClient _httpClient;
        private readonly Auth0Settings _auth0Settings;

        public PublicTokenAPI(HttpClient httpClient, Auth0Settings auth0Settings)
        {
            _httpClient = httpClient;
            _auth0Settings = auth0Settings;
        }

        public async Task<object> GetTokenAsync(string username, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_auth0Settings.BaseUrl}/oauth/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                    {"client_id", _auth0Settings.ClientId},
                    {"client_secret", _auth0Settings.ClientSecret},
                    {"audience", _auth0Settings.Audience},
                    {"grant_type", "password"},
                    {"username", username},
                    {"password", password}
                })
            };

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            // Deserialize the response content into a JSON object
            return JsonSerializer.Deserialize<object>(responseContent);
        }
    }
}
