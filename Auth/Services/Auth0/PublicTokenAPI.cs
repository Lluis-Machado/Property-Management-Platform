using Authentication.Models;
using Authentication.Utils;
using AuthenticationAPI.Models;
using AuthenticationAPI.Services.Auth0.Interfaces;
using System.Dynamic;
using System.Text.Json;

namespace Authentication.Services.Auth0
{
    public class PublicTokenAPI : IPublicTokenAPI
    {
        private readonly HttpClient _httpClient;
        private readonly Auth0Settings _auth0Settings;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<PublicTokenAPI> _logger;

        public PublicTokenAPI(HttpClient httpClient, Auth0Settings auth0Settings, IHttpContextAccessor contextAccessor, ILogger<PublicTokenAPI> logger)
        {
            _httpClient = httpClient;
            _auth0Settings = auth0Settings;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public async Task<object> GetTokenAsync(string username, string password)
        {

            _logger.LogInformation($"New token request for username {username} | Original request source IP: {_contextAccessor.HttpContext?.Request.Headers.Referer}");

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
                var token = JsonSerializer.Deserialize<TokenResponse>(responseContent) ?? new TokenResponse();

                token.source_ip = _contextAccessor.HttpContext?.Request.Headers.Referer;

                return token;
            }

            throw new ApiException(response.StatusCode, responseContent);
        }
    }
}