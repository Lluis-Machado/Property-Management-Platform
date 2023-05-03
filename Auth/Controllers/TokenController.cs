using AuthenticationAPI.Services.Auth0.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Authentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IPublicTokenAPI _publicTokenApi;

        public TokenController(IPublicTokenAPI auth0Api)
        {
            _publicTokenApi = auth0Api;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetToken([FromQuery] string username, [FromQuery] string password)
        {
            return Ok(await _publicTokenApi.GetTokenAsync(username, password));
        }
    }
}