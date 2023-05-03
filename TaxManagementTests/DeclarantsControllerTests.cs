using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
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
            Guid id = Guid.NewGuid();
            var declarant = new Declarant { Name = "testDeclarant" };
            var expectedDeclarant = new Declarant { Name = "testDeclarant", Id = id, Deleted = false };

            _mockDeclarantValidator.Setup(v => v.ValidateAsync(It.IsAny<Declarant>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockDeclarantRepo.Setup(r => r.InsertDeclarantAsync(It.IsAny<Declarant>()))
                .ReturnsAsync(expectedDeclarant);

            // Act
            var result = await _declarantsController.CreateAsync(declarant);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(expectedDeclarant, createdResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenIdFieldIsNotEmpty()
        {
            // Arrange
            var declarant = new Declarant { Name = "testDeclarant", Id = Guid.NewGuid() };

            // Act
            var result = await _declarantsController.CreateAsync(declarant);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Id fild must be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenValidationIsNotValid()
        {
            // Arrange
            var declarant = new Declarant { Name = "testDeclarant" };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });
            _mockDeclarantValidator.Setup(v => v.ValidateAsync(It.IsAny<Declarant>(), CancellationToken.None))
            .ReturnsAsync(validationResult);

            // Act
            var result = await _declarantsController.CreateAsync(declarant);

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
            Guid id1 = Guid.NewGuid();
            Guid id2 = Guid.NewGuid();

            var expectedDeclarants = new List<Declarant>()
            {
                new Declarant { Name = "testDeclarant1", Id = id1, Deleted = false },
                new Declarant { Name = "testDeclarant2", Id = id2, Deleted = false }

            };

            _mockDeclarantRepo.Setup(r => r.GetDeclarantsAsync())
                .ReturnsAsync(expectedDeclarants);

            // Act
            var result = await _declarantsController.GetAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDeclarants = Assert.IsAssignableFrom<IEnumerable<Declarant>>(okResult.Value);
            Assert.Equal(expectedDeclarants, actualDeclarants);
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
            var id = Guid.NewGuid();
            var declarant = new Declarant { Name = "testDeclarant", Id = id };

            _mockDeclarantValidator.Setup(v => v.ValidateAsync(It.IsAny<Declarant>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

            _mockDeclarantRepo.Setup(r => r.UpdateDeclarantAsync(declarant))
                .ReturnsAsync(1);

            // Act
            var result = await _declarantsController.UpdateAsync(declarant, id);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenDeclarantIdDoesNotMatch()
        {
            // Arrange
            var id = Guid.NewGuid();
            var declarant = new Declarant { Name = "testDeclarant", Id = id };

            // Act
            var result = await _declarantsController.UpdateAsync(declarant, Guid.NewGuid());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Declarant Id from body incorrect", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenDeclarantValidationNotValid()
        {
            // Arrange
            var id = Guid.NewGuid();
            var declarant = new Declarant { Name = "testDeclarant", Id = id };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });
            _mockDeclarantValidator.Setup(v => v.ValidateAsync(It.IsAny<Declarant>(), CancellationToken.None))
                                .ReturnsAsync(validationResult);

            // Act
            var result = await _declarantsController.UpdateAsync(declarant, id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenDeclarantNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var declarant = new Declarant { Name = "testDeclarant", Id = id };

            _mockDeclarantValidator.Setup(v => v.ValidateAsync(It.IsAny<Declarant>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

            _mockDeclarantRepo.Setup(r => r.UpdateDeclarantAsync(declarant))
                .ReturnsAsync(0);

            // Act
            var result = await _declarantsController.UpdateAsync(declarant, id);

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
            var id = Guid.NewGuid();

            _mockDeclarantRepo.Setup(r => r.SetDeleteDeclarantAsync(id, true))
                .ReturnsAsync(1);

            // Act
            var result = await _declarantsController.DeleteAsync(id);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenDeclarantNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockDeclarantRepo.Setup(r => r.SetDeleteDeclarantAsync(id, true))
                .ReturnsAsync(0);

            // Act
            var result = await _declarantsController.DeleteAsync(id);

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
            var id = Guid.NewGuid();

            _mockDeclarantRepo.Setup(r => r.SetDeleteDeclarantAsync(id, false))
                .ReturnsAsync(1);

            // Act
            var result = await _declarantsController.UndeleteAsync(id);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UndeleteAsync_ReturnsNotFound_WhenDeclarantNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockDeclarantRepo.Setup(r => r.SetDeleteDeclarantAsync(id, false))
                .ReturnsAsync(0);

            // Act
            var result = await _declarantsController.UndeleteAsync(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Declarant not found", notFoundResult.Value);
        }
        #endregion
    }
}
