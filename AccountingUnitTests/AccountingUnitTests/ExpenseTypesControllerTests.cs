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
    public class ExpenseTypesControllerTests
    {

        private readonly Mock<ILogger<ExpenseCategoriesController>> _mockLogger;
        private readonly Mock<IValidator<ExpenseCategory>> _mockExpenseTypeValidator;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IExpenseCategoryRepository> _mockExpenseTypeRepo;
        private readonly ExpenseCategoriesController _expenseTypesController;

        public ExpenseTypesControllerTests()
        {
            _mockLogger = new Mock<ILogger<ExpenseCategoriesController>>();
            _mockExpenseTypeValidator = new Mock<IValidator<ExpenseCategory>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockExpenseTypeRepo = new Mock<IExpenseCategoryRepository>();
            _expenseTypesController = new ExpenseCategoriesController(_mockExpenseTypeRepo.Object, _mockExpenseTypeValidator.Object, _mockLogger.Object);
        }

        #region Create

        [Fact]
        public async Task CreateAsync_ReturnCreatedResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeExpenseType = new ExpenseCategory
            {
                Code = 0,
                Description = "FakeDescription",
            };
            var fakeExpectedExpenseType = new ExpenseCategory
            {
                Code = 0,
                Description = "FakeDescription",
                Id = Guid.NewGuid(),
                Deleted = false
            };

            _mockExpenseTypeValidator
                .Setup(v => v.ValidateAsync(It.IsAny<ExpenseCategory>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockExpenseTypeRepo
                .Setup(r => r.InsertExpenseTypeAsync(It.IsAny<ExpenseCategory>()))
                .ReturnsAsync(fakeExpectedExpenseType);

            // Act
            var result = await _expenseTypesController.CreateAsync(fakeExpenseType);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(fakeExpectedExpenseType, createdResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenIdFieldIsNotEmpty()
        {
            // Arrange
            var fakeExpenseType = new ExpenseCategory
            {
                Code = 0,
                Description = "FakeDescription",
                Id = Guid.NewGuid()
            };

            // Act
            var result = await _expenseTypesController.CreateAsync(fakeExpenseType);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("ExpenseType Id field must be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenValidationIsNotValid()
        {
            // Arrange
            var fakeExpenseType = new ExpenseCategory
            {
                Code = 0,
                Description = "FakeDescription",
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockExpenseTypeValidator
                .Setup(v => v.ValidateAsync(It.IsAny<ExpenseCategory>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _expenseTypesController.CreateAsync(fakeExpenseType);

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
            var fakeExpectedExpenseTypes = new List<ExpenseCategory>()
            {
                new ExpenseCategory
                {
                    Code = 0,
                    Description = "FakeDescription",
                    Id = Guid.NewGuid(),
                    Deleted = false
                },
                new ExpenseCategory
                {
                    Code = 0,
                    Description = "FakeDescription",
                    Id = Guid.NewGuid(),
                    Deleted = false
                }
            };

            _mockExpenseTypeRepo
                .Setup(r => r.GetExpenseTypesAsync(false))
                .ReturnsAsync(fakeExpectedExpenseTypes);

            // Act
            var result = await _expenseTypesController.GetAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualExpenseTypes = Assert.IsAssignableFrom<IEnumerable<ExpenseCategory>>(okResult.Value);
            Assert.Equal(fakeExpectedExpenseTypes, actualExpenseTypes);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockExpenseTypeRepo
                .Setup(repo => repo.GetExpenseTypesAsync(false))
                .ThrowsAsync(new Exception());

            // Act
            async Task act() => await _expenseTypesController.GetAsync();

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeExpenseType = new ExpenseCategory
            {
                Code = 0,
                Description = "FakeDescription",
                Id = Guid.NewGuid()
            };

            _mockExpenseTypeValidator
                .Setup(v => v.ValidateAsync(It.IsAny<ExpenseCategory>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockExpenseTypeRepo
                .Setup(r => r.UpdateExpenseTypeAsync(fakeExpenseType))
                .ReturnsAsync(1);

            // Act
            var result = await _expenseTypesController.UpdateAsync(fakeExpenseType, fakeExpenseType.Id);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenExpenseTypeIdDoesNotMatch()
        {
            // Arrange
            var fakeExpenseType = new ExpenseCategory
            {
                Code = 0,
                Description = "FakeDescription",
                Id = Guid.NewGuid()
            }; ;

            // Act
            var result = await _expenseTypesController.UpdateAsync(fakeExpenseType, Guid.NewGuid());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ExpenseType Id from body incorrect", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenExpenseTypeValidationNotValid()
        {
            // Arrange
            var fakeExpenseType = new ExpenseCategory
            {
                Code = 0,
                Description = "FakeDescription",
                Id = Guid.NewGuid()
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockExpenseTypeValidator
                .Setup(v => v.ValidateAsync(It.IsAny<ExpenseCategory>(), CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _expenseTypesController.UpdateAsync(fakeExpenseType, fakeExpenseType.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenExpenseTypeNotFound()
        {
            // Arrange
            var fakeExpenseType = new ExpenseCategory
            {
                Code = 0,
                Description = "FakeDescription",
                Id = Guid.NewGuid()
            };

            _mockExpenseTypeValidator
                .Setup(v => v.ValidateAsync(It.IsAny<ExpenseCategory>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockExpenseTypeRepo
                .Setup(r => r.UpdateExpenseTypeAsync(It.IsAny<ExpenseCategory>()))
                .ReturnsAsync(0);

            // Act
            var result = await _expenseTypesController.UpdateAsync(fakeExpenseType, fakeExpenseType.Id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("ExpenseType not found", notFoundResult.Value);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeExpenseTypeId = Guid.NewGuid();

            _mockExpenseTypeRepo
                .Setup(r => r.SetDeleteExpenseTypeAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(1);

            // Act
            var result = await _expenseTypesController.DeleteAsync(fakeExpenseTypeId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenExpenseTypeNotFound()
        {
            // Arrange
            var fakeExpenseTypeId = Guid.NewGuid();

            _mockExpenseTypeRepo
                .Setup(r => r.SetDeleteExpenseTypeAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(0);

            // Act
            var result = await _expenseTypesController.DeleteAsync(fakeExpenseTypeId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("ExpenseType not found", notFoundResult.Value);
        }

        #endregion

        #region Undelete

        [Fact]
        public async Task UndeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeExpenseTypeId = Guid.NewGuid();

            _mockExpenseTypeRepo
                .Setup(r => r.SetDeleteExpenseTypeAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(1);

            // Act
            var result = await _expenseTypesController.UndeleteAsync(fakeExpenseTypeId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UndeleteAsync_ReturnsNotFound_WhenExpenseTypeNotFound()
        {
            // Arrange
            var fakeExpenseTypeId = Guid.NewGuid();

            _mockExpenseTypeRepo
                .Setup(r => r.SetDeleteExpenseTypeAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(0);

            // Act
            var result = await _expenseTypesController.UndeleteAsync(fakeExpenseTypeId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("ExpenseType not found", notFoundResult.Value);
        }

        #endregion
    }
}
