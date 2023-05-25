﻿using Documents.Models;
using System.Net;

namespace DocumentsAPI.Repositories
{
    public interface IDocumentRepository
    {
        Task<HttpStatusCode> UploadDocumentAsync(Guid archiveName, string fileName, Stream pFileContent, Guid? folderId = null);
        Task<IEnumerable<Document>> GetDocumentsFlatListingAsync(Guid archiveName, int? pSegmentSize, bool includeDeleted = false);
        Task<bool> DocumentExistsAsync(Guid archiveName, Guid documentId);
        Task<byte[]> DownloadDocumentAsync(Guid archiveName, Guid documentId);
        Task DeleteDocumentAsync(Guid archiveName, Guid documentId);
        Task UndeleteDocumentAsync(Guid archiveName, Guid documentId);
        Task RenameDocumentAsync(Guid archiveName, Guid documentId, string newDocumentName);
        Task CopyDocumentAsync(Guid archiveName, Guid documentId, string newDocumentName);
    }
}