using Authentication.Models;
using Authentication.Security;
using Authentication.Services.Auth0;
using Authentication.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RolesAPI _rolesAPI;

        public RolesController(RolesAPI rolesAPI)
        {
            _rolesAPI = rolesAPI;
        }

        #region BASIC_CRUD
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _rolesAPI.GetRolesListAsync();
                return Ok(roles);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
        }

        [HttpGet("{roleId}")]
        [Authorize]
        public async Task<IActionResult> GetRole(string roleId)
        {
            try
            {
                var role = await _rolesAPI.GetRoleAsync(roleId);
                return Ok(role);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateRole([FromBody] Auth0Role auth0Role)
        {
            try
            {
                var result = await _rolesAPI.CreateRoleAsync(auth0Role);
                return Ok(result);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
        }

        [HttpPatch("{roleId}")]
        [Authorize]
        public async Task<IActionResult> UpdateRole(string roleId, [FromBody] object roleUpdate)
        {
            try
            {
                var result = await _rolesAPI.UpdateRoleAsync(roleId, roleUpdate);
                return Ok(result);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
        }

        [HttpDelete("{roleId}")]
        [Authorize]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            try
            {
                await _rolesAPI.DeleteRoleAsync(roleId);
                return Ok();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
        }
        #endregion

        #region ROLE_PERMISSIONS
        [HttpGet("{roleId}/permissions")]
        [Authorize]
        public async Task<IActionResult> GetRolePermissions(string roleId)
        {
            try
            {
                var permissions = await _rolesAPI.GetRolePermissionsAsync(roleId);
                return Ok(permissions);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
        }

        [HttpPost("{roleId}/permissions")]
        [Authorize]
        public async Task<IActionResult> AssignPermissionsToRole(string roleId, [FromBody] List<Auth0Permission> permissions)
        {
            try
            {
                await _rolesAPI.AssignPermissionsToRoleAsync(roleId, permissions);
                return Ok();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
        }

        [HttpDelete("{roleId}/permissions")]
        [Authorize]
        public async Task<IActionResult> DeletePermissionsFromRole(string roleId, [FromBody] List<Auth0Permission> permissions)
        {
            try
            {
                await _rolesAPI.DeletePermissionsFromRoleAsync(roleId, permissions);
                return NoContent();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
        }
        #endregion
    }
}
