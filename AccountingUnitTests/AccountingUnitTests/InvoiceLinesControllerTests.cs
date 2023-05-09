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
    public class InvoiceLinesControllerTests
    {

        private readonly Mock<ILogger<InvoiceLinesController>> _mockLogger;
        private readonly Mock<IValidator<InvoiceLine>> _mockInvoiceLineValidator;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IInvoiceLineRepository> _mockInvoiceLineRepo;
        private readonly Mock<IInvoiceRepository> _mockInvoiceRepo;
        private readonly Mock<IExpenseTypeRepository> _mockExpenseTypeRepo;
        private readonly InvoiceLinesController _invoiceLinesController;
    
        public InvoiceLinesControllerTests() 
        { 
            _mockLogger = new Mock<ILogger<InvoiceLinesController>>(); 
            _mockInvoiceLineValidator = new Mock<IValidator<InvoiceLine>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockInvoiceLineRepo = new Mock<IInvoiceLineRepository>();
            _mockInvoiceRepo = new Mock<IInvoiceRepository>();
            _mockExpenseTypeRepo = new Mock<IExpenseTypeRepository>();
            _invoiceLinesController = new InvoiceLinesController(_mockInvoiceLineRepo.Object, _mockInvoiceRepo.Object, _mockExpenseTypeRepo.Object, _mockInvoiceLineValidator.Object, _mockLogger.Object);
        }

        #region Create

        [Fact]
        public async Task CreateAsync_ReturnCreatedResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeInvoice = new Invoice { Id = Guid.NewGuid()};
            var fakeExpenseType = new ExpenseType { Id = Guid.NewGuid()};
            var fakeInvoiceLine = new InvoiceLine
            {
                LineNumber = 0,
                ArticleRefNumber = "FakeArticleRefNumber",
                ArticleName = "FakeArticleName",
                Tax = 0,
                Quantity = 0,
                UnitPrice = 0,
                DateRefFrom = DateTime.Now,
                DateRefTo = DateTime.Now,
                ExpenseTypeId = fakeExpenseType.Id,
                InvoiceId = fakeInvoice.Id,
            };
            var fakeExpectedInvoiceLine = new InvoiceLine
            {
                LineNumber = 0,
                ArticleRefNumber = "FakeArticleRefNumber",
                ArticleName = "FakeArticleName",
                Tax = 0,
                Quantity = 0,
                UnitPrice = 0,
                DateRefFrom = DateTime.Now,
                DateRefTo = DateTime.Now,
                ExpenseTypeId = fakeExpenseType.Id,
                InvoiceId = fakeInvoice.Id,
                Id = Guid.NewGuid(),
                Deleted = false
            };

            _mockInvoiceLineValidator
                .Setup(v => v.ValidateAsync(It.IsAny<InvoiceLine>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockInvoiceRepo
                .Setup(v => v.GetInvoiceByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeInvoice);

            _mockExpenseTypeRepo
                .Setup(v => v.GetExpenseTypeByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeExpenseType);

            _mockInvoiceLineRepo
                .Setup(r => r.InsertInvoiceLineAsync(It.IsAny<InvoiceLine>()))
                .ReturnsAsync(fakeExpectedInvoiceLine);

            // Act
            var result = await _invoiceLinesController.CreateAsync(fakeInvoiceLine, fakeInvoice.Id, fakeExpenseType.Id);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(fakeExpectedInvoiceLine, createdResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenIdFieldIsNotEmpty()
        {
            // Arrange
            var fakeInvoice = new Invoice { Id = Guid.NewGuid() };
            var fakeExpenseType = new ExpenseType { Id = Guid.NewGuid() };
            var fakeInvoiceLine = new InvoiceLine
            {
                LineNumber = 0,
                ArticleRefNumber = "FakeArticleRefNumber",
                ArticleName = "FakeArticleName",
                Tax = 0,
                Quantity = 0,
                UnitPrice = 0,
                DateRefFrom = DateTime.Now,
                DateRefTo = DateTime.Now,
                ExpenseTypeId = fakeExpenseType.Id,
                InvoiceId = fakeInvoice.Id,
                Id = Guid.NewGuid(),
            };

            // Act
            var result = await _invoiceLinesController.CreateAsync(fakeInvoiceLine, fakeInvoice.Id, fakeExpenseType.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("InvoiceLine Id field must be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenValidationIsNotValid()
        {
            // Arrange
            var fakeInvoice = new Invoice { Id = Guid.NewGuid()};
            var fakeExpenseType = new ExpenseType { Id = Guid.NewGuid() };
            var fakeInvoiceLine = new InvoiceLine
            {
                LineNumber = 0,
                ArticleRefNumber = "FakeArticleRefNumber",
                ArticleName = "FakeArticleName",
                Tax = 0,
                Quantity = 0,
                UnitPrice = 0,
                DateRefFrom = DateTime.Now,
                DateRefTo = DateTime.Now,
                ExpenseTypeId = fakeExpenseType.Id,
                InvoiceId = fakeInvoice.Id,
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockInvoiceLineValidator
                .Setup(v => v.ValidateAsync(It.IsAny<InvoiceLine>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _invoiceLinesController.CreateAsync(fakeInvoiceLine, fakeInvoice.Id, fakeExpenseType.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsNotFoundResult_WhenInvoiceNotFound()
        {
            // Arrange
            var fakeInvoice = new Invoice { Id = Guid.NewGuid() };
            var fakeExpenseType = new ExpenseType { Id = Guid.NewGuid() };
            var fakeInvoiceLine = new InvoiceLine
            {
                LineNumber = 0,
                ArticleRefNumber = "FakeArticleRefNumber",
                ArticleName = "FakeArticleName",
                Tax = 0,
                Quantity = 0,
                UnitPrice = 0,
                DateRefFrom = DateTime.Now,
                DateRefTo = DateTime.Now,
                ExpenseTypeId = fakeExpenseType.Id,
                InvoiceId = fakeInvoice.Id,
            };

            fakeInvoice = null;

            _mockInvoiceLineValidator
                .Setup(v => v.ValidateAsync(It.IsAny<InvoiceLine>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockInvoiceRepo
                .Setup(v => v.GetInvoiceByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeInvoice);

            // Act
            var result = await _invoiceLinesController.CreateAsync(fakeInvoiceLine, fakeInvoiceLine.InvoiceId, fakeInvoiceLine.ExpenseTypeId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Invoice not found", notFoundResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsNotFoundResult_WhenExpenseTypeNotFound()
        {
            // Arrange
            var fakeInvoice = new Invoice { Id = Guid.NewGuid() };
            var fakeExpenseType = new ExpenseType { Id = Guid.NewGuid() };
            var fakeInvoiceLine = new InvoiceLine
            {
                LineNumber = 0,
                ArticleRefNumber = "FakeArticleRefNumber",
                ArticleName = "FakeArticleName",
                Tax = 0,
                Quantity = 0,
                UnitPrice = 0,
                DateRefFrom = DateTime.Now,
                DateRefTo = DateTime.Now,
                ExpenseTypeId = fakeExpenseType.Id,
                InvoiceId = fakeInvoice.Id,
            };

            fakeExpenseType = null;

            _mockInvoiceLineValidator
                .Setup(v => v.ValidateAsync(It.IsAny<InvoiceLine>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockInvoiceRepo
                .Setup(v => v.GetInvoiceByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeInvoice);

            _mockExpenseTypeRepo
                .Setup(v => v.GetExpenseTypeByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeExpenseType);

            // Act
            var result = await _invoiceLinesController.CreateAsync(fakeInvoiceLine, fakeInvoiceLine.InvoiceId, fakeInvoiceLine.ExpenseTypeId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("ExpenseType not found", notFoundResult.Value);
        }

        #endregion

        #region Get

        [Fact]
        public async Task GetAsync_ReturnsOkResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeExpectedInvoiceLines = new List<InvoiceLine>()
            {
                new InvoiceLine
                {
                    LineNumber = 0,
                    ArticleRefNumber = "FakeArticleRefNumber",
                    ArticleName = "FakeArticleName",
                    Tax = 0,
                    Quantity = 0,
                    UnitPrice = 0,
                    DateRefFrom = DateTime.Now,
                    DateRefTo = DateTime.Now,
                    ExpenseTypeId = Guid.NewGuid(),
                    InvoiceId = Guid.NewGuid(),
                    Id = Guid.NewGuid(),
                    Deleted = false
                },
                new InvoiceLine
                {
                    LineNumber = 0,
                    ArticleRefNumber = "FakeArticleRefNumber",
                    ArticleName = "FakeArticleName",
                    Tax = 0,
                    Quantity = 0,
                    UnitPrice = 0,
                    DateRefFrom = DateTime.Now,
                    DateRefTo = DateTime.Now,
                    ExpenseTypeId = Guid.NewGuid(),
                    InvoiceId = Guid.NewGuid(),
                    Id = Guid.NewGuid(),
                    Deleted = false
                }
            };

            _mockInvoiceLineRepo
                .Setup(r => r.GetInvoiceLinesAsync())
                .ReturnsAsync(fakeExpectedInvoiceLines);

            // Act
            var result = await _invoiceLinesController.GetAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualInvoiceLines = Assert.IsAssignableFrom<IEnumerable<InvoiceLine>>(okResult.Value);
            Assert.Equal(fakeExpectedInvoiceLines, actualInvoiceLines);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockInvoiceLineRepo
                .Setup(repo => repo.GetInvoiceLinesAsync())
                .ThrowsAsync(new Exception());

            // Act
            async Task act() => await _invoiceLinesController.GetAsync();

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeInvoice = new Invoice { Id = Guid.NewGuid() };
            var fakeExpenseType = new ExpenseType { Id = Guid.NewGuid() };
            var fakeInvoiceLine = new InvoiceLine
            {
                LineNumber = 0,
                ArticleRefNumber = "FakeArticleRefNumber",
                ArticleName = "FakeArticleName",
                Tax = 0,
                Quantity = 0,
                UnitPrice = 0,
                DateRefFrom = DateTime.Now,
                DateRefTo = DateTime.Now,
                ExpenseTypeId = Guid.NewGuid(),
                InvoiceId = Guid.NewGuid(),
                Id = Guid.NewGuid()
            };

            _mockInvoiceLineValidator
                .Setup(v => v.ValidateAsync(It.IsAny<InvoiceLine>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockInvoiceRepo
                .Setup(v => v.GetInvoiceByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeInvoice);

            _mockExpenseTypeRepo
                .Setup(v => v.GetExpenseTypeByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeExpenseType);

            _mockInvoiceLineRepo
                .Setup(r => r.UpdateInvoiceLineAsync(fakeInvoiceLine))
                .ReturnsAsync(1);

            // Act
            var result = await _invoiceLinesController.UpdateAsync(fakeInvoiceLine, fakeInvoice.Id, fakeExpenseType.Id, fakeInvoiceLine.Id);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenInvoiceLineIdDoesNotMatch()
        {
            // Arrange
            var fakeInvoice = new Invoice { Id = Guid.NewGuid() };
            var fakeExpenseType = new ExpenseType { Id = Guid.NewGuid() };
            var fakeInvoiceLine = new InvoiceLine
            {
                LineNumber = 0,
                ArticleRefNumber = "FakeArticleRefNumber",
                ArticleName = "FakeArticleName",
                Tax = 0,
                Quantity = 0,
                UnitPrice = 0,
                DateRefFrom = DateTime.Now,
                DateRefTo = DateTime.Now,
                ExpenseTypeId = Guid.NewGuid(),
                InvoiceId = Guid.NewGuid(),
                Id = Guid.NewGuid()
            }; ;

            // Act
            var result = await _invoiceLinesController.UpdateAsync(fakeInvoiceLine, fakeInvoice.Id, fakeExpenseType.Id, Guid.NewGuid());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("InvoiceLine Id from body incorrect", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenInvoiceLineValidationNotValid()
        {
            // Arrange
            var fakeInvoice = new Invoice { Id = Guid.NewGuid() };
            var fakeExpenseType = new ExpenseType { Id = Guid.NewGuid() };
            var fakeInvoiceLine = new InvoiceLine
            {
                LineNumber = 0,
                ArticleRefNumber = "FakeArticleRefNumber",
                ArticleName = "FakeArticleName",
                Tax = 0,
                Quantity = 0,
                UnitPrice = 0,
                DateRefFrom = DateTime.Now,
                DateRefTo = DateTime.Now,
                ExpenseTypeId = Guid.NewGuid(),
                InvoiceId = Guid.NewGuid(),
                Id = Guid.NewGuid()
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockInvoiceLineValidator
                .Setup(v => v.ValidateAsync(It.IsAny<InvoiceLine>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _invoiceLinesController.UpdateAsync(fakeInvoiceLine, fakeInvoice.Id, fakeExpenseType.Id, fakeInvoiceLine.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenInvoiceLineNotFound()
        {
            // Arrange
            var fakeInvoice = new Invoice { Id = Guid.NewGuid() };
            var fakeExpenseType = new ExpenseType { Id = Guid.NewGuid() };
            var fakeInvoiceLine = new InvoiceLine
            {
                LineNumber = 0,
                ArticleRefNumber = "FakeArticleRefNumber",
                ArticleName = "FakeArticleName",
                Tax = 0,
                Quantity = 0,
                UnitPrice = 0,
                DateRefFrom = DateTime.Now,
                DateRefTo = DateTime.Now,
                ExpenseTypeId = Guid.NewGuid(),
                InvoiceId = Guid.NewGuid(),
                Id = Guid.NewGuid()
            };

            _mockInvoiceLineValidator
                .Setup(v => v.ValidateAsync(It.IsAny<InvoiceLine>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockInvoiceRepo
                .Setup(v => v.GetInvoiceByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeInvoice);

            _mockExpenseTypeRepo
                .Setup(v => v.GetExpenseTypeByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeExpenseType);

            _mockInvoiceLineRepo
                .Setup(r => r.UpdateInvoiceLineAsync(It.IsAny<InvoiceLine>()))
                .ReturnsAsync(0);

            // Act
            var result = await _invoiceLinesController.UpdateAsync(fakeInvoiceLine, fakeInvoice.Id, fakeExpenseType.Id, fakeInvoiceLine.Id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("InvoiceLine not found", notFoundResult.Value);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeInvoiceLineId = Guid.NewGuid();

            _mockInvoiceLineRepo
                .Setup(r => r.SetDeleteInvoiceLineAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(1);

            // Act
            var result = await _invoiceLinesController.DeleteAsync(fakeInvoiceLineId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenInvoiceLineNotFound()
        {
            // Arrange
            var fakeInvoiceLineId = Guid.NewGuid();

            _mockInvoiceLineRepo
                .Setup(r => r.SetDeleteInvoiceLineAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(0);

            // Act
            var result = await _invoiceLinesController.DeleteAsync(fakeInvoiceLineId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("InvoiceLine not found", notFoundResult.Value);
        }

        #endregion

        #region Undelete

        [Fact]
        public async Task UndeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeInvoiceLineId = Guid.NewGuid();

            _mockInvoiceLineRepo
                .Setup(r => r.SetDeleteInvoiceLineAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(1);

            // Act
            var result = await _invoiceLinesController.UndeleteAsync(fakeInvoiceLineId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UndeleteAsync_ReturnsNotFound_WhenInvoiceLineNotFound()
        {
            // Arrange
            var fakeInvoiceLineId = Guid.NewGuid();

            _mockInvoiceLineRepo
                .Setup(r => r.SetDeleteInvoiceLineAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(0);

            // Act
            var result = await _invoiceLinesController.UndeleteAsync(fakeInvoiceLineId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("InvoiceLine not found", notFoundResult.Value);
        }

        #endregion
    }
}
