namespace Auth.Models
{
    public class Auth0Settings
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Audience { get; set; }
        public string? ManagementApiToken { get; set; }
    }
}
