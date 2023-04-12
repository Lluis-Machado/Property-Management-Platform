using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace Auth.Utils
{
    [AttributeUsage(AttributeTargets.All)]
    public class SecurityControl : Attribute, IAuthorizationFilter
    {
        // IMPORTANT: These are the default permissions. If no permission are setted, then only admin can access
        public string[] DefaultPermissions = { "admin" };

        private readonly IList<string> _permissions;

        /// <summary>
        /// Constructor for SecurityControl
        /// </summary>
        /// <param name="permissions">Permissions for this instance of SecurityControl</param>
        public SecurityControl(params string[] permissions)
        {
            _permissions = permissions ?? DefaultPermissions;
        }

        public void OnAuthorization(AuthorizationFilterContext actionContext)
        {
            // Check permissions in token
            try
            {
                var httpContext = actionContext.HttpContext;
                StringValues auth = httpContext.Request.Headers.Authorization;

                string token = auth[0].Replace("bearer", "", StringComparison.OrdinalIgnoreCase).Trim();
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);

                // Extrae los permisos del usuario desde el token
                var permissions = jwtSecurityToken.Claims.Where(claim => claim.Type == "permissions").Select(e => e.Value).ToArray();

                // Mira si hay permisos en común entre los roles permitidos para la acción y los permisos del usuario
                var hasAnyPermission = _permissions.Intersect(permissions).Any();

                if (!hasAnyPermission) throw new UnauthorizedAccessException();
            }
            catch (Exception e)
            {
                actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                actionContext.Result = new JsonResult("Error occurred on token verification") { Value = e.Message };
            }
        }
    }
}
