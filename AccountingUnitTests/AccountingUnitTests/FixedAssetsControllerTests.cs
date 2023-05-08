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
    public class FixedAssetsControllerTests
    {

        private readonly Mock<ILogger<FixedAssetsController>> _mockLogger;
        private readonly Mock<IValidator<FixedAsset>> _mockFixedAssetValidator;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IFixedAssetRepository> _mockFixedAssetRepo;
        private readonly FixedAssetsController _fixedAssetsController;

        public FixedAssetsControllerTests()
        {
            _mockLogger = new Mock<ILogger<FixedAssetsController>>();
            _mockFixedAssetValidator = new Mock<IValidator<FixedAsset>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockFixedAssetRepo = new Mock<IFixedAssetRepository>();
            _fixedAssetsController = new FixedAssetsController(_mockFixedAssetRepo.Object, _mockFixedAssetValidator.Object, _mockLogger.Object);
        }

        #region Create

        [Fact]
        public async Task CreateAsync_ReturnCreatedResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset
            {
                InvoiceId = Guid.NewGuid(),
                Name = "FakeName",
                ActivationDate = DateTime.Now,
                ActivationAmount = 0,
                DepreciationConfigId = Guid.NewGuid(),
                DepreciationAmount = 0,
            };
            var fakeExpectedFixedAsset = new FixedAsset
            {
                InvoiceId = Guid.NewGuid(),
                Name = "FakeName",
                ActivationDate = DateTime.Now,
                ActivationAmount = 0,
                DepreciationConfigId = Guid.NewGuid(),
                DepreciationAmount = 0,
                Id = Guid.NewGuid(),
                Deleted = false
            };

            _mockFixedAssetValidator
                .Setup(v => v.ValidateAsync(It.IsAny<FixedAsset>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.InsertFixedAssetAsync(It.IsAny<FixedAsset>()))
                .ReturnsAsync(fakeExpectedFixedAsset);

            // Act
            var result = await _fixedAssetsController.CreateAsync(fakeFixedAsset);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(fakeExpectedFixedAsset, createdResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenIdFieldIsNotEmpty()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset
            {
                InvoiceId = Guid.NewGuid(),
                Name = "FakeName",
                ActivationDate = DateTime.Now,
                ActivationAmount = 0,
                DepreciationConfigId = Guid.NewGuid(),
                DepreciationAmount = 0,
                Id = Guid.NewGuid()
            };

            // Act
            var result = await _fixedAssetsController.CreateAsync(fakeFixedAsset);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("FixedAsset Id field must be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenValidationIsNotValid()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset
            {
                InvoiceId = Guid.NewGuid(),
                Name = "FakeName",
                ActivationDate = DateTime.Now,
                ActivationAmount = 0,
                DepreciationConfigId = Guid.NewGuid(),
                DepreciationAmount = 0,
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockFixedAssetValidator
                .Setup(v => v.ValidateAsync(It.IsAny<FixedAsset>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _fixedAssetsController.CreateAsync(fakeFixedAsset);

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
            var fakeExpectedFixedAssets = new List<FixedAsset>()
            {
                new FixedAsset
                {
                    InvoiceId = Guid.NewGuid(),
                    Name = "FakeName",
                    ActivationDate = DateTime.Now,
                    ActivationAmount = 0,
                    DepreciationConfigId = Guid.NewGuid(),
                    DepreciationAmount = 0,
                    Id = Guid.NewGuid(),
                    Deleted = false
                },
                new FixedAsset
                {
                    InvoiceId = Guid.NewGuid(),
                    Name = "FakeName",
                    ActivationDate = DateTime.Now,
                    ActivationAmount = 0,
                    DepreciationConfigId = Guid.NewGuid(),
                    DepreciationAmount = 0,
                    Id = Guid.NewGuid(),
                    Deleted = false
                }
            };

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetsAsync())
                .ReturnsAsync(fakeExpectedFixedAssets);

            // Act
            var result = await _fixedAssetsController.GetAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualFixedAssets = Assert.IsAssignableFrom<IEnumerable<FixedAsset>>(okResult.Value);
            Assert.Equal(fakeExpectedFixedAssets, actualFixedAssets);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockFixedAssetRepo
                .Setup(repo => repo.GetFixedAssetsAsync())
                .ThrowsAsync(new Exception());

            // Act
            async Task act() => await _fixedAssetsController.GetAsync();

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset
            {
                InvoiceId = Guid.NewGuid(),
                Name = "FakeName",
                ActivationDate = DateTime.Now,
                ActivationAmount = 0,
                DepreciationConfigId = Guid.NewGuid(),
                DepreciationAmount = 0,
                Id = Guid.NewGuid()
            };

            _mockFixedAssetValidator
                .Setup(v => v.ValidateAsync(It.IsAny<FixedAsset>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.UpdateFixedAssetAsync(fakeFixedAsset))
                .ReturnsAsync(1);

            // Act
            var result = await _fixedAssetsController.UpdateAsync(fakeFixedAsset, fakeFixedAsset.Id);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenFixedAssetIdDoesNotMatch()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset
            {
                InvoiceId = Guid.NewGuid(),
                Name = "FakeName",
                ActivationDate = DateTime.Now,
                ActivationAmount = 0,
                DepreciationConfigId = Guid.NewGuid(),
                DepreciationAmount = 0,
                Id = Guid.NewGuid()
            }; ;

            // Act
            var result = await _fixedAssetsController.UpdateAsync(fakeFixedAsset, Guid.NewGuid());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("FixedAsset Id from body incorrect", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenFixedAssetValidationNotValid()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset
            {
                InvoiceId = Guid.NewGuid(),
                Name = "FakeName",
                ActivationDate = DateTime.Now,
                ActivationAmount = 0,
                DepreciationConfigId = Guid.NewGuid(),
                DepreciationAmount = 0,
                Id = Guid.NewGuid()
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockFixedAssetValidator
                .Setup(v => v.ValidateAsync(It.IsAny<FixedAsset>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _fixedAssetsController.UpdateAsync(fakeFixedAsset, fakeFixedAsset.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenFixedAssetNotFound()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset
            {
                InvoiceId = Guid.NewGuid(),
                Name = "FakeName",
                ActivationDate = DateTime.Now,
                ActivationAmount = 0,
                DepreciationConfigId = Guid.NewGuid(),
                DepreciationAmount = 0,
                Id = Guid.NewGuid()
            };

            _mockFixedAssetValidator
                .Setup(v => v.ValidateAsync(It.IsAny<FixedAsset>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.UpdateFixedAssetAsync(It.IsAny<FixedAsset>()))
                .ReturnsAsync(0);

            // Act
            var result = await _fixedAssetsController.UpdateAsync(fakeFixedAsset, fakeFixedAsset.Id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("FixedAsset not found", notFoundResult.Value);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeFixedAssetId = Guid.NewGuid();

            _mockFixedAssetRepo
                .Setup(r => r.SetDeleteFixedAssetAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(1);

            // Act
            var result = await _fixedAssetsController.DeleteAsync(fakeFixedAssetId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenFixedAssetNotFound()
        {
            // Arrange
            var fakeFixedAssetId = Guid.NewGuid();

            _mockFixedAssetRepo
                .Setup(r => r.SetDeleteFixedAssetAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(0);

            // Act
            var result = await _fixedAssetsController.DeleteAsync(fakeFixedAssetId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("FixedAsset not found", notFoundResult.Value);
        }

        #endregion

        #region Undelete

        [Fact]
        public async Task UndeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeFixedAssetId = Guid.NewGuid();

            _mockFixedAssetRepo
                .Setup(r => r.SetDeleteFixedAssetAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(1);

            // Act
            var result = await _fixedAssetsController.UndeleteAsync(fakeFixedAssetId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UndeleteAsync_ReturnsNotFound_WhenFixedAssetNotFound()
        {
            // Arrange
            var fakeFixedAssetId = Guid.NewGuid();

            _mockFixedAssetRepo
                .Setup(r => r.SetDeleteFixedAssetAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(0);

            // Act
            var result = await _fixedAssetsController.UndeleteAsync(fakeFixedAssetId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("FixedAsset not found", notFoundResult.Value);
        }

        #endregion
    }
}
