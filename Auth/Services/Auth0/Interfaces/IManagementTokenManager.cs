namespace AuthenticationAPI.Services.Auth0.Interfaces
{
    public interface IManagementTokenManager
    {
        Task<string> GetTokenAsync();
    }
}