using Auth.Services.Auth0;
using Auth.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly PublicTokenAPI _publicTokenApi;

        public AuthController(PublicTokenAPI auth0Api, UsersAPI usersAPI)
        {
            _publicTokenApi = auth0Api;
        }

        [HttpGet]
        public async Task<IActionResult> GetToken([FromQuery] string username, [FromQuery] string password)
        {
            try
            {
                var token = await _publicTokenApi.GetTokenAsync(username, password);
                return Ok(token);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}