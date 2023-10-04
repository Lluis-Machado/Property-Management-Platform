﻿namespace AuthenticationAPI.Models
{
    public class TokenResponse
    {
        public string? access_token { get; set; }
        public int? expires_in { get; set; }
        public string? token_type { get; set; }
        public string? source_ip { get; set; }
    }
}
