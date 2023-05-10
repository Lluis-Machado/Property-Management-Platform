using Accounting.Controllers;
using Accounting.Models;
using Accounting.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace AccountingUnitTests
{
    public class DepreciationsControllerTests
    {

        private readonly Mock<ILogger<DepreciationsController>> _mockLogger;
        private readonly Mock<IValidator<Depreciation>> _mockDepreciationValidator;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IDepreciationRepository> _mockDepreciationRepo;
        private readonly Mock<IDepreciationConfigRepository> _mockDepreciationConfigRepo;
        private readonly Mock<IFixedAssetRepository> _mockFixedAssetRepo;
        private readonly DepreciationsController _depreciationsController;

        public DepreciationsControllerTests()
        {
            _mockLogger = new Mock<ILogger<DepreciationsController>>();
            _mockDepreciationValidator = new Mock<IValidator<Depreciation>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockDepreciationRepo = new Mock<IDepreciationRepository>();
            _mockDepreciationConfigRepo = new Mock<IDepreciationConfigRepository>();
            _mockFixedAssetRepo = new Mock<IFixedAssetRepository>();
            _depreciationsController = new DepreciationsController(_mockDepreciationRepo.Object, _mockDepreciationConfigRepo.Object, _mockFixedAssetRepo.Object, _mockDepreciationValidator.Object, _mockLogger.Object);
        }

        #region Create

        [Fact]
        public async Task CreateAsync_ReturnCreatedResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset { Id = Guid.NewGuid() };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
            };
            var fakeExpectedDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
                Id = Guid.NewGuid(),
                Deleted = false
            };

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeFixedAsset);

            _mockDepreciationRepo
                .Setup(r => r.InsertDepreciationAsync(It.IsAny<Depreciation>()))
                .ReturnsAsync(fakeExpectedDepreciation);

            // Act
            var result = await _depreciationsController.CreateAsync(fakeDepreciation, fakeFixedAsset.Id);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(fakeExpectedDepreciation, createdResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenIdFieldIsNotEmpty()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset { Id = Guid.NewGuid() };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
                Id = Guid.NewGuid()
            };

            // Act
            var result = await _depreciationsController.CreateAsync(fakeDepreciation, fakeFixedAsset.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Depreciation Id field must be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenValidationIsNotValid()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset { Id = Guid.NewGuid() };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _depreciationsController.CreateAsync(fakeDepreciation, fakeFixedAsset.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsNotFoundResult_WhenInvoiceNotFound()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset { Id = Guid.NewGuid() };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
            };

            fakeFixedAsset = null;

            _mockDepreciationValidator
               .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
               .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeFixedAsset);

            // Act
            var result = await _depreciationsController.CreateAsync(fakeDepreciation, fakeDepreciation.FixedAssetId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("FixedAsset not found", notFoundResult.Value);
        }

        #endregion

        #region Get

        [Fact]
        public async Task GetAsync_ReturnsOkResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset { Id = Guid.NewGuid() };
            var fakeExpectedDepreciations = new List<Depreciation>()
            {
                new Depreciation
                {
                    FixedAssetId = fakeFixedAsset.Id,
                    PeriodStart = DateTime.Parse("2023-01-01"),
                    PeriodEnd = DateTime.Parse("2023-01-31"),
                    Amount = 0,
                    Id = Guid.NewGuid(),
                    Deleted = false
                },
                new Depreciation
                {
                    FixedAssetId = fakeFixedAsset.Id,
                    PeriodStart = DateTime.Parse("2023-01-01"),
                    PeriodEnd = DateTime.Parse("2023-01-31"),
                    Amount = 0,
                    Id = Guid.NewGuid(),
                    Deleted = false
                }
            };

            _mockDepreciationRepo
                .Setup(r => r.GetDepreciationsAsync())
                .ReturnsAsync(fakeExpectedDepreciations);

            // Act
            var result = await _depreciationsController.GetAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDepreciations = Assert.IsAssignableFrom<IEnumerable<Depreciation>>(okResult.Value);
            Assert.Equal(fakeExpectedDepreciations, actualDepreciations);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockDepreciationRepo
                .Setup(repo => repo.GetDepreciationsAsync())
                .ThrowsAsync(new Exception());

            // Act
            async Task act() => await _depreciationsController.GetAsync();

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset { Id = Guid.NewGuid() };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
                Id = Guid.NewGuid()
            };

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeFixedAsset);

            _mockDepreciationRepo
                .Setup(r => r.UpdateDepreciationAsync(fakeDepreciation))
                .ReturnsAsync(1);

            // Act
            var result = await _depreciationsController.UpdateAsync(fakeDepreciation, fakeFixedAsset.Id, fakeDepreciation.Id);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenDepreciationIdDoesNotMatch()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset { Id = Guid.NewGuid() };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
                Id = Guid.NewGuid()
            };

            // Act
            var result = await _depreciationsController.UpdateAsync(fakeDepreciation, fakeFixedAsset.Id, Guid.NewGuid());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Depreciation Id from body incorrect", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenDepreciationValidationNotValid()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset { Id = Guid.NewGuid() };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
                Id = Guid.NewGuid()
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _depreciationsController.UpdateAsync(fakeDepreciation, fakeFixedAsset.Id, fakeDepreciation.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenDepreciationNotFound()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset { Id = Guid.NewGuid() };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
                Id = Guid.NewGuid()
            };

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeFixedAsset);

            _mockDepreciationRepo
                .Setup(r => r.UpdateDepreciationAsync(It.IsAny<Depreciation>()))
                .ReturnsAsync(0);

            // Act
            var result = await _depreciationsController.UpdateAsync(fakeDepreciation, fakeFixedAsset.Id, fakeDepreciation.Id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Depreciation not found", notFoundResult.Value);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeDepreciationId = Guid.NewGuid();

            _mockDepreciationRepo
                .Setup(r => r.SetDeleteDepreciationAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(1);

            // Act
            var result = await _depreciationsController.DeleteAsync(fakeDepreciationId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenDepreciationNotFound()
        {
            // Arrange
            var fakeDepreciationId = Guid.NewGuid();

            _mockDepreciationRepo
                .Setup(r => r.SetDeleteDepreciationAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(0);

            // Act
            var result = await _depreciationsController.DeleteAsync(fakeDepreciationId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Depreciation not found", notFoundResult.Value);
        }

        #endregion

        #region Undelete

        [Fact]
        public async Task UndeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeDepreciationId = Guid.NewGuid();

            _mockDepreciationRepo
                .Setup(r => r.SetDeleteDepreciationAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(1);

            // Act
            var result = await _depreciationsController.UndeleteAsync(fakeDepreciationId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UndeleteAsync_ReturnsNotFound_WhenDepreciationNotFound()
        {
            // Arrange
            var fakeDepreciationId = Guid.NewGuid();

            _mockDepreciationRepo
                .Setup(r => r.SetDeleteDepreciationAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(0);

            // Act
            var result = await _depreciationsController.UndeleteAsync(fakeDepreciationId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Depreciation not found", notFoundResult.Value);
        }


        //// GET: Depreciations by Fixed Asset and Period
        //[Authorize]
        //[HttpGet]
        //[Route("{fixedAssetId}/depreciations")]
        //[ProducesResponseType((int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        //public async Task<ActionResult<IEnumerable<Depreciation>>> GetByFAandPeriodAsync(Guid fixedAssetId, DateTime? periodStart, DateTime? periodEnd)

        //// POST: Upsert depreciations
        //[Authorize]
        //[HttpPost]
        //[Route("depreciations/upsert")]
        //[ProducesResponseType((int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        //public async Task<ActionResult<IEnumerable<Depreciation>>> UpsertDepreciation([FromBody] Depreciation depreciation)


        #endregion
    }
}
