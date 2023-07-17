using DocumentsAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentsAPI.Services
{
    public interface IDocumentsService
    {
        Task<List<CreateDocumentStatus>> UploadAsync(Guid archiveId, IFormFile[] files, Guid? folderId = null);
        Task<IEnumerable<Document>> GetDocumentsAsync(Guid archiveId, int pSize = 100, Guid? folderId = null, bool includeDeleted = false);
        Task<Document?> GetDocumentByIdAsync(Guid archiveId, Guid documentId);
        Task<FileContentResult> DownloadAsync(Guid archiveId, Guid documentId);
        Task<IActionResult> DeleteAsync(Guid archiveId, Guid documentId);
        Task<IActionResult> UndeleteAsync(Guid archiveId, Guid documentId);
        Task<IActionResult> RenameAsync(Guid archiveId, Guid documentId, string documentName);
        Task<IActionResult> CopyAsync(Guid archiveId, Guid documentId, string documentName, Guid? folderId = null);
    }
}
