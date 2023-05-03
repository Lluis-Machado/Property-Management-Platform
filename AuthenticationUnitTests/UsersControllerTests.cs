using Authentication.Controllers;
using Authentication.Models;
using AuthenticationAPI.Services.Auth0.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuthenticationUnitTests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUsersAPI> _mockUsersApi;
        private readonly UsersController _usersController;

        public UsersControllerTests()
        {
            _mockUsersApi = new Mock<IUsersAPI>();
            _usersController = new UsersController(_mockUsersApi.Object);
        }

        [Fact]
        public async Task GetUsers_Success()
        {
            // Arrange
            _mockUsersApi.Setup(api => api.GetUserListAsync()).ReturnsAsync(new List<object>());

            // Act
            var result = await _usersController.GetUsers();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetUser_Success()
        {
            // Arrange
            string userId = "test-user-id";
            _mockUsersApi.Setup(api => api.GetUserAsync(userId)).ReturnsAsync(new Auth0User());

            // Act
            var result = await _usersController.GetUser(userId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CreateUser_Success()
        {
            // Arrange
            var auth0User = new Auth0User();
            _mockUsersApi.Setup(api => api.CreateUserAsync(auth0User)).ReturnsAsync(auth0User);

            // Act
            var result = await _usersController.CreateUser(auth0User);

            // Assert
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task UpdateUser_Success()
        {
            // Arrange
            string userId = "test-user-id";
            object updatedUser = new { email = "test@example.com" };
            _mockUsersApi.Setup(api => api.UpdateUserAsync(userId, updatedUser)).ReturnsAsync(new Auth0User());

            // Act
            var result = await _usersController.UpdateUser(userId, updatedUser);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteUser_Success()
        {
            // Arrange
            string userId = "test-user-id";
            _mockUsersApi.Setup(api => api.DeleteUserAsync(userId)).Returns(Task.CompletedTask);

            // Act
            var result = await _usersController.DeleteUser(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetUserRoles_Success()
        {
            // Arrange
            string userId = "test-user-id";
            _mockUsersApi.Setup(api => api.GetUserRolesAsync(userId)).ReturnsAsync(new List<object>());

            // Act
            var result = await _usersController.GetUserRoles(userId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AssignUserRoles_Success()
        {
            // Arrange
            string userId = "test-user-id";
            List<string> roles = new() { "test-role" };
            _mockUsersApi.Setup(api => api.AssignUserRolesAsync(userId, roles)).Returns(Task.CompletedTask);

            // Act
            var result = await _usersController.AssignUserRoles(userId, roles);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUserRoles_Success()
        {
            // Arrange
            string userId = "test-user-id";
            List<string> roles = new List<string> { "test-role" };
            _mockUsersApi.Setup(api => api.DeleteUserRolesAsync(userId, roles)).Returns(Task.CompletedTask);

            // Act
            var result = await _usersController.DeleteUserRoles(userId, roles);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetUserPermissions_Success()
        {
            // Arrange
            string userId = "test-user-id";
            _mockUsersApi.Setup(api => api.GetUserPermissionsAsync(userId)).ReturnsAsync(new List<object>());

            // Act
            var result = await _usersController.GetUserPermissions(userId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AssignPermissionsToUser_Success()
        {
            // Arrange
            string userId = "test-user-id";
            List<Auth0Permission> permissions = new() { new Auth0Permission { permission_name = "read:test" } };
            _mockUsersApi.Setup(api => api.AssignPermissionsToUserAsync(userId, permissions)).Returns(Task.CompletedTask);

            // Act
            var result = await _usersController.AssignPermissionsToUser(userId, permissions);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeletePermissionsFromUser_Success()
        {
            // Arrange
            string userId = "test-user-id";
            List<Auth0Permission> permissions = new() { new Auth0Permission { permission_name = "read:test" } };
            _mockUsersApi.Setup(api => api.DeletePermissionsFromUserAsync(userId, permissions)).Returns(Task.CompletedTask);

            // Act
            var result = await _usersController.DeletePermissionsFromUser(userId, permissions);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetUserLogs_Success()
        {
            // Arrange
            string userId = "test-user-id";
            _mockUsersApi.Setup(api => api.GetUserLogsAsync(userId)).ReturnsAsync(new List<object>());

            // Act
            var result = await _usersController.GetUserLogs(userId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}