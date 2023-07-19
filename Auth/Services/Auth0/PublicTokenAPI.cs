using Authentication.Models;
using Authentication.Utils;
using AuthenticationAPI.Services.Auth0.Interfaces;
using System.Text.Json;

namespace Authentication.Services.Auth0
{
    public class PublicTokenAPI : IPublicTokenAPI
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
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<object>(responseContent) ?? new object();
            }

            throw new ApiException(response.StatusCode, responseContent);
        }
    }
}
