using DocumentsAPI.Models;
using DocumentsAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DocumentsAPI.Services
{
    public class DocumentsService : IDocumentsService
    {
        private readonly IConfiguration _config;
        private readonly IDocumentRepository _documentsRepository;
        private readonly ILogger<DocumentsService> _logger;

        public DocumentsService(IConfiguration config, IDocumentRepository documentsRepository, ILogger<DocumentsService> logger)
        {
            _config = config;
            _documentsRepository = documentsRepository;
            _logger = logger;
        }

        public async Task<List<CreateDocumentStatus>> UploadAsync(Guid archiveId, IFormFile[] files, Guid? folderId = null)
        {

            //_logger.LogInformation($"Documents - Starting upload of {files.Length} documents to archive {archiveId} - FolderID: {folderId}");

            var documents = new List<CreateDocumentStatus>();

            await Parallel.ForEachAsync(files, parallelOptions: new ParallelOptions { MaxDegreeOfParallelism = 4 }, async (file, CancellationToken) =>
            {
                Stream fileStream = file.OpenReadStream();
                fileStream.Position = 0;
                HttpStatusCode status = await _documentsRepository.UploadDocumentAsync(archiveId, file.FileName, fileStream, folderId);
                fileStream.Flush();
                documents.Add(new CreateDocumentStatus(file.FileName, status));
                fileStream.Dispose();
                //_logger.LogInformation($"**Documents - Uploaded file {file.FileName} - Status: {(int)status}");

            });

            _logger.LogInformation($"Documents - Upload completed, {documents.FindAll(d => d.Status == HttpStatusCode.Created).Count}/{files.Length} files OK");


            return documents;

        }

        public async Task<IEnumerable<Document>> GetDocumentsAsync(Guid archiveId, int? pSize = 100, Guid? folderId = null, bool includeDeleted = false)
        {
            return await _documentsRepository.GetDocumentsFlatListingAsync(archiveId, pSize, folderId, includeDeleted);
        }

        public async Task<Document?> GetDocumentByIdAsync(Guid archiveId, Guid documentId)
        {
            return await _documentsRepository.GetDocumentByIdAsync(archiveId, documentId);
        }

        public async Task<FileContentResult> DownloadAsync(Guid archiveId, Guid documentId)
        {
            byte[] byteArray = await _documentsRepository.DownloadDocumentAsync(archiveId, documentId);
            return new FileContentResult(byteArray, "application/pdf");
        }

        public async Task<IActionResult> DeleteAsync(Guid archiveId, Guid documentId)
        {
            await _documentsRepository.DeleteDocumentAsync(archiveId, documentId);
            return new NoContentResult();
        }

        public async Task<IActionResult> UndeleteAsync(Guid archiveId, Guid documentId)
        {
            await _documentsRepository.UndeleteDocumentAsync(archiveId, documentId);
            return new NoContentResult();
        }

        public async Task<IActionResult> RenameAsync(Guid archiveId, Guid documentId, string documentName)
        {
            await _documentsRepository.RenameDocumentAsync(archiveId, documentId, documentName);
            return new NoContentResult();
        }

        public async Task<Guid> CopyAsync(Guid sourceArchive, Guid destinationArchive, Guid documentId, string documentName, Guid? folderId = null)
        {
            return await _documentsRepository.CopyDocumentAsync(sourceArchive, destinationArchive, documentId, documentName, folderId);
        }

        public async Task<IActionResult> MoveAsync(Guid sourceArchive, Guid destinationArchive, Guid documentId, string documentName, Guid? folderId = null)
        {
            await _documentsRepository.MoveDocumentAsync(sourceArchive, destinationArchive, documentId, documentName, folderId);
            return new NoContentResult();
        }
    }
}
