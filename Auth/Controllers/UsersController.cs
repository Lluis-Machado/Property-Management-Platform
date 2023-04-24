using Authentication.Models;
using Authentication.Security;
using Authentication.Services.Auth0;
using Authentication.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace Authentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UsersAPI _usersAPI;

        public UsersController(UsersAPI usersAPI)
        {
            _usersAPI = usersAPI;
        }

        #region BASIC_CRUD
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _usersAPI.GetUserListAsync();
                return Ok(users);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUser(string userId)
        {
            try
            {
                var user = await _usersAPI.GetUserAsync(userId);
                return Ok(user);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateUser([FromBody] Auth0User auth0User)
        {
            try
            {
                var user = await _usersAPI.CreateUserAsync(auth0User);
                return Created($"users/{user}", user);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] object updatedUser)
        {
            try
            {
                var result = await _usersAPI.UpdateUserAsync(userId, updatedUser);
                return Ok(result);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{userId}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                await _usersAPI.DeleteUserAsync(userId);
                return NoContent();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region USER_ROLES
        [HttpGet("{userId}/roles")]
        [Authorize]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            try
            {
                var roles = await _usersAPI.GetUserRolesAsync(userId);
                return Ok(roles);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{userId}/roles")]
        [Authorize]
        public async Task<IActionResult> AssignUserRoles(string userId, [FromBody] List<string> roles)
        {
            try
            {
                await _usersAPI.AssignUserRolesAsync(userId, roles);
                return Ok();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{userId}/roles")]
        [Authorize]
        public async Task<IActionResult> DeleteUserRoles(string userId, [FromBody] List<string> roles)
        {
            try
            {
                await _usersAPI.DeleteUserRolesAsync(userId, roles);
                return NoContent();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region USER_PERMISSIONS
        [HttpGet("{userId}/permissions")]
        [Authorize]
        public async Task<IActionResult> GetUserPermissions(string userId)
        {
            try
            {
                var permissions = await _usersAPI.GetUserPermissionsAsync(userId);
                return Ok(permissions);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{userId}/permissions")]
        [Authorize]
        public async Task<IActionResult> AssignPermissionsToUser(string userId, [FromBody] List<Auth0Permission> permissions)
        {
            try
            {
                await _usersAPI.AssignPermissionsToUserAsync(userId, permissions);
                return Ok();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{userId}/permissions")]
        [Authorize]
        public async Task<IActionResult> DeletePermissionsFromUser(string userId, [FromBody] List<Auth0Permission> permissions)
        {
            try
            {
                await _usersAPI.DeletePermissionsFromUserAsync(userId, permissions);
                return NoContent();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region USER_LOGS
        [HttpGet("{userId}/logs")]
        [Authorize]
        public async Task<IActionResult> GetUserLogs(string userId)
        {
            try
            {
                var logs = await _usersAPI.GetUserLogsAsync(userId);
                return Ok(logs);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}