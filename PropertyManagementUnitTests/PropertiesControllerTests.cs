using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using PropertyManagementAPI.Controllers;
using PropertyManagementAPI.Models;
using PropertyManagementAPI.Repositories;

namespace PropertyManagementUnitTests
{
    public class PropertiesControllerTests
    {
        private readonly Mock<ILogger<PropertiesController>> _mockLogger;
        private readonly Mock<IValidator<Property>> _mockPropertyValidator;
        private readonly Mock<IValidator<Address>> _mockAddressValidator;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IPropertiesRepository> _mockPropertiesRepo;
        private readonly Mock<UpdateResult> _mockUpdateResult;
        private readonly PropertiesController _propertiesController;

        public PropertiesControllerTests()
        {
            _mockLogger = new Mock<ILogger<PropertiesController>>();
            _mockPropertyValidator = new Mock<IValidator<Property>>();
            _mockAddressValidator = new Mock<IValidator<Address>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockPropertiesRepo = new Mock<IPropertiesRepository>();
            _mockUpdateResult = new Mock<UpdateResult>();
            _propertiesController = new PropertiesController(_mockPropertiesRepo.Object, _mockLogger.Object, _mockPropertyValidator.Object, _mockAddressValidator.Object);
        }

        #region Create
        [Fact]
        public async Task CreateAsync_ReturnsOkResult_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeProperty = new Property { Name = "fakeProperty" };
            var fakeExpectedProperty = new Property { Name = "fakeProperty", _id = Guid.NewGuid(), Deleted = false };

            _mockPropertyValidator.Setup(v => v.ValidateAsync(It.IsAny<Property>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockPropertiesRepo.Setup(r => r.InsertOneAsync(It.IsAny<Property>()))
                .ReturnsAsync(fakeExpectedProperty);

            // Act
            var result = await _propertiesController.CreateAsync(fakeProperty);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(fakeExpectedProperty, createdResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsOkResult_WhenIdFieldIsNotEmpty()
        {
            // Arrange
            var fakeProperty = new Property { Name = "fakeProperty", _id = Guid.NewGuid() };

            // Act
            var result = await _propertiesController.CreateAsync(fakeProperty);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Id fild must be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsOkResult_WhenValidationIsNotValid()
        {
            // Arrange
            var fakeProperty = new Property { Name = "fakeProperty" };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });

            _mockPropertyValidator.Setup(v => v.ValidateAsync(It.IsAny<Property>(), CancellationToken.None))
            .ReturnsAsync(validationResult);


            // Act
            var result = await _propertiesController.CreateAsync(fakeProperty);

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
            var fakeExpectedProperties = new List<Property>()
            {
                new Property { Name = "fakeProperty", _id = Guid.NewGuid(), Deleted = false },
                new Property { Name = "fakeProperty", _id = Guid.NewGuid(), Deleted = false }

            };

            _mockPropertiesRepo.Setup(r => r.GetAsync())
                .ReturnsAsync(fakeExpectedProperties);

            // Act
            var result = await _propertiesController.GetAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualProperties = Assert.IsAssignableFrom<IEnumerable<Property>>(okResult.Value);
            Assert.Equal(fakeExpectedProperties, actualProperties);
        }

        [Fact]
        public async Task GetAsync_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockPropertiesRepo.Setup(repo => repo.GetAsync()).ThrowsAsync(new Exception());

            // Act
            async Task act() => await _propertiesController.GetAsync();

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }
        #endregion

        #region Update
        [Fact(Skip ="mock Properties Repo response not done")]
        public async Task UpdateAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakeProperty = new Property { Name = "fakeProperty", _id = Guid.NewGuid() };

            _mockPropertyValidator.Setup(v => v.ValidateAsync(It.IsAny<Property>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

            //_mockPropertiesRepo.Setup(r => r.UpdateAsync(fakeProperty))
            //    .ReturnsAsync(new UpdateResult());

            // Act
            var result = await _propertiesController.UpdateAsync(fakeProperty, fakeProperty._id);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenDeclarantIdDoesNotMatch()
        {
            // Arrange
            var fakeProperty = new Property { Name = "fakeProperty", _id = Guid.NewGuid() };

            // Act
            var result = await _propertiesController.UpdateAsync(fakeProperty, Guid.NewGuid());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Property Id from body incorrect", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsBadResuqest_WhenDeclarantValidationNotValid()
        {
            // Arrange
            var fakeProperty = new Property { Name = "fakeProperty", _id = Guid.NewGuid() };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });
            _mockPropertyValidator.Setup(v => v.ValidateAsync(It.IsAny<Property>(), CancellationToken.None))
                                  .ReturnsAsync(validationResult);

            // Act
            var result = await _propertiesController.UpdateAsync(fakeProperty, fakeProperty._id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }

        [Fact(Skip = "mock Properties Repo response not done")]
        public async Task UpdateAsync_ReturnsNotFound_WhenDeclarantNotFound()
        {
            // Arrange
            var fakeProperty = new Property { Name = "fakeProperty", _id = Guid.NewGuid() };

            _mockPropertyValidator.Setup(v => v.ValidateAsync(It.IsAny<Property>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

            //_mockPropertyRepo.Setup(r => r.UpdateDeclarantAsync(It.IsAny<Property>()))
            //    .ReturnsAsync(0);

            // Act
            var result = await _propertiesController.UpdateAsync(fakeProperty, fakeProperty._id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Property not found", notFoundResult.Value);
        }
        #endregion

        #region Delete
        [Fact(Skip = "mock Properties Repo response not done")]
        public async Task DeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakePropertyId = Guid.NewGuid();

            //_mockPropertiesRepo.Setup(r => r.SetDeleteAsync(It.IsAny<Guid>(), true, "UserId"))
            //    .ReturnsAsync(1);

            // Act
            var result = await _propertiesController.DeleteAsync(fakePropertyId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact(Skip = "mock Properties Repo response not done")]
        public async Task DeleteAsync_ReturnsNotFound_WhenDeclarantNotFound()
        {
            // Arrange
            var fakePropertyId = Guid.NewGuid();

            //_mockPropertiesRepo.Setup(r => r.SetDeleteAsync(It.IsAny<Guid>(), true, "UserId"))
            //    .ReturnsAsync(0);

            // Act
            var result = await _propertiesController.DeleteAsync(fakePropertyId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Property not found", notFoundResult.Value);
        }
        #endregion

        #region Undelete
        [Fact(Skip = "mock Properties Repo response not done")]
        public async Task UndeleteAsync_ReturnsNoContent_WhenValidRequestIsMade()
        {
            // Arrange
            var fakePropertyId = Guid.NewGuid();

            //_mockPropertiesRepo.Setup(r => r.SetDeleteAsync(It.IsAny<Guid>(), false, "UserId"))
            //    .ReturnsAsync(1);

            // Act
            var result = await _propertiesController.DeleteAsync(fakePropertyId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact(Skip = "mock Properties Repo response not done")]
        public async Task UndeleteAsync_ReturnsNotFound_WhenDeclarantNotFound()
        {
            // Arrange
            var fakePropertyId = Guid.NewGuid();

            //_mockPropertiesRepo.Setup(r => r.SetDeleteAsync(It.IsAny<Guid>(), false, "UserId"))
            //    .ReturnsAsync(0);

            // Act
            var result = await _propertiesController.DeleteAsync(fakePropertyId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Property not found", notFoundResult.Value);
        }
        #endregion
    }
}