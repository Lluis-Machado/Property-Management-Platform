using Authentication.Models;
using Authentication.Utils;
using AuthenticationAPI.Services.Auth0.Interfaces;
using System.Text.Json;

namespace Authentication.Services.Auth0
{
    public class ManagementTokenManager : IManagementTokenManager
    {
        private static string? _token;
        private static DateTime _expiration;
        private readonly HttpClient _httpClient;
        private readonly Auth0Settings _auth0Settings;

        public ManagementTokenManager(HttpClient httpClient, Auth0Settings auth0Settings)
        {
            _httpClient = httpClient;
            _auth0Settings = auth0Settings;
        }

        public async Task<string> GetTokenAsync()
        {
            if (string.IsNullOrEmpty(_token) || DateTime.UtcNow >= _expiration)
            {
                await RefreshTokenAsync();
            }

            return _token!;
        }

        private async Task RefreshTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_auth0Settings.BaseUrl}/oauth/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                    {"client_id", _auth0Settings.ManagementClientId},
                    {"client_secret", _auth0Settings.ManagementClientSecret},
                    {"audience", $"{_auth0Settings.BaseUrl}/api/v2/"},
                    {"grant_type", "client_credentials"}
                })
            };

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var responseObject = JsonSerializer.Deserialize<JsonElement>(responseContent);
                if (responseObject.TryGetProperty("expires_in", out var expiresIn) && expiresIn.TryGetInt32(out var expiresInSeconds))
                {
                    if (responseObject.TryGetProperty("access_token", out var token))
                    {
                        _token = token.ToString();
                        _expiration = DateTime.UtcNow.AddSeconds(expiresInSeconds);
                        return;
                    }
                    else throw new Exception("No value for access_token found on Auth0 response");
                }
                else throw new Exception("No value for expires_in found on Auth0 response");
            }

            throw new ApiException(response.StatusCode, responseContent);
        }
    }
}