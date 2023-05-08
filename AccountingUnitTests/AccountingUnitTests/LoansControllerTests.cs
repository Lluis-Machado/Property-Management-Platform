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
    public class LoansControllerTests
    {
        private readonly Mock<ILogger<LoansController>> _mockLogger;
        private readonly Mock<IValidator<Loan>> _mockLoanValidator;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILoanRepository> _mockLoanRepo;
        private readonly Mock<IBusinessPartnerRepository> _mockBusinessPartnerRepo;
        private readonly LoansController _loansController;

        public LoansControllerTests()
        {
            _mockLogger = new Mock<ILogger<LoansController>>();
            _mockLoanValidator = new Mock<IValidator<Loan>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLoanRepo = new Mock<ILoanRepository>();
            _mockBusinessPartnerRepo = new Mock<IBusinessPartnerRepository>();
            _loansController = new LoansController(_mockLoanRepo.Object, _mockBusinessPartnerRepo.Object, _mockLoanValidator.Object, _mockLogger.Object);
        }

        #region Create

        [Fact]
        public async Task CreateAsync_ReturnCreatedResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner
            {
                AccountID = "FakeAccountId",
                Name = "FakeName",
                Type = "FakeType",
                VATNumber = "FakeVATNumber",
                Id = Guid.NewGuid(),
            };
            var fakeLoan = new Loan
            {
                BusinessPartnerId = fakeBusinessPartner.Id,
                Concept = "FakeConcept",
                Amount = 0,
                AmountPaid = 0,
            };
            var fakeExpectedLoan = new Loan
            {
                BusinessPartnerId = fakeBusinessPartner.Id,
                Concept = "FakeConcept",
                Amount = 0,
                AmountPaid = 0,
                Id = Guid.NewGuid(),
                Deleted = false
            };

            _mockLoanValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Loan>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockBusinessPartnerRepo
                .Setup(v => v.GetBusinessPartnerByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeBusinessPartner);

            _mockLoanRepo
                .Setup(r => r.InsertLoanAsync(It.IsAny<Loan>()))
                .ReturnsAsync(fakeExpectedLoan);

            // Act
            var result = await _loansController.CreateAsync(fakeLoan, fakeLoan.BusinessPartnerId);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(fakeExpectedLoan, createdResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenIdFieldIsNotEmpty()
        {
            // Arrange
            var fakeLoan = new Loan
            {
                BusinessPartnerId = Guid.NewGuid(),
                Concept = "FakeConcept",
                Amount = 0,
                AmountPaid = 0,
                Id = Guid.NewGuid()
            };

            // Act
            var result = await _loansController.CreateAsync(fakeLoan, fakeLoan.BusinessPartnerId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Loan Id field must be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenValidationIsNotValid()
        {
            // Arrange
            var fakeLoan = new Loan
            {
                BusinessPartnerId = Guid.NewGuid(),
                Concept = "FakeConcept",
                Amount = 0,
                AmountPaid = 0,
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockLoanValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Loan>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _loansController.CreateAsync(fakeLoan, fakeLoan.BusinessPartnerId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsNotFoundResult_WhenBusinessPartnerNotFound()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner
            {
                AccountID = "FakeAccountId",
                Name = "FakeName",
                Type = "FakeType",
                VATNumber = "FakeVATNumber",
                Id = Guid.NewGuid(),
            };
            var fakeLoan = new Loan
            {
                BusinessPartnerId = fakeBusinessPartner.Id,
                Concept = "FakeConcept",
                Amount = 0,
                AmountPaid = 0,
            };

            fakeBusinessPartner = null;

            _mockLoanValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Loan>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockBusinessPartnerRepo
                .Setup(v => v.GetBusinessPartnerByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeBusinessPartner);

            // Act
            var result = await _loansController.CreateAsync(fakeLoan, fakeLoan.BusinessPartnerId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("BusinessPartner not found", notFoundResult.Value);
        }

        #endregion

        #region Get

        [Fact]
        public async Task GetAsync_ReturnsOkResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeExpectedLoans = new List<Loan>()
            {
                new Loan
                {
                    BusinessPartnerId = Guid.NewGuid(),
                    Concept = "FakeConcept",
                    Amount = 0,
                    AmountPaid = 0,
                    Id = Guid.NewGuid(),
                    Deleted = false
                },
                new Loan
                {
                    BusinessPartnerId = Guid.NewGuid(),
                    Concept = "FakeConcept",
                    Amount = 0,
                    AmountPaid = 0,
                    Id = Guid.NewGuid(),
                    Deleted = false
                }
            };

            _mockLoanRepo
                .Setup(r => r.GetLoansAsync())
                .ReturnsAsync(fakeExpectedLoans);

            // Act
            var result = await _loansController.GetAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualLoans = Assert.IsAssignableFrom<IEnumerable<Loan>>(okResult.Value);
            Assert.Equal(fakeExpectedLoans, actualLoans);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockLoanRepo
                .Setup(repo => repo.GetLoansAsync())
                .ThrowsAsync(new Exception());

            // Act
            async Task act() => await _loansController.GetAsync();

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
                AccountID = "FakeAccountId",
                Name = "FakeName",
                Type = "FakeType",
                VATNumber = "FakeVATNumber",
                Id = Guid.NewGuid(),
            };
            var fakeLoan = new Loan
            {
                BusinessPartnerId = Guid.NewGuid(),
                Concept = "FakeConcept",
                Amount = 0,
                AmountPaid = 0,
                Id = Guid.NewGuid()
            };

            _mockLoanValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Loan>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockBusinessPartnerRepo
                .Setup(v => v.GetBusinessPartnerByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeBusinessPartner);

            _mockLoanRepo
                .Setup(r => r.UpdateLoanAsync(fakeLoan))
                .ReturnsAsync(1);

            // Act
            var result = await _loansController.UpdateAsync(fakeLoan, fakeLoan.BusinessPartnerId, fakeLoan.Id);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenLoanIdDoesNotMatch()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner
            {
                AccountID = "FakeAccountId",
                Name = "FakeName",
                Type = "FakeType",
                VATNumber = "FakeVATNumber",
                Id = Guid.NewGuid(),
            };
            var fakeLoan = new Loan
            {
                BusinessPartnerId = fakeBusinessPartner.Id,
                Concept = "FakeConcept",
                Amount = 0,
                AmountPaid = 0,
                Id = Guid.NewGuid()
            };

            // Act
            var result = await _loansController.UpdateAsync(fakeLoan, fakeLoan.BusinessPartnerId, Guid.NewGuid());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Loan Id from body incorrect", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenLoanValidationNotValid()
        {
            // Arrange
            var fakeLoan = new Loan
            {
                BusinessPartnerId = Guid.NewGuid(),
                Concept = "FakeConcept",
                Amount = 0,
                AmountPaid = 0,
                Id = Guid.NewGuid()
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockLoanValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Loan>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _loansController.UpdateAsync(fakeLoan, fakeLoan.BusinessPartnerId, fakeLoan.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenLoanNotFound()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner
            {
                AccountID = "FakeAccountId",
                Name = "FakeName",
                Type = "FakeType",
                VATNumber = "FakeVATNumber",
                Id = Guid.NewGuid(),
            };

            var fakeLoan = new Loan
            {
                BusinessPartnerId = Guid.NewGuid(),
                Concept = "FakeConcept",
                Amount = 0,
                AmountPaid = 0,
                Id = Guid.NewGuid()
            };

            _mockLoanValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Loan>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockBusinessPartnerRepo
                .Setup(v => v.GetBusinessPartnerByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeBusinessPartner);

            _mockLoanRepo
                .Setup(r => r.UpdateLoanAsync(It.IsAny<Loan>()))
                .ReturnsAsync(0);

            // Act
            var result = await _loansController.UpdateAsync(fakeLoan, fakeLoan.BusinessPartnerId, fakeLoan.Id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Loan not found", notFoundResult.Value);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeLoanId = Guid.NewGuid();

            _mockLoanRepo
                .Setup(r => r.SetDeleteLoanAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(1);

            // Act
            var result = await _loansController.DeleteAsync(fakeLoanId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenLoanNotFound()
        {
            // Arrange
            var fakeLoanId = Guid.NewGuid();

            _mockLoanRepo
                .Setup(r => r.SetDeleteLoanAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(0);

            // Act
            var result = await _loansController.DeleteAsync(fakeLoanId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Loan not found", notFoundResult.Value);
        }

        #endregion

        #region Undelete

        [Fact]
        public async Task UndeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeLoanId = Guid.NewGuid();

            _mockLoanRepo
                .Setup(r => r.SetDeleteLoanAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(1);

            // Act
            var result = await _loansController.UndeleteAsync(fakeLoanId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UndeleteAsync_ReturnsNotFound_WhenLoanNotFound()
        {
            // Arrange
            var fakeLoanId = Guid.NewGuid();

            _mockLoanRepo
                .Setup(r => r.SetDeleteLoanAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(0);

            // Act
            var result = await _loansController.UndeleteAsync(fakeLoanId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Loan not found", notFoundResult.Value);
        }

        #endregion
    }
}
