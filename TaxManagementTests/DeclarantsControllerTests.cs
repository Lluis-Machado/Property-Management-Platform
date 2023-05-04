using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TaxManagement.Controllers;
using TaxManagement.Models;
using TaxManagement.Repositories;


namespace TaxManagementControllerTests
{
    public class DeclarantsControllerTests
    {
        private readonly Mock<ILogger<DeclarantsController>> _mockLogger;
        private readonly Mock<IValidator<Declarant>> _mockDeclarantValidator;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IDeclarantRepository> _mockDeclarantRepo;
        private readonly DeclarantsController _declarantsController;

        public DeclarantsControllerTests()
        {
            _mockLogger = new Mock<ILogger<DeclarantsController>>();
            _mockDeclarantValidator = new Mock<IValidator<Declarant>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockDeclarantRepo = new Mock<IDeclarantRepository>();
            _declarantsController = new DeclarantsController(_mockDeclarantRepo.Object, _mockDeclarantValidator.Object, _mockLogger.Object);
        }

        #region Create
        [Fact]
        public async Task CreateAsync_ReturnsOkResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant" };
            var fakeExpectedDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid(), Deleted = false };

            _mockDeclarantValidator.Setup(v => v.ValidateAsync(It.IsAny<Declarant>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockDeclarantRepo.Setup(r => r.InsertDeclarantAsync(It.IsAny<Declarant>()))
                .ReturnsAsync(fakeExpectedDeclarant);

            // Act
            var result = await _declarantsController.CreateAsync(fakeDeclarant);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(fakeExpectedDeclarant, createdResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenIdFieldIsNotEmpty()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };

            // Act
            var result = await _declarantsController.CreateAsync(fakeDeclarant);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Id fild must be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenValidationIsNotValid()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant" };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });
            
            _mockDeclarantValidator.Setup(v => v.ValidateAsync(It.IsAny<Declarant>(), CancellationToken.None))
            .ReturnsAsync(validationResult);

            // Act
            var result = await _declarantsController.CreateAsync(fakeDeclarant);

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
            var fakeExpectedDeclarants = new List<Declarant>()
            {
                new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid(), Deleted = false },
                new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid(), Deleted = false }

            };

            _mockDeclarantRepo.Setup(r => r.GetDeclarantsAsync())
                .ReturnsAsync(fakeExpectedDeclarants);

            // Act
            var result = await _declarantsController.GetAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDeclarants = Assert.IsAssignableFrom<IEnumerable<Declarant>>(okResult.Value);
            Assert.Equal(fakeExpectedDeclarants, actualDeclarants);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockDeclarantRepo.Setup(repo => repo.GetDeclarantsAsync()).ThrowsAsync(new Exception());

            // Act
            async Task act() => await _declarantsController.GetAsync();

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        #endregion

        #region Update
        [Fact]
        public async Task UpdateAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };

            _mockDeclarantValidator.Setup(v => v.ValidateAsync(It.IsAny<Declarant>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

            _mockDeclarantRepo.Setup(r => r.UpdateDeclarantAsync(fakeDeclarant))
                .ReturnsAsync(1);

            // Act
            var result = await _declarantsController.UpdateAsync(fakeDeclarant, fakeDeclarant.Id);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenDeclarantIdDoesNotMatch()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };

            // Act
            var result = await _declarantsController.UpdateAsync(fakeDeclarant, Guid.NewGuid());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Declarant Id from body incorrect", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenDeclarantValidationNotValid()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });
            _mockDeclarantValidator.Setup(v => v.ValidateAsync(It.IsAny<Declarant>(), CancellationToken.None))
                                .ReturnsAsync(validationResult);

            // Act
            var result = await _declarantsController.UpdateAsync(fakeDeclarant, fakeDeclarant.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenDeclarantNotFound()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };

            _mockDeclarantValidator.Setup(v => v.ValidateAsync(It.IsAny<Declarant>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

            _mockDeclarantRepo.Setup(r => r.UpdateDeclarantAsync(It.IsAny<Declarant>()))
                .ReturnsAsync(0);

            // Act
            var result = await _declarantsController.UpdateAsync(fakeDeclarant, fakeDeclarant.Id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Declarant not found", notFoundResult.Value);
        }
        #endregion
        #region Delete
        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeDeclarantId = Guid.NewGuid();

            _mockDeclarantRepo.Setup(r => r.SetDeleteDeclarantAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(1);

            // Act
            var result = await _declarantsController.DeleteAsync(fakeDeclarantId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenDeclarantNotFound()
        {
            // Arrange
            var fakeDeclarantId = Guid.NewGuid();

            _mockDeclarantRepo.Setup(r => r.SetDeleteDeclarantAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(0);

            // Act
            var result = await _declarantsController.DeleteAsync(fakeDeclarantId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Declarant not found", notFoundResult.Value);
        }
        #endregion
        #region Undelete
        [Fact]
        public async Task UndeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeDeclarantId = Guid.NewGuid();

            _mockDeclarantRepo.Setup(r => r.SetDeleteDeclarantAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(1);

            // Act
            var result = await _declarantsController.UndeleteAsync(fakeDeclarantId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UndeleteAsync_ReturnsNotFound_WhenDeclarantNotFound()
        {
            // Arrange
            var fakeDeclarantId = Guid.NewGuid();

            _mockDeclarantRepo.Setup(r => r.SetDeleteDeclarantAsync(It.IsAny<Guid>(), false))
                .ReturnsAsync(0);

            // Act
            var result = await _declarantsController.UndeleteAsync(fakeDeclarantId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Declarant not found", notFoundResult.Value);
        }
        #endregion
    }
}
