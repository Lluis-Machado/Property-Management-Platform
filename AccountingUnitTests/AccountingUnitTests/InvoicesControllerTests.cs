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
    public class InvoicesControllerTests
    {

        private readonly Mock<ILogger<InvoicesController>> _mockLogger;
        private readonly Mock<IValidator<Invoice>> _mockInvoiceValidator;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IInvoiceRepository> _mockInvoiceRepo;
        private readonly Mock<IBusinessPartnerRepository> _mockBusinessPartnerRepo;
        private readonly InvoicesController _invoicesController;

        public InvoicesControllerTests()
        {
            _mockLogger = new Mock<ILogger<InvoicesController>>();
            _mockInvoiceValidator = new Mock<IValidator<Invoice>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockInvoiceRepo = new Mock<IInvoiceRepository>();
            _mockBusinessPartnerRepo = new Mock<IBusinessPartnerRepository>();
            _invoicesController = new InvoicesController(_mockInvoiceRepo.Object, _mockBusinessPartnerRepo.Object, _mockInvoiceValidator.Object, _mockLogger.Object);
        }

        #region Create

        [Fact]
        public async Task CreateAsync_ReturnCreatedResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner { Id = Guid.NewGuid() };
            var fakeInvoice = new Invoice
            {
                BusinessPartnerId = fakeBusinessPartner.Id,
                RefNumber = "FakeRefNumber",
                Date = DateTime.Now,
                Currency = "FakeCurrency",
                GrossAmount = 0,
                NetAmount = 0,
                InvoiceLines = Array.Empty<InvoiceLine>(),
            };
            var fakeExpectedInvoice = new Invoice
            {
                BusinessPartnerId = fakeBusinessPartner.Id,
                RefNumber = "FakeRefNumber",
                Date = DateTime.Now,
                Currency = "FakeCurrency",
                GrossAmount = 0,
                NetAmount = 0,
                InvoiceLines = Array.Empty<InvoiceLine>(),
                Id = Guid.NewGuid(),
                Deleted = false
            };

            _mockInvoiceValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Invoice>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockBusinessPartnerRepo
                .Setup(v => v.GetBusinessPartnerByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeBusinessPartner);

            _mockInvoiceRepo
                .Setup(r => r.InsertInvoiceAsync(It.IsAny<Invoice>()))
                .ReturnsAsync(fakeExpectedInvoice);

            // Act
            var result = await _invoicesController.CreateAsync(fakeInvoice, fakeBusinessPartner.Id);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(fakeExpectedInvoice, createdResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenIdFieldIsNotEmpty()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner { Id = Guid.NewGuid() };
            var fakeInvoice = new Invoice
            {
                BusinessPartnerId = fakeBusinessPartner.Id,
                RefNumber = "FakeRefNumber",
                Date = DateTime.Now,
                Currency = "FakeCurrency",
                GrossAmount = 0,
                NetAmount = 0,
                InvoiceLines = Array.Empty<InvoiceLine>(),
                Id = Guid.NewGuid()
            };

            // Act
            var result = await _invoicesController.CreateAsync(fakeInvoice, fakeBusinessPartner.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invoice Id field must be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenValidationIsNotValid()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner { Id = Guid.NewGuid() };
            var fakeInvoice = new Invoice
            {
                BusinessPartnerId = fakeBusinessPartner.Id,
                RefNumber = "FakeRefNumber",
                Date = DateTime.Now,
                Currency = "FakeCurrency",
                GrossAmount = 0,
                NetAmount = 0,
                InvoiceLines = Array.Empty<InvoiceLine>(),
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockInvoiceValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Invoice>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _invoicesController.CreateAsync(fakeInvoice, fakeBusinessPartner.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsNotFoundResult_WhenBusinessPartnerNotFound()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner { Id = Guid.NewGuid() };
            var fakeInvoice = new Invoice
            {
                BusinessPartnerId = fakeBusinessPartner.Id,
                RefNumber = "FakeRefNumber",
                Date = DateTime.Now,
                Currency = "FakeCurrency",
                GrossAmount = 0,
                NetAmount = 0,
                InvoiceLines = Array.Empty<InvoiceLine>(),
            };

            fakeBusinessPartner = null;

            _mockInvoiceValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Invoice>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockBusinessPartnerRepo
                .Setup(v => v.GetBusinessPartnerByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeBusinessPartner);

            // Act
            var result = await _invoicesController.CreateAsync(fakeInvoice, fakeInvoice.BusinessPartnerId);

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
            var fakeExpectedInvoices = new List<Invoice>()
            {
                new Invoice
                {

                    BusinessPartnerId = Guid.NewGuid(),
                    RefNumber = "FakeRefNumber",
                    Date = DateTime.Now,
                    Currency = "FakeCurrency",
                    GrossAmount = 0,
                    NetAmount = 0,
                    InvoiceLines = Array.Empty<InvoiceLine>(),
                    Id = Guid.NewGuid(),
                    Deleted = false
                },
                new Invoice
                {

                    BusinessPartnerId = Guid.NewGuid(),
                    RefNumber = "FakeRefNumber",
                    Date = DateTime.Now,
                    Currency = "FakeCurrency",
                    GrossAmount = 0,
                    NetAmount = 0,
                    InvoiceLines = Array.Empty<InvoiceLine>(),
                    Id = Guid.NewGuid(),
                    Deleted = false
                }
            };

            _mockInvoiceRepo
                .Setup(r => r.GetInvoicesAsync(false))
                .ReturnsAsync(fakeExpectedInvoices);

            // Act
            var result = await _invoicesController.GetAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualInvoices = Assert.IsAssignableFrom<IEnumerable<Invoice>>(okResult.Value);
            Assert.Equal(fakeExpectedInvoices, actualInvoices);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockInvoiceRepo
                .Setup(repo => repo.GetInvoicesAsync(false))
                .ThrowsAsync(new Exception());

            // Act
            async Task act() => await _invoicesController.GetAsync();

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner { Id = Guid.NewGuid() };
            var fakeInvoice = new Invoice
            {
                BusinessPartnerId = fakeBusinessPartner.Id,
                RefNumber = "FakeRefNumber",
                Date = DateTime.Now,
                Currency = "FakeCurrency",
                GrossAmount = 0,
                NetAmount = 0,
                InvoiceLines = Array.Empty<InvoiceLine>(),
                Id = Guid.NewGuid()
            };

            _mockInvoiceValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Invoice>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockBusinessPartnerRepo
                .Setup(v => v.GetBusinessPartnerByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeBusinessPartner);

            _mockInvoiceRepo
                .Setup(r => r.UpdateInvoiceAsync(fakeInvoice))
                .ReturnsAsync(1);

            // Act
            var result = await _invoicesController.UpdateAsync(fakeInvoice, fakeBusinessPartner.Id, fakeInvoice.Id);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenInvoiceIdDoesNotMatch()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner { Id = Guid.NewGuid() };
            var fakeInvoice = new Invoice
            {
                BusinessPartnerId = fakeBusinessPartner.Id,
                RefNumber = "FakeRefNumber",
                Date = DateTime.Now,
                Currency = "FakeCurrency",
                GrossAmount = 0,
                NetAmount = 0,
                InvoiceLines = Array.Empty<InvoiceLine>(),
                Id = Guid.NewGuid()
            }; ;

            // Act
            var result = await _invoicesController.UpdateAsync(fakeInvoice, fakeBusinessPartner.Id, Guid.NewGuid());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invoice Id from body incorrect", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenInvoiceValidationNotValid()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner { Id = Guid.NewGuid() };
            var fakeInvoice = new Invoice
            {
                BusinessPartnerId = fakeBusinessPartner.Id,
                RefNumber = "FakeRefNumber",
                Date = DateTime.Now,
                Currency = "FakeCurrency",
                GrossAmount = 0,
                NetAmount = 0,
                InvoiceLines = Array.Empty<InvoiceLine>(),
                Id = Guid.NewGuid()
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockInvoiceValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Invoice>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _invoicesController.UpdateAsync(fakeInvoice, fakeBusinessPartner.Id, fakeInvoice.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenInvoiceNotFound()
        {
            // Arrange
            var fakeBusinessPartner = new BusinessPartner { Id = Guid.NewGuid() };
            var fakeInvoice = new Invoice
            {
                BusinessPartnerId = fakeBusinessPartner.Id,
                RefNumber = "FakeRefNumber",
                Date = DateTime.Now,
                Currency = "FakeCurrency",
                GrossAmount = 0,
                NetAmount = 0,
                InvoiceLines = Array.Empty<InvoiceLine>(),
                Id = Guid.NewGuid()
            };

            _mockInvoiceValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Invoice>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockBusinessPartnerRepo
                .Setup(v => v.GetBusinessPartnerByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeBusinessPartner);

            _mockInvoiceRepo
                .Setup(r => r.UpdateInvoiceAsync(It.IsAny<Invoice>()))
                .ReturnsAsync(0);

            // Act
            var result = await _invoicesController.UpdateAsync(fakeInvoice, fakeBusinessPartner.Id, fakeInvoice.Id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Invoice not found", notFoundResult.Value);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeInvoiceId = Guid.NewGuid();

            _mockInvoiceRepo
                .Setup(r => r.SetDeleteInvoiceAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(1);

            // Act
            var result = await _invoicesController.DeleteAsync(fakeInvoiceId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenInvoiceNotFound()
        {
            // Arrange
            var fakeInvoiceId = Guid.NewGuid();

            _mockInvoiceRepo
                .Setup(r => r.SetDeleteInvoiceAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(0);

            // Act
            var result = await _invoicesController.DeleteAsync(fakeInvoiceId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Invoice not found", notFoundResult.Value);
        }

        #endregion

        #region Undelete

        [Fact]
        public async Task UndeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeInvoiceId = Guid.NewGuid();

            _mockInvoiceRepo
                .Setup(r => r.SetDeleteInvoiceAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(1);

            // Act
            var result = await _invoicesController.UndeleteAsync(fakeInvoiceId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UndeleteAsync_ReturnsNotFound_WhenInvoiceNotFound()
        {
            // Arrange
            var fakeInvoiceId = Guid.NewGuid();

            _mockInvoiceRepo
                .Setup(r => r.SetDeleteInvoiceAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(0);

            // Act
            var result = await _invoicesController.UndeleteAsync(fakeInvoiceId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Invoice not found", notFoundResult.Value);
        }

        #endregion
    }
}
