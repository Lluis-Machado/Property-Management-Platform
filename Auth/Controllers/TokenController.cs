using AuthenticationAPI.Services.Auth0.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security;

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

        [HttpGet]
        [Route("GetBackofficeToken")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBackofficeToken([FromQuery] string username, [FromQuery] string password)
        {
            string[] allowedRoles = { "admin", "backoffice" };

            var tokenInfo = (System.Text.Json.JsonElement) await _publicTokenApi.GetTokenAsync(username, password);
            
            string? token = tokenInfo.GetProperty("access_token").ToString().Replace("bearer", "", StringComparison.OrdinalIgnoreCase)?.Trim();
            if (string.IsNullOrEmpty(token)) return BadRequest("Token is empty, check credentials");
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            // Extrae los permisos del usuario desde el token
            var permissions = jwtSecurityToken.Claims.Where(claim => claim.Type == "permissions").Select(e => e.Value.ToLowerInvariant()).ToArray();

            // Mira si hay permisos en común entre los roles permitidos para la acción y los permisos del usuario
            var hasAnyPermission = allowedRoles.Intersect(permissions).Any();

            if (!hasAnyPermission) return Unauthorized();
            else return Ok(tokenInfo);

        }
    }
}