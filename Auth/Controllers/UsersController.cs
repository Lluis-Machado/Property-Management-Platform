using Authentication.Models;
using Authentication.Security;
using Authentication.Services.Auth0;
using AuthenticationAPI.Services.Auth0.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Authentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersAPI _usersAPI;

        public UsersController(IUsersAPI usersAPI)
        {
            _usersAPI = usersAPI;
        }

        #region BASIC_CRUD
        [HttpGet]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _usersAPI.GetUserListAsync());
        }

        [HttpGet("{userId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUser(string userId)
        {
            return Ok(await _usersAPI.GetUserAsync(userId));
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] Auth0User auth0User)
        {
            var user = await _usersAPI.CreateUserAsync(auth0User);
            return Created($"users/{user}", user);
        }

        [HttpPatch("{userId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] object updatedUser)
        {
            return Ok(await _usersAPI.UpdateUserAsync(userId, updatedUser));
        }

        [HttpDelete("{userId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _usersAPI.DeleteUserAsync(userId);
            return NoContent();
        }
        #endregion

        #region USER_ROLES
        [HttpGet("{userId}/roles")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            return Ok(await _usersAPI.GetUserRolesAsync(userId));
        }

        [HttpPost("{userId}/roles")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AssignUserRoles(string userId, [FromBody] List<string> roles)
        {
            await _usersAPI.AssignUserRolesAsync(userId, roles);
            return NoContent();
        }

        [HttpDelete("{userId}/roles")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteUserRoles(string userId, [FromBody] List<string> roles)
        {
            await _usersAPI.DeleteUserRolesAsync(userId, roles);
            return NoContent();
        }
        #endregion

        #region USER_PERMISSIONS
        [HttpGet("{userId}/permissions")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserPermissions(string userId)
        {
            return Ok(await _usersAPI.GetUserPermissionsAsync(userId));
        }

        [HttpPost("{userId}/permissions")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AssignPermissionsToUser(string userId, [FromBody] List<Auth0Permission> permissions)
        {
            await _usersAPI.AssignPermissionsToUserAsync(userId, permissions);
            return Ok();
        }

        [HttpDelete("{userId}/permissions")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeletePermissionsFromUser(string userId, [FromBody] List<Auth0Permission> permissions)
        {
            await _usersAPI.DeletePermissionsFromUserAsync(userId, permissions);
            return NoContent();
        }
        #endregion

        #region USER_LOGS
        [HttpGet("{userId}/logs")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserLogs(string userId)
        {
            return Ok(await _usersAPI.GetUserLogsAsync(userId));
        }
        #endregion

        [HttpGet("resetPassword")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ResetPassword([FromQuery] string email)
        {
            return Ok(await _usersAPI.ResetPasswordAsync(email));
        }
    }
}