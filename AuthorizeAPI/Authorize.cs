using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Runtime.Serialization;

namespace AuthorizeAPI
{
    [AttributeUsage(AttributeTargets.All)]
    public class Authorize : Attribute, IAuthorizationFilter
    {
        // IMPORTANT: These are the default permissions. If no permission are set, then only admins can access
        public readonly string[] DefaultPermissions = { "admin" };

        private readonly IList<string> _permissions;

        /// <summary>
        /// Constructor for Authorize
        /// </summary>
        /// <param name="permissions">Permissions for this instance of Authorize</param>
        public Authorize(params string[] permissions)
        {
            _permissions = permissions.Any() ? permissions : DefaultPermissions;
        }

        public void OnAuthorization(AuthorizationFilterContext actionContext)
        {
            // Check permissions in token
            try
            {
                var httpContext = actionContext.HttpContext;
                StringValues auth = httpContext.Request.Headers.Authorization;

                if (!auth.Any() || string.IsNullOrEmpty(auth[0])) throw new EmptyTokenException();

                string token = auth[0].Replace("bearer", "", StringComparison.OrdinalIgnoreCase).Trim();
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);

                // Extrae los permisos del usuario desde el token
                var permissions = jwtSecurityToken.Claims.Where(claim => claim.Type == "permissions").Select(e => e.Value).ToArray();

                // Mira si hay permisos en común entre los roles permitidos para la acción y los permisos del usuario
                var hasAnyPermission = _permissions.Intersect(permissions).Any();

                if (!hasAnyPermission) throw new UnauthorizedAccessException();
            }
            catch (EmptyTokenException e)
            {
                actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                actionContext.Result = new JsonResult("Expected token not provided") { Value = e.Message };
            }
            catch (UnauthorizedAccessException e)
            {
                actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                actionContext.Result = new JsonResult("Unauthorized") { Value = e.Message };
            }
            catch (Exception e)
            {
                actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                actionContext.Result = new JsonResult("Error occurred on token verification") { Value = e.Message };
            }
        }
    }

    [Serializable]
    internal class EmptyTokenException : Exception
    {
        public EmptyTokenException()
        {
        }

        public EmptyTokenException(string? message) : base(message)
        {
        }

        public EmptyTokenException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected EmptyTokenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}