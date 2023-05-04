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
    public class DeclarantionsControllerTests
    {
        private readonly Mock<ILogger<DeclarationsController>> _mockLogger;
        private readonly Mock<IValidator<Declaration>> _mockDeclarationValidator;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IDeclarationRepository> _mockDeclarationRepo;
        private readonly Mock<IDeclarantRepository> _mockDeclarantRepo;
        private readonly DeclarationsController _declarationssController;

        public DeclarantionsControllerTests()
        {
            _mockLogger = new Mock<ILogger<DeclarationsController>>();
            _mockDeclarationValidator = new Mock<IValidator<Declaration>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockDeclarationRepo = new Mock<IDeclarationRepository>();
            _mockDeclarantRepo = new Mock<IDeclarantRepository>();
            _declarationssController = new DeclarationsController(_mockDeclarationRepo.Object, _mockDeclarantRepo.Object, _mockDeclarationValidator.Object, _mockLogger.Object);
        }

        #region Create
        [Fact]
        public async Task CreateAsync_ReturnsOkResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };
            var fakeDeclaration = new Declaration { DeclarantId = fakeDeclarant.Id };
            var fakeExpectedDeclaration = new Declaration { 
                Id = Guid.NewGuid(),
                DeclarantId = fakeDeclarant.Id,
                Status = 0,
                Deleted = false,
                CreateUser = "fakeUser",
                CreateDate = DateTime.Today,
                UpdateUser = "fakeUser",
                UpdateDate = DateTime.Today,
            };

            _mockDeclarantRepo.Setup(v => v.GetDeclarantByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeDeclarant);

            _mockDeclarationValidator.Setup(v => v.ValidateAsync(It.IsAny<Declaration>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockDeclarationRepo.Setup(r => r.InsertDeclarationAsync(It.IsAny<Declaration>()))
                .ReturnsAsync(fakeExpectedDeclaration);

            // Act
            var result = await _declarationssController.CreateAsync(fakeDeclaration, fakeDeclarant.Id);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(fakeExpectedDeclaration, createdResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenIdFieldIsNotEmpty()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };
            var fakeDeclaration = new Declaration { DeclarantId = fakeDeclarant.Id, Id = Guid.NewGuid()};

            // Act
            var result = await _declarationssController.CreateAsync(fakeDeclaration, fakeDeclaration.DeclarantId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Id fild must be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenValidationIsNotValid()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };
            var fakeDeclaration = new Declaration { DeclarantId = fakeDeclarant.Id};

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Deleted", "Deleted must be false") });

            _mockDeclarationValidator.Setup(v => v.ValidateAsync(It.IsAny<Declaration>(), CancellationToken.None))
            .ReturnsAsync(validationResult);

            // Act
            var result = await _declarationssController.CreateAsync(fakeDeclaration, fakeDeclaration.DeclarantId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Deleted must be false", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsNotFoundResult_WhenDeclarantNotFound()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };
            var fakeDeclaration = new Declaration { DeclarantId = fakeDeclarant.Id};

            _mockDeclarationValidator.Setup(v => v.ValidateAsync(It.IsAny<Declaration>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            fakeDeclarant = null;

            _mockDeclarantRepo.Setup(v => v.GetDeclarantByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeDeclarant);

            // Act
            var result = await _declarationssController.CreateAsync(fakeDeclaration, fakeDeclaration.DeclarantId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Declarant not found", notFoundResult.Value);
        }
        #endregion

        #region Get
        [Fact]
        public async Task GetAsync_ReturnsOkResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };
            var fakeDeclaration = new Declaration { DeclarantId = fakeDeclarant.Id, Id = Guid.NewGuid() };

            var fakeExpectedDeclarations = new List<Declaration>
            {
                fakeDeclaration,
                fakeDeclaration
            };


            _mockDeclarantRepo.Setup(v => v.GetDeclarantByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeDeclarant);

            _mockDeclarationRepo.Setup(r => r.GetDeclarationsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeExpectedDeclarations);

            // Act
            var result = await _declarationssController.GetAsync(fakeDeclarant.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDeclarations = Assert.IsAssignableFrom<IEnumerable<Declaration>>(okResult.Value);
            Assert.Equal(fakeExpectedDeclarations, actualDeclarations);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };

            _mockDeclarantRepo.Setup(v => v.GetDeclarantByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeDeclarant);

            _mockDeclarationRepo.Setup(repo => repo.GetDeclarationsAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception());

            // Act
            async Task act() => await _declarationssController.GetAsync(fakeDeclarant.Id);

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
            var fakeDeclaration = new Declaration { DeclarantId = fakeDeclarant.Id, Id = Guid.NewGuid() };

            _mockDeclarantRepo.Setup(v => v.GetDeclarantByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeDeclarant);

            _mockDeclarationValidator.Setup(v => v.ValidateAsync(It.IsAny<Declaration>(), CancellationToken.None))
               .ReturnsAsync(new ValidationResult());

            _mockDeclarationRepo.Setup(r => r.UpdateDeclarationAsync(It.IsAny<Declaration>()))
                .ReturnsAsync(1);

            // Act
            var result = await _declarationssController.UpdateAsync(fakeDeclaration.DeclarantId, fakeDeclaration.Id, fakeDeclaration);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenDeclarationIdDoesNotMatch()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };
            var fakeDeclaration = new Declaration { DeclarantId = fakeDeclarant.Id, Id = Guid.NewGuid() };

            // Act
            var result = await _declarationssController.UpdateAsync(fakeDeclaration.DeclarantId, Guid.NewGuid(), fakeDeclaration);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Declaration Id from body incorrect", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenDeclarationValidationNotValid()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };
            var fakeDeclaration = new Declaration { DeclarantId = fakeDeclarant.Id, Id = Guid.NewGuid() };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Deleted", "Deleted must be false") });

            _mockDeclarationValidator.Setup(v => v.ValidateAsync(It.IsAny<Declaration>(), CancellationToken.None))
            .ReturnsAsync(validationResult);

            // Act
            var result = await _declarationssController.UpdateAsync(fakeDeclaration.DeclarantId, fakeDeclaration.Id, fakeDeclaration);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Deleted must be false", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenDeclarationNotFound()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };
            var fakeDeclaration = new Declaration { DeclarantId = fakeDeclarant.Id, Id = Guid.NewGuid() };

            _mockDeclarantRepo.Setup(v => v.GetDeclarantByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(fakeDeclarant);

            _mockDeclarationValidator.Setup(v => v.ValidateAsync(It.IsAny<Declaration>(), CancellationToken.None))
               .ReturnsAsync(new ValidationResult());

            _mockDeclarationRepo.Setup(r => r.UpdateDeclarationAsync(It.IsAny<Declaration>()))
                .ReturnsAsync(0);

            // Act
            var result = await _declarationssController.UpdateAsync(fakeDeclaration.DeclarantId, fakeDeclaration.Id, fakeDeclaration);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Declaration not found", notFoundResult.Value);
        }
        #endregion

        #region Delete
        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };
            var fakeDeclaration = new Declaration { DeclarantId = fakeDeclarant.Id, Id = Guid.NewGuid() };

            _mockDeclarationRepo.Setup(r => r.SetDeletedDeclarationAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(1);

            // Act
            var result = await _declarationssController.DeleteAsync(fakeDeclaration.DeclarantId, fakeDeclaration.Id, "fakeUser");

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenDeclarantNotFound()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };
            var fakeDeclaration = new Declaration { DeclarantId = fakeDeclarant.Id, Id = Guid.NewGuid() };

            _mockDeclarationRepo.Setup(r => r.SetDeletedDeclarationAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(0);

            // Act
            var result = await _declarationssController.DeleteAsync(fakeDeclaration.DeclarantId, fakeDeclaration.Id, "fakeUser");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Declaration not found", notFoundResult.Value);
        }
        #endregion
        #region Undelete
        [Fact]
        public async Task UndeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };
            var fakeDeclaration = new Declaration { DeclarantId = fakeDeclarant.Id, Id = Guid.NewGuid() };

            _mockDeclarationRepo.Setup(r => r.SetDeletedDeclarationAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()))
             .ReturnsAsync(1);

            // Act
            var result = await _declarationssController.UndeleteAsync(fakeDeclaration.DeclarantId, fakeDeclaration.Id, "fakeUser");

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UndeleteAsync_ReturnsNotFound_WhenDeclarantNotFound()
        {
            // Arrange
            var fakeDeclarant = new Declarant { Name = "fakeDeclarant", Id = Guid.NewGuid() };
            var fakeDeclaration = new Declaration { DeclarantId = fakeDeclarant.Id, Id = Guid.NewGuid() };

            _mockDeclarationRepo.Setup(r => r.SetDeletedDeclarationAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(0);

            // Act
            var result = await _declarationssController.UndeleteAsync(fakeDeclaration.DeclarantId, fakeDeclaration.Id, "fakeUser");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Declaration not found", notFoundResult.Value);
        }
        #endregion
    }
}
