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
                .Setup(r => r.GetDepreciationsAsync(false))
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
                .Setup(repo => repo.GetDepreciationsAsync(false))
                .ThrowsAsync(new Exception());

            // Act
            async Task act() => await _depreciationsController.GetAsync();

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        [Fact]
        public async Task GetByFAandPeriodAsync_ReturnsOkResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeFixedAssetId = Guid.NewGuid();
            var fakePeriodStart = DateTime.Parse("2023-01-01");
            var fakePeriodEnd = DateTime.Parse("2023-01-31");
            var fakeDepreciations = new List<Depreciation>
            {
                new Depreciation { Id = Guid.NewGuid(), FixedAssetId = fakeFixedAssetId, PeriodStart = fakePeriodStart, PeriodEnd = fakePeriodEnd, Amount = 10 },
                new Depreciation { Id = Guid.NewGuid(), FixedAssetId = fakeFixedAssetId, PeriodStart = fakePeriodStart, PeriodEnd = fakePeriodEnd, Amount = 20 },
                new Depreciation { Id = Guid.NewGuid(), FixedAssetId = fakeFixedAssetId, PeriodStart = fakePeriodStart, PeriodEnd = fakePeriodEnd, Amount = 30 }
            };

            _mockDepreciationRepo
                .Setup(r => r.GetDepreciationByFAandPeriodAsync(fakeFixedAssetId, false, fakePeriodStart, fakePeriodEnd))
                .ReturnsAsync(fakeDepreciations);

            // Act
            var result = await _depreciationsController.GetByFAandPeriodAsync(fakeFixedAssetId, false, fakePeriodStart, fakePeriodEnd);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDepreciations = Assert.IsAssignableFrom<IEnumerable<Depreciation>>(okResult.Value);
            Assert.Equal(fakeDepreciations, actualDepreciations);
        }

        [Fact]
        public async Task GetByFAandPeriodAsync_ReturnsBadRequestResult_WhenPeriodEndIsBeforePeriodStart()
        {
            // Arrange
            var fakeFixedAssetId = Guid.NewGuid();
            var fakePeriodStart = DateTime.Parse("2023-01-31");
            var fakePeriodEnd = DateTime.Parse("2023-01-01");

            // Act
            var result = await _depreciationsController.GetByFAandPeriodAsync(fakeFixedAssetId, false, fakePeriodStart, fakePeriodEnd);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Period end cannot be before period start", badRequestResult.Value);
        }

        [Fact]
        public async Task GetByFAandPeriodAsync_ReturnsOkResult_WhenPeriodStartIsNull()
        {
            // Arrange
            var fakeFixedAssetId = Guid.NewGuid();
            var fakeDepreciation = new Depreciation { FixedAssetId = fakeFixedAssetId };
            var fakeDepreciations = new List<Depreciation> { fakeDepreciation };

            _mockDepreciationRepo
                .Setup(r => r.GetDepreciationByFAandPeriodAsync(It.IsAny<Guid>(), false, null, null))
                .ReturnsAsync(fakeDepreciations);

            // Act
            var result = await _depreciationsController.GetByFAandPeriodAsync(fakeFixedAssetId, false, null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDepreciations = Assert.IsAssignableFrom<IEnumerable<Depreciation>>(okResult.Value);
            Assert.Equal(fakeDepreciations, returnedDepreciations);
        }

        [Fact]
        public async Task GetByFAandPeriodAsync_ReturnsOkResult_WhenPeriodEndIsNull()
        {
            // Arrange
            var fakeFixedAssetId = Guid.NewGuid();
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAssetId,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31")
            };
            var fakeDepreciations = new List<Depreciation> { fakeDepreciation };

            _mockDepreciationRepo
                .Setup(r => r.GetDepreciationByFAandPeriodAsync(It.IsAny<Guid>(), false, DateTime.Parse("2023-01-01"), null))
                .ReturnsAsync(fakeDepreciations);

            // Act
            var result = await _depreciationsController.GetByFAandPeriodAsync(fakeFixedAssetId, false, DateTime.Parse("2023-01-01"), null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDepreciations = Assert.IsAssignableFrom<IEnumerable<Depreciation>>(okResult.Value);
            Assert.Equal(fakeDepreciations, returnedDepreciations);
        }


        [Fact]
        public async Task GetByFAandPeriodAsync_ReturnsInternalServerErrorResult_WhenRepositoryThrowsException()
        {
            // Arrange
            _mockDepreciationRepo
                .Setup(repo => repo.GetDepreciationByFAandPeriodAsync(new Guid(), false, null, null))
                .ThrowsAsync(new Exception());

            // Act
            async Task act() => await _depreciationsController.GetByFAandPeriodAsync(new Guid(), false, null, null);

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
        public async Task UpdateAsync_ReturnsBadRequest_WhenDepreciationIdDoesNotMatch()
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
        public async Task UpdateAsync_ReturnsBadRequest_WhenDepreciationValidationNotValid()
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


        [Fact]
        public async Task UpsertDepreciation_ReturnsBadRequest_WhenDepreciationIsNull()
        {
            // Act
            var result = await _depreciationsController.UpsertDepreciation(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Incorrect body format", badRequestResult.Value);
        }


        [Fact]
        public async Task UpsertDepreciation_ReturnsBadRequest_WhenDepreciationPeriodEndIsBeforePeriodStart()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset { Id = Guid.NewGuid() };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2022-01-31"),
                Amount = 0
            };

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeFixedAsset);

            // Act
            var result = await _depreciationsController.UpsertDepreciation(fakeDepreciation);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Period end cannot be before period start", badRequestResult.Value);
        }

        [Fact]
        public async Task UpsertDepreciation_ReturnsNotFound_WhenFixedAssetDoesNotExist()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset { Id = Guid.NewGuid() };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0
            };

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _depreciationsController.UpsertDepreciation(fakeDepreciation);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("FixedAsset not found", notFoundResult.Value);
        }

        [Fact]
        public async Task UpsertDepreciation_ReturnsOk_WhenDepreciationAlreadyExists_ExactMatch()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset { Id = Guid.NewGuid() };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0
            };
            var fakeDepreciationsFound = new List<Depreciation> { fakeDepreciation };

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeFixedAsset);

            _mockDepreciationRepo
                .Setup(r => r.GetDepreciationByFAandPeriodAsync(It.IsAny<Guid>(), false, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(fakeDepreciationsFound);


            // Act
            var result = await _depreciationsController.UpsertDepreciation(fakeDepreciation);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(fakeDepreciation.Amount, ((Depreciation)okResult.Value).Amount);
            Assert.Equal(fakeDepreciation.PeriodStart, ((Depreciation)okResult.Value).PeriodStart);
            Assert.Equal(fakeDepreciation.PeriodEnd, ((Depreciation)okResult.Value).PeriodEnd);
        }

        [Fact]
        public async Task UpsertDepreciation_ReturnsOk_UpdatesExistingDepreciationWithOverlap_LinearPercent_PeriodStartOnly()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset { 
                Id = Guid.NewGuid(),
                DepreciationAmountPercent = 20,
            };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
                Id = new Guid()
            };
            var fakeDepreciationsFound = new List<Depreciation> { fakeDepreciation };

            var fakeDepreciationToUpdate = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2022-12-25"),
                PeriodEnd = DateTime.Parse("2023-01-10")
            };

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeFixedAsset);

            _mockDepreciationRepo
                .Setup(r => r.GetDepreciationByFAandPeriodAsync(It.IsAny<Guid>(), false, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(fakeDepreciationsFound);


            // Act
            var result = await _depreciationsController.UpsertDepreciation(fakeDepreciationToUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(fakeDepreciationToUpdate.Amount, ((Depreciation)okResult.Value).Amount);
            Assert.Equal(fakeDepreciationToUpdate.PeriodStart, ((Depreciation)okResult.Value).PeriodStart);
            Assert.Equal(fakeDepreciation.PeriodEnd, ((Depreciation)okResult.Value).PeriodEnd);
            Assert.Equal(fakeDepreciation.Id, ((Depreciation)okResult.Value).Id);
        }

        [Fact]
        public async Task UpsertDepreciation_ReturnsOk_UpdatesExistingDepreciationWithOverlap_LinearYears_PeriodStartOnly()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset
            {
                Id = Guid.NewGuid(),
                DepreciationMaxYears = 10
            };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
                Id = new Guid()
            };
            var fakeDepreciationsFound = new List<Depreciation> { fakeDepreciation };

            var fakeDepreciationToUpdate = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2022-12-25"),
                PeriodEnd = DateTime.Parse("2023-01-10")
            };

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeFixedAsset);

            _mockDepreciationRepo
                .Setup(r => r.GetDepreciationByFAandPeriodAsync(It.IsAny<Guid>(), false, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(fakeDepreciationsFound);


            // Act
            var result = await _depreciationsController.UpsertDepreciation(fakeDepreciationToUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(fakeDepreciationToUpdate.Amount, ((Depreciation)okResult.Value).Amount);
            Assert.Equal(fakeDepreciationToUpdate.PeriodStart, ((Depreciation)okResult.Value).PeriodStart);
            Assert.Equal(fakeDepreciation.PeriodEnd, ((Depreciation)okResult.Value).PeriodEnd);
            Assert.Equal(fakeDepreciation.Id, ((Depreciation)okResult.Value).Id);
        }

        [Fact]
        public async Task UpsertDepreciation_ReturnsOk_UpdatesExistingDepreciationWithOverlap_LinearPercent_PeriodEndOnly()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset
            {
                Id = Guid.NewGuid(),
                DepreciationAmountPercent = 20,
            };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
                Id = new Guid()
            };
            var fakeDepreciationsFound = new List<Depreciation> { fakeDepreciation };

            var fakeDepreciationToUpdate = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-15"),
                PeriodEnd = DateTime.Parse("2023-02-10")
            };

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeFixedAsset);

            _mockDepreciationRepo
                .Setup(r => r.GetDepreciationByFAandPeriodAsync(It.IsAny<Guid>(), false, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(fakeDepreciationsFound);


            // Act
            var result = await _depreciationsController.UpsertDepreciation(fakeDepreciationToUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(fakeDepreciationToUpdate.Amount, ((Depreciation)okResult.Value).Amount);
            Assert.Equal(fakeDepreciation.PeriodStart, ((Depreciation)okResult.Value).PeriodStart);
            Assert.Equal(fakeDepreciationToUpdate.PeriodEnd, ((Depreciation)okResult.Value).PeriodEnd);
            Assert.Equal(fakeDepreciation.Id, ((Depreciation)okResult.Value).Id);
        }


        [Fact]
        public async Task UpsertDepreciation_ReturnsOk_UpdatesExistingDepreciationWithOverlap_LinearYears_PeriodEndOnly()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset
            {
                Id = Guid.NewGuid(),
                DepreciationMaxYears = 10
            };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
                Id = new Guid()
            };
            var fakeDepreciationsFound = new List<Depreciation> { fakeDepreciation };

            var fakeDepreciationToUpdate = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-15"),
                PeriodEnd = DateTime.Parse("2023-02-10")
            };

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeFixedAsset);

            _mockDepreciationRepo
                .Setup(r => r.GetDepreciationByFAandPeriodAsync(It.IsAny<Guid>(), false, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(fakeDepreciationsFound);


            // Act
            var result = await _depreciationsController.UpsertDepreciation(fakeDepreciationToUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(fakeDepreciationToUpdate.Amount, ((Depreciation)okResult.Value).Amount);
            Assert.Equal(fakeDepreciation.PeriodStart, ((Depreciation)okResult.Value).PeriodStart);
            Assert.Equal(fakeDepreciationToUpdate.PeriodEnd, ((Depreciation)okResult.Value).PeriodEnd);
            Assert.Equal(fakeDepreciation.Id, ((Depreciation)okResult.Value).Id);
        }

        [Fact]
        public async Task UpsertDepreciation_ReturnsOk_UpdatesExistingDepreciationWithOverlap_LinearPercent_FullOverlap()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset
            {
                Id = Guid.NewGuid(),
                DepreciationAmountPercent = 20,
            };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
                Id = new Guid()
            };
            var fakeDepreciationsFound = new List<Depreciation> { fakeDepreciation };

            var fakeDepreciationToUpdate = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2022-12-01"),
                PeriodEnd = DateTime.Parse("2023-02-25")
            };

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeFixedAsset);

            _mockDepreciationRepo
                .Setup(r => r.GetDepreciationByFAandPeriodAsync(It.IsAny<Guid>(), false, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(fakeDepreciationsFound);


            // Act
            var result = await _depreciationsController.UpsertDepreciation(fakeDepreciationToUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(fakeDepreciationToUpdate.Amount, ((Depreciation)okResult.Value).Amount);
            Assert.Equal(fakeDepreciationToUpdate.PeriodStart, ((Depreciation)okResult.Value).PeriodStart);
            Assert.Equal(fakeDepreciationToUpdate.PeriodEnd, ((Depreciation)okResult.Value).PeriodEnd);
            Assert.Equal(fakeDepreciation.Id, ((Depreciation)okResult.Value).Id);
        }

        [Fact]
        public async Task UpsertDepreciation_ReturnsOk_UpdatesExistingDepreciationWithOverlap_LinearYears_FullOverlap()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset
            {
                Id = Guid.NewGuid(),
                DepreciationMaxYears = 10
            };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
                Id = new Guid()
            };
            var fakeDepreciationsFound = new List<Depreciation> { fakeDepreciation };

            var fakeDepreciationToUpdate = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2022-12-01"),
                PeriodEnd = DateTime.Parse("2023-02-25")
            };

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeFixedAsset);

            _mockDepreciationRepo
                .Setup(r => r.GetDepreciationByFAandPeriodAsync(It.IsAny<Guid>(), false, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(fakeDepreciationsFound);


            // Act
            var result = await _depreciationsController.UpsertDepreciation(fakeDepreciationToUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(fakeDepreciationToUpdate.Amount, ((Depreciation)okResult.Value).Amount);
            Assert.Equal(fakeDepreciationToUpdate.PeriodStart, ((Depreciation)okResult.Value).PeriodStart);
            Assert.Equal(fakeDepreciationToUpdate.PeriodEnd, ((Depreciation)okResult.Value).PeriodEnd);
            Assert.Equal(fakeDepreciation.Id, ((Depreciation)okResult.Value).Id);
        }

        [Fact]
        public async Task UpsertDepreciation_ReturnsOk_UpdatesExistingDepreciationWithOverlap_PercentAndYears_FullOverlap()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset
            {
                Id = Guid.NewGuid(),
                DepreciationAmountPercent = 20,
                DepreciationMaxYears = 10,
                ActivationAmount = 500,
                ActivationDate = DateTime.Parse("2021-04-20")
            };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
                Id = new Guid()
            };
            var fakeDepreciationsFound = new List<Depreciation> { fakeDepreciation };

            var fakeDepreciationToUpdate = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2022-12-01"),
                PeriodEnd = DateTime.Parse("2023-02-25")
            };

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeFixedAsset);

            _mockDepreciationRepo
                .Setup(r => r.GetDepreciationByFAandPeriodAsync(It.IsAny<Guid>(), false, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(fakeDepreciationsFound);


            // Act
            var result = await _depreciationsController.UpsertDepreciation(fakeDepreciationToUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(fakeDepreciationToUpdate.PeriodStart, ((Depreciation)okResult.Value).PeriodStart);
            Assert.Equal(fakeDepreciationToUpdate.PeriodEnd, ((Depreciation)okResult.Value).PeriodEnd);
            Assert.Equal(fakeDepreciation.Id, ((Depreciation)okResult.Value).Id);
            Assert.NotEqual(fakeDepreciation.Amount, ((Depreciation)okResult.Value).Amount);
        }

        [Fact]
        public async Task UpsertDepreciation_ReturnsOk_CreatesNewDepreciation()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset
            {
                Id = Guid.NewGuid(),
                DepreciationAmountPercent = 20,
                DepreciationMaxYears = 10,
                ActivationAmount = 500,
                ActivationDate = DateTime.Parse("2021-04-20")
            };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
                Id = new Guid()
            };

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeFixedAsset);

            _mockDepreciationRepo
                .Setup(r => r.GetDepreciationByFAandPeriodAsync(It.IsAny<Guid>(), false, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(new List<Depreciation>());


            // Act
            var result = await _depreciationsController.UpsertDepreciation(fakeDepreciation);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(((Depreciation)okResult.Value).Id);
        }

        [Fact]
        public async Task UpsertDepreciation_ReturnsInternalServerError_WhenPeriodStartsBeforeFixedAssetActivationDate()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset
            {
                Id = Guid.NewGuid(),
                DepreciationAmountPercent = 20,
                ActivationDate = DateTime.Parse("2023-01-15")
            };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
                Id = new Guid()
            };

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeFixedAsset);

            // Act
            async Task act() => await _depreciationsController.UpsertDepreciation(fakeDepreciation);

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        [Fact]
        public async Task UpsertDepreciation_ReturnsInternalServerError_WhenNoDepreciationSettingsFound()
        {
            // Arrange
            var fakeFixedAsset = new FixedAsset
            {
                Id = Guid.NewGuid(),
                ActivationDate = DateTime.Parse("2023-01-15"),
                DepreciationConfigId = new Guid(),
                DepreciationAmountPercent = 0,
                DepreciationMaxYears = 0
            };
            var fakeDepreciation = new Depreciation
            {
                FixedAssetId = fakeFixedAsset.Id,
                PeriodStart = DateTime.Parse("2023-01-01"),
                PeriodEnd = DateTime.Parse("2023-01-31"),
                Amount = 0,
                Id = new Guid()
            };

            _mockDepreciationValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Depreciation>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockFixedAssetRepo
                .Setup(r => r.GetFixedAssetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeFixedAsset);

            // Act
            async Task act() => await _depreciationsController.UpsertDepreciation(fakeDepreciation);

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
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





        #endregion
    }
}
