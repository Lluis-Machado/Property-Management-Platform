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
    public class DepreciationConfigsControllerTests
    {

        private readonly Mock<ILogger<DepreciationConfigsController>> _mockLogger;
        private readonly Mock<IValidator<DepreciationConfig>> _mockDepreciationConfigValidator;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IDepreciationConfigRepository> _mockDepreciationConfigRepo;
        private readonly DepreciationConfigsController _depreciationConfigsController;

        public DepreciationConfigsControllerTests()
        {
            _mockLogger = new Mock<ILogger<DepreciationConfigsController>>();
            _mockDepreciationConfigValidator = new Mock<IValidator<DepreciationConfig>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockDepreciationConfigRepo = new Mock<IDepreciationConfigRepository>();
            _depreciationConfigsController = new DepreciationConfigsController(_mockDepreciationConfigRepo.Object, _mockDepreciationConfigValidator.Object, _mockLogger.Object);
        }

        #region Create

        [Fact]
        public async Task CreateAsync_ReturnCreatedResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeDepreciationConfig = new DepreciationConfig
            {
                Type = "fakeType",
                DepreciationPercent = 0,
                MaxYears = 10,
            };
            var fakeExpectedDepreciationConfig = new DepreciationConfig
            {
                Type = "fakeType",
                DepreciationPercent = 0,
                MaxYears = 10,
                CustomSetPercent = false,
                CustomSetYears = false,
                Id = Guid.NewGuid(),
                Deleted = false
            };

            _mockDepreciationConfigValidator
                .Setup(v => v.ValidateAsync(It.IsAny<DepreciationConfig>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockDepreciationConfigRepo
                .Setup(r => r.InsertDepreciationConfigAsync(It.IsAny<DepreciationConfig>()))
                .ReturnsAsync(fakeExpectedDepreciationConfig);

            // Act
            var result = await _depreciationConfigsController.CreateAsync(fakeDepreciationConfig);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(fakeExpectedDepreciationConfig, createdResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenIdFieldIsNotEmpty()
        {
            // Arrange
            var fakeDepreciationConfig = new DepreciationConfig
            {
                Type = "fakeType",
                DepreciationPercent = 0,
                Id = Guid.NewGuid()
            };

            // Act
            var result = await _depreciationConfigsController.CreateAsync(fakeDepreciationConfig);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("DepreciationConfig Id field must be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenValidationIsNotValid()
        {
            // Arrange
            var fakeDepreciationConfig = new DepreciationConfig
            {
                Type = "fakeType",
                DepreciationPercent = 0,
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockDepreciationConfigValidator
                .Setup(v => v.ValidateAsync(It.IsAny<DepreciationConfig>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _depreciationConfigsController.CreateAsync(fakeDepreciationConfig);

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
            var fakeExpectedDepreciationConfigs = new List<DepreciationConfig>()
            {
                new DepreciationConfig
                {
                    Type = "fakeType",
                    DepreciationPercent = 0,
                    Id = Guid.NewGuid(),
                    Deleted = false
                },
                new DepreciationConfig
                {
                    Type = "fakeType",
                    DepreciationPercent = 0,
                    Id = Guid.NewGuid(),
                    Deleted = false
                }
            };

            _mockDepreciationConfigRepo
                .Setup(r => r.GetDepreciationConfigsAsync())
                .ReturnsAsync(fakeExpectedDepreciationConfigs);

            // Act
            var result = await _depreciationConfigsController.GetAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDepreciationConfigs = Assert.IsAssignableFrom<IEnumerable<DepreciationConfig>>(okResult.Value);
            Assert.Equal(fakeExpectedDepreciationConfigs, actualDepreciationConfigs);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockDepreciationConfigRepo
                .Setup(repo => repo.GetDepreciationConfigsAsync())
                .ThrowsAsync(new Exception());

            // Act
            async Task act() => await _depreciationConfigsController.GetAsync();

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeDepreciationConfig = new DepreciationConfig
            {
                Type = "fakeType",
                DepreciationPercent = 0,
                Id = Guid.NewGuid()
            };

            _mockDepreciationConfigValidator
                .Setup(v => v.ValidateAsync(It.IsAny<DepreciationConfig>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockDepreciationConfigRepo
                .Setup(r => r.UpdateDepreciationConfigAsync(fakeDepreciationConfig))
                .ReturnsAsync(1);

            // Act
            var result = await _depreciationConfigsController.UpdateAsync(fakeDepreciationConfig, fakeDepreciationConfig.Id);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenDepreciationConfigIdDoesNotMatch()
        {
            // Arrange
            var fakeDepreciationConfig = new DepreciationConfig
            {
                Type = "fakeType",
                DepreciationPercent = 0,
                Id = Guid.NewGuid()
            }; ;

            // Act
            var result = await _depreciationConfigsController.UpdateAsync(fakeDepreciationConfig, Guid.NewGuid());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("DepreciationConfig Id from body incorrect", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenDepreciationConfigValidationNotValid()
        {
            // Arrange
            var fakeDepreciationConfig = new DepreciationConfig
            {
                Type = "fakeType",
                DepreciationPercent = 0,
                Id = Guid.NewGuid()
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockDepreciationConfigValidator
                .Setup(v => v.ValidateAsync(It.IsAny<DepreciationConfig>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _depreciationConfigsController.UpdateAsync(fakeDepreciationConfig, fakeDepreciationConfig.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenDepreciationConfigNotFound()
        {
            // Arrange
            var fakeDepreciationConfig = new DepreciationConfig
            {
                Type = "fakeType",
                DepreciationPercent = 0,
                Id = Guid.NewGuid()
            };

            _mockDepreciationConfigValidator
                .Setup(v => v.ValidateAsync(It.IsAny<DepreciationConfig>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockDepreciationConfigRepo
                .Setup(r => r.UpdateDepreciationConfigAsync(It.IsAny<DepreciationConfig>()))
                .ReturnsAsync(0);

            // Act
            var result = await _depreciationConfigsController.UpdateAsync(fakeDepreciationConfig, fakeDepreciationConfig.Id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("DepreciationConfig not found", notFoundResult.Value);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeDepreciationConfigId = Guid.NewGuid();

            _mockDepreciationConfigRepo
                .Setup(r => r.SetDeleteDepreciationConfigAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(1);

            // Act
            var result = await _depreciationConfigsController.DeleteAsync(fakeDepreciationConfigId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenDepreciationConfigNotFound()
        {
            // Arrange
            var fakeDepreciationConfigId = Guid.NewGuid();

            _mockDepreciationConfigRepo
                .Setup(r => r.SetDeleteDepreciationConfigAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(0);

            // Act
            var result = await _depreciationConfigsController.DeleteAsync(fakeDepreciationConfigId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("DepreciationConfig not found", notFoundResult.Value);
        }

        #endregion

        #region Undelete

        [Fact]
        public async Task UndeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeDepreciationConfigId = Guid.NewGuid();

            _mockDepreciationConfigRepo
                .Setup(r => r.SetDeleteDepreciationConfigAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(1);

            // Act
            var result = await _depreciationConfigsController.UndeleteAsync(fakeDepreciationConfigId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UndeleteAsync_ReturnsNotFound_WhenDepreciationConfigNotFound()
        {
            // Arrange
            var fakeDepreciationConfigId = Guid.NewGuid();

            _mockDepreciationConfigRepo
                .Setup(r => r.SetDeleteDepreciationConfigAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(0);

            // Act
            var result = await _depreciationConfigsController.UndeleteAsync(fakeDepreciationConfigId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("DepreciationConfig not found", notFoundResult.Value);
        }

        #endregion
    }
}
