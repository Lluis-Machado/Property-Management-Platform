using Authentication.Models;
using Authentication.Security;
using Authentication.Services.Auth0;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _rolesAPI.GetRolesListAsync());
        }

        [HttpGet("{roleId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetRole(string roleId)
        {
            return Ok(await _rolesAPI.GetRoleAsync(roleId));
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateRole([FromBody] Auth0Role auth0Role)
        {
            return Ok(await _rolesAPI.CreateRoleAsync(auth0Role));
        }

        [HttpPatch("{roleId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateRole(string roleId, [FromBody] object roleUpdate)
        {
            return Ok(await _rolesAPI.UpdateRoleAsync(roleId, roleUpdate));
        }

        [HttpDelete("{roleId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            await _rolesAPI.DeleteRoleAsync(roleId);
            return Ok();
        }
        #endregion

        #region ROLE_PERMISSIONS
        [HttpGet("{roleId}/permissions")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetRolePermissions(string roleId)
        {
            return Ok(await _rolesAPI.GetRolePermissionsAsync(roleId));
        }

        [HttpPost("{roleId}/permissions")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AssignPermissionsToRole(string roleId, [FromBody] List<Auth0Permission> permissions)
        {
            await _rolesAPI.AssignPermissionsToRoleAsync(roleId, permissions);
            return Ok();
        }

        [HttpDelete("{roleId}/permissions")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeletePermissionsFromRole(string roleId, [FromBody] List<Auth0Permission> permissions)
        {
            await _rolesAPI.DeletePermissionsFromRoleAsync(roleId, permissions);
            return Ok();
        }
        #endregion
    }
}
