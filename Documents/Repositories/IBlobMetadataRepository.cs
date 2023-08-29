using DocumentsAPI.Models;
using MongoDB.Driver;

namespace DocumentsAPI.Repositories
{
    public interface IBlobMetadataRepository
    {

        Task<BlobMetadata?> GetByBlobIdAsync(Guid blobId);
        Task<IEnumerable<BlobMetadata>> GetByContainerIdAsync(Guid containerId, Guid? folderId = null, bool includeDeleted = false);
        Task<BlobMetadata> InsertAsync(BlobMetadata blobMetadata, string username = "sa");
        Task<BlobMetadata> InsertAsync(Document document, Guid containerId, string username = "sa");
        Task<BlobMetadata> UpdateAsync(BlobMetadata blobMetadata, string username = "sa");
        Task<BlobMetadata> UpsertAsync(Guid blobId, Guid containerId, Guid? folderId, string displayName, string username = "sa");
        Task<UpdateResult> DeleteAsync(Guid blobId, string username = "sa");
        Task<UpdateResult> UndeleteAsync(Guid blobId, string username = "sa");

        // TODO: Allow searching by created/update user, create/update date
        Task<IEnumerable<BlobMetadata>> SearchMetadata(string? displayName, Guid? folderId = null, Guid? containerId = null, bool includeDeleted = false);

    }
}
