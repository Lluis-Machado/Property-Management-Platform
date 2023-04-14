namespace Auth.Models
{
    public class Auth0User
    {
        // To know more visit: https://auth0.com/docs/api/management/v2#!/Users/post_users
        public string email { get; set; }
        public bool blocked { get; set; }
        public bool email_verified { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string name { get; set; }
        public string nickname { get; set; }
        public string picture { get; set; }
        public string user_id { get; set; }
        public string connection { get; set; } = "Username-Password-Authentication";
        public string password { get; set; }
        public bool verify_email { get; set; }
    }
}
