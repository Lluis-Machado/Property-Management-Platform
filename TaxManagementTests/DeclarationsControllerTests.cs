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
            var declarant = new Declarant { Name = "testDeclarant", Id = Guid.NewGuid() };
            var declaration = new Declaration { DeclarantId = declarant.Id };
            var expectedDeclaration = new Declaration { 
                Id = Guid.NewGuid(),
                DeclarantId = declarant.Id,
                Status = 0,
                Deleted = false,
                CreateUser = "testUser",
                CreateDate = DateTime.Today,
                UpdateUser = "testUser",
                UpdateDate = DateTime.Today,
            };

            _mockDeclarantRepo.Setup(v => v.GetDeclarantByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(declarant);

            _mockDeclarationValidator.Setup(v => v.ValidateAsync(It.IsAny<Declaration>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockDeclarationRepo.Setup(r => r.InsertDeclarationAsync(It.IsAny<Declaration>()))
                .ReturnsAsync(expectedDeclaration);

            // Act
            var result = await _declarationssController.CreateAsync(declaration, declarant.Id);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(expectedDeclaration, createdResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenIdFieldIsNotEmpty()
        {
            // Arrange
            Guid declarantId = Guid.NewGuid();
            Guid declarationId = Guid.NewGuid();
            var declaration = new Declaration { DeclarantId = declarantId, Id = declarationId };

            // Act
            var result = await _declarationssController.CreateAsync(declaration, declarantId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Id fild must be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequestResult_WhenValidationIsNotValid()
        {
            // Arrange
            Guid declarantId = Guid.NewGuid();
            var declaration = new Declaration { DeclarantId = declarantId, Deleted = true };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Deleted", "Deleted must be false") });

            _mockDeclarationValidator.Setup(v => v.ValidateAsync(It.IsAny<Declaration>(), CancellationToken.None))
            .ReturnsAsync(validationResult);

            // Act
            var result = await _declarationssController.CreateAsync(declaration, declarantId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Deleted must be false", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsNotFoundResult_WhenDeclarantNotFound()
        {
            // Arrange
            var declarant = new Declarant();
            declarant = null;
            Guid declarantId = Guid.NewGuid();
            var declaration = new Declaration { DeclarantId = declarantId, Deleted = true };

            _mockDeclarationValidator.Setup(v => v.ValidateAsync(It.IsAny<Declaration>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockDeclarantRepo.Setup(v => v.GetDeclarantByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(declarant);

            // Act
            var result = await _declarationssController.CreateAsync(declaration, declarantId);

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
            var declarant = new Declarant()
            {
                Id = Guid.NewGuid()
            };
            Guid id1 = Guid.NewGuid();
            Guid id2 = Guid.NewGuid();

            var expectedDeclarations = new List<Declaration>()
            {
                new Declaration { DeclarantId = declarant.Id, Id = id1, Deleted = false },
                new Declaration { DeclarantId = declarant.Id, Id = id2, Deleted = false },
            };

            _mockDeclarantRepo.Setup(v => v.GetDeclarantByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(declarant);

            _mockDeclarationRepo.Setup(r => r.GetDeclarationsAsync(declarant.Id))
                .ReturnsAsync(expectedDeclarations);

            // Act
            var result = await _declarationssController.GetAsync(declarant.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDeclarations = Assert.IsAssignableFrom<IEnumerable<Declaration>>(okResult.Value);
            Assert.Equal(expectedDeclarations, actualDeclarations);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var declarant = new Declarant()
            {
                Id = Guid.NewGuid()
            };

            _mockDeclarantRepo.Setup(v => v.GetDeclarantByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(declarant);

            _mockDeclarationRepo.Setup(repo => repo.GetDeclarationsAsync(declarant.Id)).ThrowsAsync(new Exception());

            // Act
            async Task act() => await _declarationssController.GetAsync(declarant.Id);

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        #endregion

        #region Update
        [Fact]
        public async Task UpdateAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var declarant = new Declarant { Name = "testDeclarant", Id = Guid.NewGuid() };
            var declaration = new Declaration { DeclarantId = declarant.Id, Id = Guid.NewGuid() };

            _mockDeclarantRepo.Setup(v => v.GetDeclarantByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(declarant);

            _mockDeclarationValidator.Setup(v => v.ValidateAsync(It.IsAny<Declaration>(), CancellationToken.None))
               .ReturnsAsync(new ValidationResult());

            _mockDeclarationRepo.Setup(r => r.UpdateDeclarationAsync(declaration))
                .ReturnsAsync(1);

            // Act
            var result = await _declarationssController.UpdateAsync(declarant.Id, declaration.Id, declaration);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenDeclarationIdDoesNotMatch()
        {
            // Arrange
            var declarant = new Declarant { Name = "testDeclarant", Id = Guid.NewGuid() };
            var declaration = new Declaration { DeclarantId = Guid.NewGuid(), Id = Guid.NewGuid() };

            // Act
            var result = await _declarationssController.UpdateAsync(declarant.Id, Guid.NewGuid(), declaration);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Declaration Id from body incorrect", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenDeclarationValidationNotValid()
        {
            // Arrange
            var declarant = new Declarant { Name = "testDeclarant", Id = Guid.NewGuid() };
            var declaration = new Declaration { DeclarantId = Guid.NewGuid(), Id = Guid.NewGuid() };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Deleted", "Deleted must be false") });

            _mockDeclarationValidator.Setup(v => v.ValidateAsync(It.IsAny<Declaration>(), CancellationToken.None))
            .ReturnsAsync(validationResult);

            // Act
            var result = await _declarationssController.UpdateAsync(declarant.Id, declaration.Id, declaration);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Deleted must be false", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenDeclarationNotFound()
        {
            // Arrange
            var declarant = new Declarant { Name = "testDeclarant", Id = Guid.NewGuid() };
            var declaration = new Declaration { DeclarantId = declarant.Id, Id = Guid.NewGuid() };

            _mockDeclarantRepo.Setup(v => v.GetDeclarantByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(declarant);

            _mockDeclarationValidator.Setup(v => v.ValidateAsync(It.IsAny<Declaration>(), CancellationToken.None))
               .ReturnsAsync(new ValidationResult());

            _mockDeclarationRepo.Setup(r => r.UpdateDeclarationAsync(declaration))
                .ReturnsAsync(0);

            // Act
            var result = await _declarationssController.UpdateAsync(declarant.Id, declaration.Id, declaration);

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
            var declarant = new Declarant { Name = "testDeclarant", Id = Guid.NewGuid() };
            var declaration = new Declaration { DeclarantId = declarant.Id, Id = Guid.NewGuid() };

            _mockDeclarationRepo.Setup(r => r.SetDeletedDeclarationAsync(declaration.Id,"testUser", true))
                .ReturnsAsync(1);

            // Act
            var result = await _declarationssController.DeleteAsync(declarant.Id, declaration.Id, "testUser");

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenDeclarantNotFound()
        {
            // Arrange
            var declarant = new Declarant { Name = "testDeclarant", Id = Guid.NewGuid() };
            var declaration = new Declaration { DeclarantId = declarant.Id, Id = Guid.NewGuid() };

            _mockDeclarationRepo.Setup(r => r.SetDeletedDeclarationAsync(declarant.Id, "testUser", true))
                .ReturnsAsync(0);

            // Act
            var result = await _declarationssController.DeleteAsync(declarant.Id, declaration.Id, "testUser");

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
            var declarant = new Declarant { Name = "testDeclarant", Id = Guid.NewGuid() };
            var declaration = new Declaration { DeclarantId = declarant.Id, Id = Guid.NewGuid() };

            _mockDeclarationRepo.Setup(r => r.SetDeletedDeclarationAsync(declaration.Id, "testUser", false))
                .ReturnsAsync(1);

            // Act
            var result = await _declarationssController.UndeleteAsync(declarant.Id, declaration.Id, "testUser");

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UndeleteAsync_ReturnsNotFound_WhenDeclarantNotFound()
        {
            // Arrange
            var declarant = new Declarant { Name = "testDeclarant", Id = Guid.NewGuid() };
            var declaration = new Declaration { DeclarantId = declarant.Id, Id = Guid.NewGuid() };

            _mockDeclarationRepo.Setup(r => r.SetDeletedDeclarationAsync(declarant.Id, "testUser", false))
                .ReturnsAsync(0);

            // Act
            var result = await _declarationssController.UndeleteAsync(declarant.Id, declaration.Id, "testUser");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Declaration not found", notFoundResult.Value);
        }
        #endregion
    }
}
