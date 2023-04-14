using Auth.Models;
using Auth.Services.Auth0;
using Auth.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
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

        [HttpGet]
        [SecurityControl]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _usersAPI.GetUserListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userId}")]
        [SecurityControl]
        public async Task<IActionResult> GetUser(string userId)
        {
            try
            {
                var user = await _usersAPI.GetUserAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [SecurityControl]
        public async Task<IActionResult> CreateUser([FromBody] Auth0User auth0User)
        {
            try
            {
                var user = await _usersAPI.CreateUserAsync(auth0User);
                return Created($"users/{user}", user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{userId}")]
        [SecurityControl]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] object updatedUser)
        {
            try
            {
                var result = await _usersAPI.UpdateUserAsync(userId, updatedUser);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{userId}")]
        [SecurityControl]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                await _usersAPI.DeleteUserAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}