using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> GetToken([FromQuery] string username, [FromQuery] string password)
        {
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://dev-jovotcv8xe1ywn6j.eu.auth0.com/oauth/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                    {"client_id", "zmvYmcngSkFn9Y3acNNkHPctwJpxtNqx"},
                    {"client_secret", "Ks0wmn5kBmN-HTbxRdrBpMrBvh0nM_BjQWe2-_VlCofbVs4l28pzLiTxE9AeSt87"},
                    {"audience", "wuf-app"},
                    {"grant_type", "password"},
                    {"username", username},
                    {"password", password}
                })
            };

            var httpClient = new HttpClient();
            var tokenResponse = await httpClient.SendAsync(tokenRequest);

            if (tokenResponse.IsSuccessStatusCode)
                return await tokenResponse.Content.ReadAsStringAsync();
            else
                throw new UnauthorizedAccessException(); // TODO: Cambiar este error tan genérico
        }
    }
}