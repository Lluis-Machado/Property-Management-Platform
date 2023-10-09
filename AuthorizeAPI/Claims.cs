using System.IdentityModel.Tokens.Jwt;

namespace AuthorizeAPI
{
    public static class Claims
    {
        public static string GetUserEmail(string t)
        {
            string token = t.Replace("bearer", "", StringComparison.OrdinalIgnoreCase).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            // Extrae los permisos del usuario desde el token
            var email = jwtSecurityToken.Claims.Where(claim => claim.Type == "user_emails").Select(e => e.Value).FirstOrDefault();

            return email ?? "Unknown User";
        }
    }
}