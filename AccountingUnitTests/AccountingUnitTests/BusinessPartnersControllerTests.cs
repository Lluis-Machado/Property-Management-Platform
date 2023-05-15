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
    public class BusinessPartnersControllerTests
    {
        private readonly Mock<ILogger<BusinessPartnersController>> _mockLogger;
        private readonly Mock<IValidator<BusinessPartner>> _mockBusinessPartnerValidator;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IBusinessPartnerRepository> _mockBusinessPartnerRepo;
        private readonly BusinessPartnersController _businessPartnersController;

        public BusinessPartnersControllerTests()
        {
            _mockLogger = new Mock<ILogger<BusinessPartnersController>>();
            _mockBusinessPartnerValidator = new Mock<IValidator<BusinessPartner>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockBusinessPartnerRepo = new Mock<IBusinessPartnerRepository>();
            _businessPartnersController = new BusinessPartnersController(_mockBusinessPartnerRepo.Object, _mockBusinessPartnerValidator.Object, _mockLogger.Object);
        }

        #region Create

        [Fact]
        public async Task CreateAsync_ReturnCreatedResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner
            {
                Name = "fakeName",
                VATNumber = "fakeVATNumber",
                AccountID = "fakeAccountId",
                Type = "fakeType",
            };
            var fakeExpectedBusinessPartner = new BusinessPartner
            {
                Name = "fakeName",
                VATNumber = "fakeVATNumber",
                AccountID = "fakeAccountId",
                Type = "fakeType",
                Id = Guid.NewGuid(),
                Deleted = false
            };

            _mockBusinessPartnerValidator
                .Setup(v => v.ValidateAsync(It.IsAny<BusinessPartner>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockBusinessPartnerRepo
                .Setup(r => r.InsertBusinessPartnerAsync(It.IsAny<BusinessPartner>()))
                .ReturnsAsync(fakeExpectedBusinessPartner);

            // Act
            var result = await _businessPartnersController.CreateAsync(fakeBusinessPartner);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(fakeExpectedBusinessPartner, createdResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenIdFieldIsNotEmpty()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner
            {
                Name = "fakeName",
                VATNumber = "fakeVATNumber",
                AccountID = "fakeAccountId",
                Type = "fakeType",
                Id = Guid.NewGuid()
            };

            // Act
            var result = await _businessPartnersController.CreateAsync(fakeBusinessPartner);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("BusinessPartner Id field must be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenValidationIsNotValid()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner
            {
                Name = "fakeName",
                VATNumber = "fakeVATNumber",
                AccountID = "fakeAccountId",
                Type = "fakeType",
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockBusinessPartnerValidator
                .Setup(v => v.ValidateAsync(It.IsAny<BusinessPartner>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _businessPartnersController.CreateAsync(fakeBusinessPartner);

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
            var fakeExpectedBusinessPartners = new List<BusinessPartner>()
            {
                new BusinessPartner
                {
                    Name = "fakeName",
                    VATNumber = "fakeVATNumber",
                    AccountID = "fakeAccountId",
                    Type = "fakeType",
                    Id = Guid.NewGuid(),
                    Deleted = false
                },
                new BusinessPartner
                {
                    Name = "fakeName",
                    VATNumber = "fakeVATNumber",
                    AccountID = "fakeAccountId",
                    Type = "fakeType",
                    Id = Guid.NewGuid(),
                    Deleted = false
                }
            };

            _mockBusinessPartnerRepo
                .Setup(r => r.GetBusinessPartnersAsync(false))
                .ReturnsAsync(fakeExpectedBusinessPartners);

            // Act
            var result = await _businessPartnersController.GetAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualBusinessPartners = Assert.IsAssignableFrom<IEnumerable<BusinessPartner>>(okResult.Value);
            Assert.Equal(fakeExpectedBusinessPartners, actualBusinessPartners);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockBusinessPartnerRepo
                .Setup(repo => repo.GetBusinessPartnersAsync(false))
                .ThrowsAsync(new Exception());

            // Act
            async Task act() => await _businessPartnersController.GetAsync();

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner
            {
                Name = "fakeName",
                VATNumber = "fakeVATNumber",
                AccountID = "fakeAccountId",
                Type = "fakeType",
                Id = Guid.NewGuid()
            };

            _mockBusinessPartnerValidator
                .Setup(v => v.ValidateAsync(It.IsAny<BusinessPartner>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockBusinessPartnerRepo
                .Setup(r => r.UpdateBusinessPartnerAsync(fakeBusinessPartner))
                .ReturnsAsync(1);

            // Act
            var result = await _businessPartnersController.UpdateAsync(fakeBusinessPartner, fakeBusinessPartner.Id);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenBusinessPartnerIdDoesNotMatch()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner
            {
                Name = "fakeName",
                VATNumber = "fakeVATNumber",
                AccountID = "fakeAccountId",
                Type = "fakeType",
                Id = Guid.NewGuid()
            }; ;

            // Act
            var result = await _businessPartnersController.UpdateAsync(fakeBusinessPartner, Guid.NewGuid());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("BusinessPartner Id from body incorrect", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenBusinessPartnerValidationNotValid()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner
            {
                Name = "fakeName",
                VATNumber = "fakeVATNumber",
                AccountID = "fakeAccountId",
                Type = "fakeType",
                Id = Guid.NewGuid()
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });
            
            _mockBusinessPartnerValidator
                .Setup(v => v.ValidateAsync(It.IsAny<BusinessPartner>(), CancellationToken.None))             
                .ReturnsAsync(validationResult);

            // Act
            var result = await _businessPartnersController.UpdateAsync(fakeBusinessPartner, fakeBusinessPartner.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenBusinessPartnerNotFound()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner
            {
                Name = "fakeName",
                VATNumber = "fakeVATNumber",
                AccountID = "fakeAccountId",
                Type = "fakeType",
                Id = Guid.NewGuid()
            };

            _mockBusinessPartnerValidator
                .Setup(v => v.ValidateAsync(It.IsAny<BusinessPartner>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockBusinessPartnerRepo
                .Setup(r => r.UpdateBusinessPartnerAsync(It.IsAny<BusinessPartner>()))
                .ReturnsAsync(0);

            // Act
            var result = await _businessPartnersController.UpdateAsync(fakeBusinessPartner, fakeBusinessPartner.Id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("BusinessPartner not found", notFoundResult.Value);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeBusinessPartnerId = Guid.NewGuid();

            _mockBusinessPartnerRepo
                .Setup(r => r.SetDeleteBusinessPartnerAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(1);

            // Act
            var result = await _businessPartnersController.DeleteAsync(fakeBusinessPartnerId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenBusinessPartnerNotFound()
        {
            // Arrange
            var fakeBusinessPartnerId = Guid.NewGuid();

            _mockBusinessPartnerRepo
                .Setup(r => r.SetDeleteBusinessPartnerAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(0);

            // Act
            var result = await _businessPartnersController.DeleteAsync(fakeBusinessPartnerId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("BusinessPartner not found", notFoundResult.Value);
        }

        #endregion

        #region Undelete

        [Fact]
        public async Task UndeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeBusinessPartnerId = Guid.NewGuid();

            _mockBusinessPartnerRepo
                .Setup(r => r.SetDeleteBusinessPartnerAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(1);

            // Act
            var result = await _businessPartnersController.UndeleteAsync(fakeBusinessPartnerId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UndeleteAsync_ReturnsNotFound_WhenBusinessPartnerNotFound()
        {
            // Arrange
            var fakeBusinessPartnerId = Guid.NewGuid();

            _mockBusinessPartnerRepo
                .Setup(r => r.SetDeleteBusinessPartnerAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(0);

            // Act
            var result = await _businessPartnersController.UndeleteAsync(fakeBusinessPartnerId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("BusinessPartner not found", notFoundResult.Value);
        }

        #endregion
    }
}
