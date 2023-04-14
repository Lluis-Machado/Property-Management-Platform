using Auth.Services.Auth0;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly Auth0Api _auth0Api;

        public AuthController(Auth0Api auth0Api)
        {
            _auth0Api = auth0Api;
        }

        [HttpGet]
        public async Task<IActionResult> GetToken([FromQuery] string username, [FromQuery] string password)
        {
            try
            {
                var token = await _auth0Api.GetTokenAsync(username, password);
                return Ok(token);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}