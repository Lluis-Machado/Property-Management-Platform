using Authentication.Models;

namespace AuthenticationAPI.Services.Auth0.Interfaces
{
    public interface IUsersAPI
    {
        #region BASIC_CRUD
        Task<List<object>> GetUserListAsync();
        Task<object> GetUserAsync(string userId);
        Task<object> CreateUserAsync(Auth0User auth0User);
        Task<object> UpdateUserAsync(string userId, object userUpdate);
        Task DeleteUserAsync(string userId);
        #endregion

        #region USER_ROLES
        Task<List<object>> GetUserRolesAsync(string userId);
        Task AssignUserRolesAsync(string userId, List<string> roles);
        Task DeleteUserRolesAsync(string userId, List<string> roles);
        #endregion

        #region USER_PERMISSIONS
        Task<List<object>> GetUserPermissionsAsync(string userId);
        Task AssignPermissionsToUserAsync(string userId, List<Auth0Permission> permissions);
        Task DeletePermissionsFromUserAsync(string userId, List<Auth0Permission> permissions);
        #endregion

        #region USER_LOGS
        Task<List<object>> GetUserLogsAsync(string userId);
        #endregion
        Task<string> ResetPasswordAsync(string email);
    }
}
