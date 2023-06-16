using Documents.Models;
using DocumentsAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Documents.Services
{
    public class DocumentsService : IDocumentsService
    {
        private readonly IConfiguration _config;
        private readonly IDocumentRepository _documentsRepository;

        public DocumentsService(IConfiguration config, IDocumentRepository documentsRepository)
        {
            _config = config;
            _documentsRepository = documentsRepository;
        }

        public async Task<List<CreateDocumentStatus>> UploadAsync(Guid archiveId, IFormFile[] files, Guid? folderId = null)
        {
            var documents = new List<CreateDocumentStatus>();

            await Parallel.ForEachAsync(files, async (file, CancellationToken) =>
            {
                Stream fileStream = file.OpenReadStream();
                HttpStatusCode status = await _documentsRepository.UploadDocumentAsync(archiveId, file.FileName, fileStream, folderId);
                documents.Add(new CreateDocumentStatus(file.FileName, status));
            });

            return documents;

        }

        public async Task<IEnumerable<Document>> GetDocumentsAsync(Guid archiveId, int pSize = 100 , bool includeDeleted = false)
        {
            return await _documentsRepository.GetDocumentsFlatListingAsync(archiveId, 100, includeDeleted);
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

        public async Task<IActionResult> CopyAsync(Guid archiveId, Guid documentId, string documentName)
        {
            await _documentsRepository.CopyDocumentAsync(archiveId, documentId, documentName);
            return new NoContentResult();
        }
    }
}
