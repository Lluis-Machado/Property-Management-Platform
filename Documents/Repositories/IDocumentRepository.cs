using DocumentsAPI.DTOs;
using DocumentsAPI.Models;

namespace DocumentsAPI.Repositories
{
    public interface IDocumentRepository
    {
        Task<DocumentUploadDTO> UploadDocumentAsync(Guid archiveName, string fileName, Stream pFileContent, string contentType, Guid? folderId = null);
        Task<IEnumerable<Document>> GetDocumentsFlatListingAsync(Guid archiveId, int? pSegmentSize, Guid? folderId = null, bool includeDeleted = false);
        Task<Document?> GetDocumentByIdAsync(Guid archiveId, Guid documentId);
        Task<string?> GetDocumentUrlByIdAsync(Guid archiveId, Guid documentId);
        Task<IEnumerable<Document>> SearchDocumentsTagsAsync(string query, bool includeDeleted = false);
        Task<bool> DocumentExistsAsync(Guid archiveName, Guid documentId);
        Task<byte[]> DownloadDocumentAsync(Guid archiveName, Guid documentId);
        Task DeleteDocumentAsync(Guid archiveName, Guid documentId);
        Task UndeleteDocumentAsync(Guid archiveName, Guid documentId);
        Task RenameDocumentAsync(Guid archiveName, Guid documentId, string newDocumentName);
        Task<Guid> CopyDocumentAsync(Guid sourceArchive, Guid destinationArchive, Guid documentId, string newDocumentName, Guid? folderId = null);
        Task MoveDocumentAsync(Guid sourceArchive, Guid destinationArchive, Guid documentId, string newDocumentName, Guid? folderId = null);
    }
}
