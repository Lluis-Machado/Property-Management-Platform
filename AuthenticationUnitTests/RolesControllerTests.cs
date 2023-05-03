using Authentication.Controllers;
using Authentication.Models;
using AuthenticationAPI.Services.Auth0.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuthenticationUnitTests
{
    public class RolesControllerTests
    {
        private readonly Mock<IRolesAPI> _mockRolesAPI;
        private readonly RolesController _rolesController;

        public RolesControllerTests()
        {
            _mockRolesAPI = new Mock<IRolesAPI>();
            _rolesController = new RolesController(_mockRolesAPI.Object);
        }

        [Fact]
        public async Task GetRoles_ReturnsOk()
        {
            // Arrange
            var roles = new List<object> { new Auth0Role { name = "role1", description = "Role 1" } };
            _mockRolesAPI.Setup(api => api.GetRolesListAsync()).ReturnsAsync(roles);

            // Act
            var result = await _rolesController.GetRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedRoles = Assert.IsType<List<object>>(okResult.Value);
            Assert.Single(returnedRoles);
            Assert.Equal(roles, returnedRoles);
        }

        [Fact]
        public async Task GetRole_ReturnsOk()
        {
            // Arrange
            string roleId = "rol_WbeVspftFq119Zq7";
            var role = new object();
            _mockRolesAPI.Setup(api => api.GetRoleAsync(roleId)).ReturnsAsync(role);

            // Act
            var result = await _rolesController.GetRole(roleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedRole = Assert.IsType<object>(okResult.Value);
            Assert.Equal(role, returnedRole);
        }

        [Fact]
        public async Task CreateRole_ReturnsOk()
        {
            // Arrange
            var roleToCreate = new Auth0Role { name = "Role 1", description = "test role" };
            var createdRole = new Auth0Role { name = "Role 1", description = "test role" };
            _mockRolesAPI.Setup(api => api.CreateRoleAsync(roleToCreate)).ReturnsAsync(createdRole);

            // Act
            var result = await _rolesController.CreateRole(roleToCreate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedRole = Assert.IsType<Auth0Role>(okResult.Value);
            Assert.Equal(createdRole, returnedRole);
        }

        [Fact]
        public async Task UpdateRole_ReturnsOk()
        {
            // Arrange
            string roleId = "rol_Y4Va2WQuAOzdQVKG";
            var roleUpdate = new { name = "Updated Role 1" };
            var updatedRole = new Auth0Role { name = "Updated Role 1", description = "test role" };
            _mockRolesAPI.Setup(api => api.UpdateRoleAsync(roleId, roleUpdate)).ReturnsAsync(updatedRole);

            // Act
            var result = await _rolesController.UpdateRole(roleId, roleUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedRole = Assert.IsType<Auth0Role>(okResult.Value);
            Assert.Equal(updatedRole, returnedRole);
        }

        [Fact]
        public async Task DeleteRole_ReturnsOk()
        {
            // Arrange
            string roleId = "role1";
            _mockRolesAPI.Setup(api => api.DeleteRoleAsync(roleId)).Returns(Task.CompletedTask);

            // Act
            var result = await _rolesController.DeleteRole(roleId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetRolePermissions_ReturnsOk()
        {
            // Arrange
            string roleId = "rol_WbeVspftFq119Zq7";
            var permissions = new List<object> { new Auth0Permission { resource_server_identifier = "perm1", permission_name = "Permission 1" } };
            _mockRolesAPI.Setup(api => api.GetRolePermissionsAsync(roleId)).ReturnsAsync(permissions);

            // Act
            var result = await _rolesController.GetRolePermissions(roleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPermissions = Assert.IsType<List<object>>(okResult.Value);
            Assert.Equal(permissions, returnedPermissions);
        }

        [Fact]
        public async Task AssignPermissionsToRole_ReturnsOk()
        {
            // Arrange
            string roleId = "role1";
            var permissions = new List<Auth0Permission> { new Auth0Permission { resource_server_identifier = "perm1", permission_name = "Permission 1" } };
            _mockRolesAPI.Setup(api => api.AssignPermissionsToRoleAsync(roleId, permissions)).Returns(Task.CompletedTask);

            // Act
            var result = await _rolesController.AssignPermissionsToRole(roleId, permissions);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeletePermissionsFromRole_ReturnsOk()
        {
            // Arrange
            string roleId = "role1";
            var permissions = new List<Auth0Permission> { new Auth0Permission { resource_server_identifier = "perm1", permission_name = "Permission 1" } };
            _mockRolesAPI.Setup(api => api.DeletePermissionsFromRoleAsync(roleId, permissions)).Returns(Task.CompletedTask);

            // Act
            var result = await _rolesController.DeletePermissionsFromRole(roleId, permissions);

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}