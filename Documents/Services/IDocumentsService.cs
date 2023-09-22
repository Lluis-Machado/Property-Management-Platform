using DocumentsAPI.DTOs;
using DocumentsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using static MimeDetective.Definitions.Default.FileTypes;

namespace DocumentsAPI.Services
{
    public interface IDocumentsService
    {
        Task<Guid> CopyAsync(Guid sourceArchive, Guid destinationArchive, Guid documentId, string documentName, Guid? folderId = null);

        Task<IActionResult> DeleteAsync(Guid archiveId, Guid documentId);

        Task<FileContentResult> DownloadAsync(Guid archiveId, Guid documentId);

        Task<Document?> GetDocumentByIdAsync(Guid archiveId, Guid documentId);

        Task<IEnumerable<Document>> GetDocumentsAsync(Guid archiveId, int? pSize = 100, Guid? folderId = null, bool includeDeleted = false);

        Task<string?> GetDocumentUrlByIdAsync(Guid archiveId, Guid documentId);

        Task<string> JoinBlobsAsync(Guid archiveId, string[] documentIds);

        Task<IActionResult> MoveAsync(Guid sourceArchive, Guid destinationArchive, Guid documentId, string documentName, Guid? folderId = null);

        Task<IActionResult> RenameAsync(Guid archiveId, Guid documentId, string documentName);

        Task<IEnumerable<Document>> SearchDocumentsTagsAsync(string query, bool includeDeleted = false);

        Task<IEnumerable<BlobMetadata>> SearchMetadataAsync(string? displayName, Guid? folderId = null, Guid? containerId = null, bool includeDeleted = false);

        Task<IEnumerable<FileContentResult>> SplitAsync(IFormFile file, DocSplitInterval[]? range);

        Task<IEnumerable<string>> SplitBlobAsync(Guid archiveId, Guid documentId, DocSplitInterval[]? range);

        Task<IActionResult> UndeleteAsync(Guid archiveId, Guid documentId);

        Task<List<CreateDocumentStatus>> UploadAsync(Guid archiveId, IFormFile[] files, Guid? folderId = null);
    }
}
