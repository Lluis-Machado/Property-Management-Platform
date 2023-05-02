using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Documents.Services.AzureBlobStorage;
using FluentValidation;
using Documents.Models;
using Tenants.Controllers;
using Microsoft.Extensions.Logging;
using FluentValidation.Results;
using Azure;
using Newtonsoft.Json.Linq;

namespace DocumentsUnitTests
{
    public class TenantControllerTests
    {
        private readonly Mock<ILogger<TenantsController>> _mockLogger;
        private readonly Mock<IAzureBlobStorage> _mockAzureBlobStorage;
        private readonly Mock<IValidator<Tenant>> _mockTenantValidator;
        private readonly TenantsController _tenantsController;

        public TenantControllerTests()
        {
            _mockLogger = new Mock<ILogger<TenantsController>>();
            _mockAzureBlobStorage = new Mock<IAzureBlobStorage>();
            _mockTenantValidator = new Mock<IValidator<Tenant>>();
            _tenantsController = new TenantsController(_mockAzureBlobStorage.Object, _mockTenantValidator.Object, _mockLogger.Object);
        }

        #region Create
        [Fact]
        public async Task Create_WithValidInput_ReturnsOk()
        {
            //Arrange
            var tenant = new Tenant
            {
                Name = "TestTenant"
            };

            _mockAzureBlobStorage.Setup(x => x.CreateBlobContainerAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            _mockTenantValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Tenant>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            //Act
            var result = await _tenantsController.Create(tenant);

            //Assert
            var okResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal((int)HttpStatusCode.Created, okResult.StatusCode);

            _mockAzureBlobStorage.Verify(s => s.CreateBlobContainerAsync(tenant.Name), Times.Once);
        }

        [Fact(Skip = "Simulate json constructor not yet implemented")]
        public async Task Create_WithNullInput_ReturnsBadRequest()
        {
            //Arrange
            dynamic tenant = new JObject();
            tenant.fake = "fake";

            //Act
            var result = await _tenantsController.Create(tenant);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Incorrect body format", badRequestResult.Value);
        }

        [Fact]
        public async Task Create_WithInvalidInput_ReturnsBadRequest()
        {
            //Arrange
            var tenant = new Tenant
            {
                Name = "",
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });
            _mockTenantValidator.Setup(v => v.ValidateAsync(It.IsAny<Tenant>(),CancellationToken.None))
                                .ReturnsAsync(validationResult);

            //Act
            var result = await _tenantsController.Create(tenant);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }
        #endregion

        #region Get
        [Fact]
        public async Task GetAsync_WhenIncludeDeletedIsFalse_ReturnsOkResultWithTenants()
        {
            // Arrange
            var tenants = new List<Tenant>
            {
            new Tenant { Name = "Tenant1" },
            new Tenant { Name = "Tenant2" },
            new Tenant { Name = "Tenant3" }
            };

            _mockAzureBlobStorage.Setup(x => x.GetBlobContainersAsync(100, false)).ReturnsAsync(tenants);

            // Act
            var result = await _tenantsController.GetAsync(false);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTenants = Assert.IsAssignableFrom<IEnumerable<Tenant>>(okResult.Value);
            Assert.Equal(tenants.Count, returnedTenants.Count());
            Assert.Equal(tenants[0].Name, returnedTenants.First().Name);
            Assert.Equal(tenants[1].Name, returnedTenants.ElementAt(1).Name);
            Assert.Equal(tenants[2].Name, returnedTenants.Last().Name);
        }

        [Fact]
        public async Task GetAsync_WhenIncludeDeletedIsTrue_ReturnsOkResultWithTenants()
        {
            // Arrange
            var tenants = new List<Tenant>
            {
            new Tenant { Name = "Tenant1" },
            new Tenant { Name = "Tenant2" },
            new Tenant { Name = "Tenant3" }
            };
            _mockAzureBlobStorage.Setup(x => x.GetBlobContainersAsync(100, true)).ReturnsAsync(tenants);

            // Act
            var result = await _tenantsController.GetAsync(true);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTenants = Assert.IsAssignableFrom<IEnumerable<Tenant>>(okResult.Value);
            Assert.Equal(tenants.Count, returnedTenants.Count());
            Assert.Equal(tenants[0].Name, returnedTenants.First().Name);
            Assert.Equal(tenants[1].Name, returnedTenants.ElementAt(1).Name);
            Assert.Equal(tenants[2].Name, returnedTenants.Last().Name);
        }

        #endregion

        #region Delete
        [Fact]
        public async Task DeleteAsync_WhenTenantExists_ResturnsOK()
        {
            // Arrange
            string tenantName = "testTenant";
            _mockAzureBlobStorage.Setup(x => x.DeleteBlobContainerAsync(tenantName)).Returns(Task.CompletedTask);

            // Act
            var result = await _tenantsController.DeleteAsync(tenantName);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFoundResult_WhenTenantDoesNotExist()
        {
            // Arrange
            string tenantName = "testTenant";
            var expectedException = new RequestFailedException((int)HttpStatusCode.NotFound, "ContainerNotFound");
            _mockAzureBlobStorage.Setup(x => x.DeleteBlobContainerAsync(tenantName)).ThrowsAsync(expectedException);

            // Act
            async Task act() => await _tenantsController.DeleteAsync(tenantName);

            // Assert
            await Assert.ThrowsAsync<RequestFailedException>(act);
        }
        #endregion

        #region Undelete
        [Fact]
        public async Task UndeleteAsync_WhenTenantExists_ResturnsOK()
        {
            // Arrange
            string tenantName = "testTenant";
            _mockAzureBlobStorage.Setup(x => x.UndeleteBlobContainerAsync(tenantName)).Returns(Task.CompletedTask);

            // Act
            var result = await _tenantsController.UndeleteAsync(tenantName);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UndeleteAsync_ReturnsNotFoundResult_WhenTenantDoesNotExist()
        {
            // Arrange
            string tenantName = "testTenant";
            var expectedException = new RequestFailedException((int)HttpStatusCode.NotFound, "ContainerNotFound");
            _mockAzureBlobStorage.Setup(x => x.UndeleteBlobContainerAsync(tenantName)).ThrowsAsync(expectedException);

            // Act
            async Task act() => await _tenantsController.UndeleteAsync(tenantName);

            // Assert
            await Assert.ThrowsAsync<RequestFailedException>(act);
        }
        #endregion
    }
}
