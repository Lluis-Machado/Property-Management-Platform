namespace AuthenticationAPI.Services.Auth0.Interfaces
{
    public interface IPublicTokenAPI
    {
        Task<object> GetTokenAsync(string username, string password);
    }
}
