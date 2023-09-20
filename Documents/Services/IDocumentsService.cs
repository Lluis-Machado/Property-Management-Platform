using DocumentsAPI.DTOs;
using DocumentsAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentsAPI.Services
{
    public interface IDocumentsService
    {
        Task<List<CreateDocumentStatus>> UploadAsync(Guid archiveId, IFormFile[] files, Guid? folderId = null);
        Task<IEnumerable<Document>> GetDocumentsAsync(Guid archiveId, int? pSize = 100, Guid? folderId = null, bool includeDeleted = false);
        Task<Document?> GetDocumentByIdAsync(Guid archiveId, Guid documentId);
        Task<string?> GetDocumentUrlByIdAsync(Guid archiveId, Guid documentId);
        Task<IEnumerable<Document>> SearchDocumentsTagsAsync(string query, bool includeDeleted = false);
        Task<FileContentResult> DownloadAsync(Guid archiveId, Guid documentId);
        Task<IEnumerable<FileContentResult>> SplitAsync(IFormFile file, DocSplitInterval[]? range);
        Task<IEnumerable<string>> SplitBlobAsync(Guid archiveId, Guid documentId, DocSplitInterval[]? range);
        Task<IActionResult> DeleteAsync(Guid archiveId, Guid documentId);
        Task<IActionResult> UndeleteAsync(Guid archiveId, Guid documentId);
        Task<IActionResult> RenameAsync(Guid archiveId, Guid documentId, string documentName);
        Task<Guid> CopyAsync(Guid sourceArchive, Guid destinationArchive, Guid documentId, string documentName, Guid? folderId = null);
        Task<IActionResult> MoveAsync(Guid sourceArchive, Guid destinationArchive, Guid documentId, string documentName, Guid? folderId = null);

        Task<IEnumerable<BlobMetadata>> SearchMetadataAsync(string? displayName, Guid? folderId = null, Guid? containerId = null, bool includeDeleted = false);
    }
}
