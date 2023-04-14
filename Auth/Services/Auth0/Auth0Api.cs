namespace Auth.Services.Auth0
{
    public class Auth0Api
    {
        private readonly HttpClient _httpClient;
        private readonly Auth0Settings _auth0Settings;

        public Auth0Api(HttpClient httpClient, Auth0Settings auth0Settings)
        {
            _httpClient = httpClient;
            _auth0Settings = auth0Settings;
        }

        public async Task<string> GetTokenAsync(string username, string password)
        {
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"{_auth0Settings.BaseUrl}/oauth/token")
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

            var tokenResponse = await _httpClient.SendAsync(tokenRequest);

            if (tokenResponse.IsSuccessStatusCode)
                return await tokenResponse.Content.ReadAsStringAsync();
            else
                throw new UnauthorizedAccessException(); // TODO: Cambiar este error tan genérico
        }
    }
}
