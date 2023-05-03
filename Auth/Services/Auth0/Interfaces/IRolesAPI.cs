using Authentication.Models;

namespace AuthenticationAPI.Services.Auth0.Interfaces
{
    public interface IRolesAPI
    {
        #region BASIC_CRUD
        Task<List<object>> GetRolesListAsync();
        Task<object> GetRoleAsync(string roleId);
        Task<object> CreateRoleAsync(Auth0Role auth0Role);
        Task<object> UpdateRoleAsync(string roleId, object roleUpdate);
        Task DeleteRoleAsync(string roleId);
        #endregion

        #region ROLE_PERMISSIONS
        Task<List<object>> GetRolePermissionsAsync(string roleId);
        Task AssignPermissionsToRoleAsync(string roleId, List<Auth0Permission> permissions);
        Task DeletePermissionsFromRoleAsync(string roleId, List<Auth0Permission> permissions);
        #endregion
    }
}
