namespace Authentication.Models
{
    public class Auth0Settings
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ManagementClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ManagementClientSecret { get; set; }
        public string Audience { get; set; }

        public Auth0Settings()
        {
            BaseUrl = string.Empty;
            ClientId = string.Empty;
            ManagementClientId = string.Empty;
            ClientSecret = string.Empty;
            ManagementClientSecret = string.Empty;
            Audience = string.Empty;
        }
    }
}