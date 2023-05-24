using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FluentValidation;
using Documents.Models;
using Microsoft.Extensions.Logging;
using FluentValidation.Results;
using Azure;
using Newtonsoft.Json.Linq;
using DocumentsAPI.Repositories;
using Archives.Controllers;

namespace DocumentsControllersTests

{
    public class ArchivesControllerTests
    {
        private readonly Mock<ILogger<ArchivesController>> _mockLogger;
        private readonly Mock<IArchiveRepository> _mockArchiveRepository;
        private readonly Mock<IValidator<Archive>> _mockArchiveValidator;
        private readonly ArchivesController _archivesController;

        public ArchivesControllerTests()
        {
            _mockLogger = new Mock<ILogger<ArchivesController>>();
            _mockArchiveRepository = new Mock<IArchiveRepository>();
            _mockArchiveValidator = new Mock<IValidator<Archive>>();
            _archivesController = new ArchivesController(_mockArchiveRepository.Object, _mockArchiveValidator.Object, _mockLogger.Object);
        }

        #region Create
        [Fact]
        public async Task Create_WithValidInput_ReturnsOk()
        {
            //Arrange
            var archive = new Archive
            {
                Name = "TestArchive"
            };

            _mockArchiveRepository.Setup(x => x.CreateArchiveAsync(It.IsAny<Archive>())).Returns(Task.CompletedTask);

            _mockArchiveValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Archive>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            //Act
            var result = await _archivesController.Create(archive);

            //Assert
            var okResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal((int)HttpStatusCode.Created, okResult.StatusCode);

            _mockArchiveRepository.Verify(s => s.CreateArchiveAsync(archive), Times.Once);
        }

        [Fact(Skip = "Simulate json constructor not yet implemented")]
        public async Task Create_WithNullInput_ReturnsBadRequest()
        {
            //Arrange
            dynamic archive = new JObject();
            archive.fake = "fake";

            //Act
            var result = await _archivesController.Create(archive);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Incorrect body format", badRequestResult.Value);
        }

        [Fact]
        public async Task Create_WithInvalidInput_ReturnsBadRequest()
        {
            //Arrange
            var archive = new Archive
            {
                Name = "",
            };

            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name cannot be empty") });
            _mockArchiveValidator.Setup(v => v.ValidateAsync(It.IsAny<Archive>(),CancellationToken.None))
                                .ReturnsAsync(validationResult);

            //Act
            var result = await _archivesController.Create(archive);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Name cannot be empty", badRequestResult.Value);
        }
        #endregion

        #region Get
        [Fact]
        public async Task GetAsync_WhenIncludeDeletedIsFalse_ReturnsOkResultWithArchives()
        {
            // Arrange
            var archives = new List<Archive>
            {
            new Archive { Name = "Archive1" },
            new Archive { Name = "Archive2" },
            new Archive { Name = "Archive3" }
            };

            _mockArchiveRepository.Setup(x => x.GetArchivesAsync(100, false)).ReturnsAsync(archives);

            // Act
            var result = await _archivesController.GetAsync(false);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedArchives = Assert.IsAssignableFrom<IEnumerable<Archive>>(okResult.Value);
            Assert.Equal(archives.Count, returnedArchives.Count());
            Assert.Equal(archives[0].Name, returnedArchives.First().Name);
            Assert.Equal(archives[1].Name, returnedArchives.ElementAt(1).Name);
            Assert.Equal(archives[2].Name, returnedArchives.Last().Name);
        }

        [Fact]
        public async Task GetAsync_WhenIncludeDeletedIsTrue_ReturnsOkResultWithArchives()
        {
            // Arrange
            var archives = new List<Archive>
            {
            new Archive { Name = "Archive1" },
            new Archive { Name = "Archive2" },
            new Archive { Name = "Archive3" }
            };
            _mockArchiveRepository.Setup(x => x.GetArchivesAsync(100, true)).ReturnsAsync(archives);

            // Act
            var result = await _archivesController.GetAsync(true);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedArchives = Assert.IsAssignableFrom<IEnumerable<Archive>>(okResult.Value);
            Assert.Equal(archives.Count, returnedArchives.Count());
            Assert.Equal(archives[0].Name, returnedArchives.First().Name);
            Assert.Equal(archives[1].Name, returnedArchives.ElementAt(1).Name);
            Assert.Equal(archives[2].Name, returnedArchives.Last().Name);
        }

        #endregion

        #region Delete
        [Fact]
        public async Task DeleteAsync_WhenArchiveExists_ResturnsOK()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            _mockArchiveRepository.Setup(x => x.DeleteArchiveAsync(archiveId)).Returns(Task.CompletedTask);

            // Act
            var result = await _archivesController.DeleteAsync(archiveId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFoundResult_WhenArchiveDoesNotExist()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            var expectedException = new RequestFailedException((int)HttpStatusCode.NotFound, "ContainerNotFound");
            _mockArchiveRepository.Setup(x => x.DeleteArchiveAsync(archiveId)).ThrowsAsync(expectedException);

            // Act
            async Task act() => await _archivesController.DeleteAsync(archiveId);

            // Assert
            await Assert.ThrowsAsync<RequestFailedException>(act);
        }
        #endregion

        #region Undelete
        [Fact]
        public async Task UndeleteAsync_WhenArchiveExists_ResturnsOK()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            _mockArchiveRepository.Setup(x => x.UndeleteArchiveAsync(archiveId)).Returns(Task.CompletedTask);

            // Act
            var result = await _archivesController.UndeleteAsync(archiveId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UndeleteAsync_ReturnsNotFoundResult_WhenArchiveDoesNotExist()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            var expectedException = new RequestFailedException((int)HttpStatusCode.NotFound, "ContainerNotFound");
            _mockArchiveRepository.Setup(x => x.UndeleteArchiveAsync(archiveId)).ThrowsAsync(expectedException);

            // Act
            async Task act() => await _archivesController.UndeleteAsync(archiveId);

            // Assert
            await Assert.ThrowsAsync<RequestFailedException>(act);
        }
        #endregion
    }
}
