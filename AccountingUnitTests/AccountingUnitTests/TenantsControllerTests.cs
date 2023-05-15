using Accounting.Controllers;
using Accounting.Models;
using Accounting.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AccountingUnitTests
{
    public class TenantsControllerTests
    {
        private readonly Mock<ILogger<TenantsController>> _mockLogger;
        private readonly Mock<IValidator<Tenant>> _mockTenantValidator;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ITenantRepository> _mockTenantRepo;
        private readonly TenantsController _tenantsController;

        public TenantsControllerTests()
        {
            _mockLogger = new Mock<ILogger<TenantsController>>();
            _mockTenantValidator = new Mock<IValidator<Tenant>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockTenantRepo = new Mock<ITenantRepository>();
            _tenantsController = new TenantsController(_mockTenantRepo.Object, _mockTenantValidator.Object, _mockLogger.Object);
        }

        #region Create

        [Fact]
        public async Task CreateAsync_ReturnCreatedResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeTenant = new Tenant
            {
                Name = "fakeName",
            };
            var fakeExpectedTenant = new Tenant
            {
                Name = "fakeName",
                Id = Guid.NewGuid(),
                Deleted = false
            };

            _mockTenantValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Tenant>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockTenantRepo
                .Setup(r => r.InsertTenantAsync(It.IsAny<Tenant>()))
                .ReturnsAsync(fakeExpectedTenant);

            // Act
            var result = await _tenantsController.CreateAsync(fakeTenant);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(fakeExpectedTenant, createdResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenIdFieldIsNotEmpty()
        {
            // Arrange
            var fakeTenant = new Tenant
            {
                Name = "fakeName",
                Id = Guid.NewGuid()
            };

            // Act
            var result = await _tenantsController.CreateAsync(fakeTenant);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Tenant Id field must be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenValidationIsNotValid()
        {
            // Arrange
            var fakeTenant = new Tenant
            {
                Name = "fakeName",
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockTenantValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Tenant>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _tenantsController.CreateAsync(fakeTenant);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        #endregion

        #region Get

        [Fact]
        public async Task GetAsync_ReturnsOkResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeExpectedTenants = new List<Tenant>()
            {
                new Tenant
                {
                    Name = "fakeName",
                    Id = Guid.NewGuid(),
                    Deleted = false
                },
                new Tenant
                {
                    Name = "fakeName",
                    Id = Guid.NewGuid(),
                    Deleted = false
                }
            };

            _mockTenantRepo
                .Setup(r => r.GetTenantsAsync(false))
                .ReturnsAsync(fakeExpectedTenants);

            // Act
            var result = await _tenantsController.GetAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualTenants = Assert.IsAssignableFrom<IEnumerable<Tenant>>(okResult.Value);
            Assert.Equal(fakeExpectedTenants, actualTenants);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockTenantRepo
                .Setup(repo => repo.GetTenantsAsync(false))
                .ThrowsAsync(new Exception());

            // Act
            async Task act() => await _tenantsController.GetAsync();

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeTenant = new Tenant
            {
                Name = "fakeName",
                Id = Guid.NewGuid()
            };

            _mockTenantValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Tenant>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockTenantRepo
                .Setup(r => r.UpdateTenantAsync(fakeTenant))
                .ReturnsAsync(1);

            // Act
            var result = await _tenantsController.UpdateAsync(fakeTenant, fakeTenant.Id);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenTenantIdDoesNotMatch()
        {
            // Arrange
            var fakeTenant = new Tenant
            {
                Name = "fakeName",
                Id = Guid.NewGuid()
            }; ;

            // Act
            var result = await _tenantsController.UpdateAsync(fakeTenant, Guid.NewGuid());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Tenant Id from body incorrect", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenTenantValidationNotValid()
        {
            // Arrange
            var fakeTenant = new Tenant
            {
                Name = "fakeName",
                Id = Guid.NewGuid()
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockTenantValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Tenant>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _tenantsController.UpdateAsync(fakeTenant, fakeTenant.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenTenantNotFound()
        {
            // Arrange
            var fakeTenant = new Tenant
            {
                Name = "fakeName",
                Id = Guid.NewGuid()
            };

            _mockTenantValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Tenant>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockTenantRepo
                .Setup(r => r.UpdateTenantAsync(It.IsAny<Tenant>()))
                .ReturnsAsync(0);

            // Act
            var result = await _tenantsController.UpdateAsync(fakeTenant, fakeTenant.Id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Tenant not found", notFoundResult.Value);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeTenantId = Guid.NewGuid();

            _mockTenantRepo
                .Setup(r => r.SetDeleteTenantAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(1);

            // Act
            var result = await _tenantsController.DeleteAsync(fakeTenantId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenTenantNotFound()
        {
            // Arrange
            var fakeTenantId = Guid.NewGuid();

            _mockTenantRepo
                .Setup(r => r.SetDeleteTenantAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(0);

            // Act
            var result = await _tenantsController.DeleteAsync(fakeTenantId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Tenant not found", notFoundResult.Value);
        }

        #endregion

        #region Undelete

        [Fact]
        public async Task UndeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeTenantId = Guid.NewGuid();

            _mockTenantRepo
                .Setup(r => r.SetDeleteTenantAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(1);

            // Act
            var result = await _tenantsController.UndeleteAsync(fakeTenantId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UndeleteAsync_ReturnsNotFound_WhenTenantNotFound()
        {
            // Arrange
            var fakeTenantId = Guid.NewGuid();

            _mockTenantRepo
                .Setup(r => r.SetDeleteTenantAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(0);

            // Act
            var result = await _tenantsController.UndeleteAsync(fakeTenantId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Tenant not found", notFoundResult.Value);
        }

        #endregion
    }
}
