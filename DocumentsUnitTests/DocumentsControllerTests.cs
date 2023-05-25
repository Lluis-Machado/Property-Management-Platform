using Azure;
using Documents.Controllers;
using Documents.Models;
using DocumentsAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace DocumentsControllersTests
{
    public class DocumentsControllerTests
    {
        private readonly Mock<ILogger<DocumentsController>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IDocumentRepository> _mockDocumentRepository;
        private readonly DocumentsController _documentsController;

        public DocumentsControllerTests()
        {
            _mockLogger = new Mock<ILogger<DocumentsController>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockDocumentRepository = new Mock<IDocumentRepository>();
            _documentsController = new DocumentsController(_mockConfiguration.Object, _mockDocumentRepository.Object, _mockLogger.Object);
        }

        #region Upload
        [Fact(Skip = "Config MaxNbOfUploadFiles not yet implemented")]
        public async Task UploadDocumentAsync_WithValidRequest_ReturnsOk()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            var files = new List<IFormFile>
            {
                new FormFile(new MemoryStream(new byte[100]), 0, 100, "file1", "file1.txt"),
            };
            var expectedDocuments = new List<CreateDocumentStatus>
            {
                new CreateDocumentStatus("file1.txt", HttpStatusCode.OK),
            };

            _mockConfiguration.Setup(c => c.GetValue<int>("Files:MaxNbOfUploadFiles")).Returns(2);

            _mockDocumentRepository
                .Setup(x => x.UploadDocumentAsync(archiveId, It.IsAny<string>(), It.IsAny<Stream>(), Guid.NewGuid()))
                .ReturnsAsync(HttpStatusCode.OK);

            // Act
            var result = await _documentsController.UploadAsync(archiveId, files.ToArray(), Guid.NewGuid());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDocuments = Assert.IsAssignableFrom<List<CreateDocumentStatus>>(okResult.Value);
            Assert.Equal(expectedDocuments.Count, actualDocuments.Count);
            for (int i = 0; i < expectedDocuments.Count; i++)
            {
                Assert.Equal(expectedDocuments[i].FileName, actualDocuments[i].FileName);
                Assert.Equal(expectedDocuments[i].Status, actualDocuments[i].Status);
            }
            _mockDocumentRepository.Verify(
                x => x.UploadDocumentAsync(archiveId, It.IsAny<string>(), It.IsAny<Stream>(), Guid.NewGuid()),
                Times.Exactly(files.Count));
        }

        [Fact]
        public async Task UploadDocumentAsync_WithNullFiles_ReturnsBadRequest()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            var files = new List<IFormFile>();

            // Act
            var result = await _documentsController.UploadAsync(archiveId, files.ToArray());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Files not found", badRequestResult.Value);
        }

        [Fact(Skip = "Config MaxNbOfUploadFiles not yet implemented")]
        public async Task UploadDocumentAsync_WithTooManyFiles_ReturnsBadRequest()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            var files = new List<IFormFile>
            {
                new FormFile(new MemoryStream(new byte[100]), 0, 100, "file1", "file1.txt"),
                new FormFile(new MemoryStream(new byte[200]), 0, 200, "file2", "file2.txt"),
                new FormFile(new MemoryStream(new byte[300]), 0, 300, "file3", "file3.txt"),
                new FormFile(new MemoryStream(new byte[400]), 0, 400, "file4", "file4.txt"),
                new FormFile(new MemoryStream(new byte[500]), 0, 500, "file5", "file5.txt"),
                new FormFile(new MemoryStream(new byte[600]), 0, 600, "file6", "file6.txt")
        };

            _mockConfiguration.Setup(c => c.GetValue<int>("Files:MaxNbOfUploadFiles")).Returns(2);

            // Act
            var result = await _documentsController.UploadAsync(archiveId, files.ToArray());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Maximal number of files (5) exceeded", badRequestResult.Value);
        }

        [Fact(Skip = "Config MaxNbOfUploadFiles not yet implemented")]
        public async Task UploadDocumentAsync_WithSomeFilesFailing_ReturnsMultiStatus()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            var files = new List<IFormFile>
            {
            new FormFile(new MemoryStream(new byte[100]), 0, 100, "file1", "file1.txt"),
            new FormFile(new MemoryStream(new byte[200]), 0, 200, "file2", "file2.txt")
            };
            var expectedDocuments = new List<CreateDocumentStatus>
            {
            new CreateDocumentStatus("file1.txt", HttpStatusCode.OK),
            new CreateDocumentStatus("file2.txt", HttpStatusCode.NotFound)
            };

            _mockConfiguration.Setup(c => c.GetValue<int>("Files:MaxNbOfUploadFiles")).Returns(2);

            _mockDocumentRepository
                .Setup(x => x.UploadDocumentAsync(archiveId, It.IsAny<string>(), It.IsAny<Stream>(), Guid.NewGuid()))
                .ReturnsAsync((string t, string f, Stream s) =>
                    f == "file1.txt" ? HttpStatusCode.OK : HttpStatusCode.NotFound);

            // Act
            var result = await _documentsController.UploadAsync(archiveId, files.ToArray());

            // Assert
            var multiStatusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.MultiStatus, multiStatusResult.StatusCode);
            var actualDocuments = Assert.IsAssignableFrom<List<CreateDocumentStatus>>(multiStatusResult.Value);
            Assert.Equal(expectedDocuments.Count, actualDocuments.Count);
            for (int i = 0; i < expectedDocuments.Count; i++)
            {
                Assert.Equal(expectedDocuments[i].FileName, actualDocuments[i].FileName);
                Assert.Equal(expectedDocuments[i].Status, actualDocuments[i].Status);
            }
            _mockDocumentRepository.Verify(
                x => x.UploadDocumentAsync(archiveId, It.IsAny<string>(), It.IsAny<Stream>(), Guid.NewGuid()),
                Times.Exactly(files.Count));
        }
        #endregion

        #region Get

        [Fact]
        public async Task GetAsync_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            const int numberOfDocuments = 3;
            var expectedDocuments = Enumerable.Range(1, numberOfDocuments)
                .Select(i => new Document() { Name = $"file{i}.txt"})
                .ToList();

            _mockDocumentRepository
                .Setup(x => x.GetDocumentsFlatListingAsync(archiveId, 100, false))
                .ReturnsAsync(expectedDocuments);

            // Act
            var result = await _documentsController.GetDocumentsAsync(archiveId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDocuments = Assert.IsAssignableFrom<IEnumerable<Document>>(okResult.Value);
            Assert.Equal(expectedDocuments.Count, actualDocuments.Count());
            for (int i = 0; i < expectedDocuments.Count; i++)
            {
                Assert.Equal(expectedDocuments[i].Name, actualDocuments.ElementAt(i).Name);
            }
        }

        [Fact]
        public async Task GetAsync_WithIncludeDeleted_ReturnsOkResult()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            const bool includeDeleted = true;
            const int numberOfDocuments = 3;
            var expectedDocuments = Enumerable.Range(1, numberOfDocuments)
           .Select(i => new Document() { Name = $"file{i}.txt" })
                .ToList();

            _mockDocumentRepository
                .Setup(x => x.GetDocumentsFlatListingAsync(archiveId, 100, includeDeleted))
                .ReturnsAsync(expectedDocuments);

            // Act
            var result = await _documentsController.GetDocumentsAsync(archiveId,null, includeDeleted);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDocuments = Assert.IsAssignableFrom<IEnumerable<Document>>(okResult.Value);
            Assert.Equal(expectedDocuments.Count, actualDocuments.Count());
            for (int i = 0; i < expectedDocuments.Count; i++)
            {
                Assert.Equal(expectedDocuments[i].Name, actualDocuments.ElementAt(i).Name);
            }
        }

        [Fact]
        public async Task GetAsync_WithInvalidRequest_ReturnsNotFoundResult()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            var expectedException = new RequestFailedException((int)HttpStatusCode.NotFound, "DocumentNotFound");

            _mockDocumentRepository
                .Setup(x => x.GetDocumentsFlatListingAsync(archiveId, 100, false))
                .ThrowsAsync(expectedException);

            // Act
            async Task act() => await _documentsController.GetDocumentsAsync(archiveId);

            // Assert
            await Assert.ThrowsAsync<RequestFailedException>(act);
        }

        #endregion

        #region Download

        [Fact]
        public async Task DownloadAsync_ReturnsOkResult()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            Guid documentId = Guid.NewGuid();
            byte[] mockFileData = new byte[] { 0x1, 0x2, 0x3 };

            _mockDocumentRepository
                .Setup(s => s.DownloadDocumentAsync(archiveId, documentId))
                .ReturnsAsync(mockFileData);

            // Act
            FileContentResult result = await _documentsController.DownloadAsync(archiveId, documentId);

            // Assert
            Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/pdf", result.ContentType);
            Assert.Equal(mockFileData, result.FileContents);
        }

        [Fact]
        public async Task DownloadAsync_ReturnsNotFoundResult()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            Guid documentId = Guid.NewGuid();
            var expectedException = new RequestFailedException((int)HttpStatusCode.NotFound, "DocumentNotFound", "DocumentNotFound", new Exception("test"));

            _mockDocumentRepository
                .Setup(s => s.DownloadDocumentAsync(archiveId, documentId))
                .ThrowsAsync(expectedException);

            // Act
            async Task act() => await _documentsController.DownloadAsync(archiveId, documentId);

            // Assert
            await Assert.ThrowsAsync<RequestFailedException>(act);
        }

        [Fact]
        public async Task DownloadAsync_ReturnsInternalServerError()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            Guid documentId = Guid.NewGuid();
            var expectedException = new Exception("Something went wrong!");

            _mockDocumentRepository
                .Setup(s => s.DownloadDocumentAsync(archiveId, documentId))
                .ThrowsAsync(expectedException);

            // Act
            async Task act() => await _documentsController.DownloadAsync(archiveId, documentId);

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteAsync_WithValidInputs_ReturnsOkResult()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            Guid documentId = Guid.NewGuid();
            _mockDocumentRepository.Setup(x => x.DeleteDocumentAsync(archiveId, documentId)).Returns(Task.CompletedTask);

            // Act
            var result = await _documentsController.DeleteAsync(archiveId, documentId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidInputs_ReturnsNotFoundResult()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            Guid documentId = Guid.NewGuid();
            var expectedException = new RequestFailedException((int)HttpStatusCode.NotFound, "DocumentNotFound", "DocumentNotFound", new Exception("test"));
            _mockDocumentRepository.Setup(x => x.DeleteDocumentAsync(archiveId, documentId)).ThrowsAsync(expectedException);

            // Act
            async Task act() => await _documentsController.DeleteAsync(archiveId, documentId);

            // Assert
            await Assert.ThrowsAsync<RequestFailedException>(act);
        }

        #endregion

        #region Undelete


        [Fact]
        public async Task UndeleteAsync_WithValidInputs_ReturnsOkResult()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            Guid documentId = Guid.NewGuid();
            _mockDocumentRepository.Setup(x => x.UndeleteDocumentAsync(archiveId, documentId)).Returns(Task.CompletedTask);

            // Act
            var result = await _documentsController.UndeleteAsync(archiveId, documentId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UndeleteAsync_WithInvalidInputs_ReturnsNotFoundResult()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            Guid documentId = Guid.NewGuid();
            var expectedException = new RequestFailedException((int)HttpStatusCode.NotFound, "DocumentNotFound", "DocumentNotFound", new Exception("test"));
            _mockDocumentRepository.Setup(x => x.UndeleteDocumentAsync(archiveId, documentId)).ThrowsAsync(expectedException);

            // Act
            async Task act() => await _documentsController.UndeleteAsync(archiveId, documentId);

            // Assert
            await Assert.ThrowsAsync<RequestFailedException>(act);
        }

        #endregion

        #region Rename

        [Fact]
        public async Task RenameAsync_WithValidInputs_ReturnsOkResult()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            Guid documentId = Guid.NewGuid();
            string newDocumentId = "testNewDocumentId";
            _mockDocumentRepository.Setup(x => x.RenameDocumentAsync(archiveId, documentId, newDocumentId)).Returns(Task.CompletedTask);

            // Act
            var result = await _documentsController.RenameAsync(archiveId, documentId, newDocumentId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RenameAsync_WithInvalidInputs_ReturnsNotFoundResult()
        {
            // Arrange
            Guid archiveId = Guid.NewGuid();
            Guid documentId = Guid.NewGuid();
            string newDocumentId = "testNewDocumentId";
            var expectedException = new RequestFailedException((int)HttpStatusCode.NotFound, "DocumentNotFound", "DocumentNotFound", new Exception("test"));
            _mockDocumentRepository.Setup(x => x.RenameDocumentAsync(archiveId, documentId, newDocumentId)).ThrowsAsync(expectedException);

            // Act
            async Task act() => await _documentsController.RenameAsync(archiveId, documentId, newDocumentId);

            // Assert
            await Assert.ThrowsAsync<RequestFailedException>(act);
        }

        #endregion
    }
}