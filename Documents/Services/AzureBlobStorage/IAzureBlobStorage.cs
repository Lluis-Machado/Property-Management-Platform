using Documents.Models;
using System.Net;

namespace Documents.Services.AzureBlobStorage
{
    public interface IAzureBlobStorage
    {
        #region Containers
        Task CreateBlobContainerAsync(string pBlobContainerName);
        Task<IEnumerable<Tenant>> GetBlobContainersAsync(int? pSegmentSize, bool includeDeleted = false);
        Task<bool> BlobContainerExistsAsync(string blobContainerName);
        Task DeleteBlobContainerAsync(string pBlobContainerName);
        Task UndeleteBlobContainerAsync(string pBlobContainerName);
        #endregion
        #region Blobs
        Task<HttpStatusCode> UploadAsync(string pBlobContainerName, string pFileName, Stream pFileContent);
        Task<IEnumerable<Document>> ListBlobsFlatListingAsync(string pBlobContainerName, int? pSegmentSize, bool includeDeleted = false);
        Task<bool> BlobExistsAsync(string blobContainerName, string blobName);
        Task<byte[]> DownloadBlobAsync(string pBlobContainerName, string pBlobName);
        Task DeleteBlobAsync(string pBlobContainerName, string pBlobName);
        Task UndeleteBlobAsync(string pBlobContainerName, string pBlobName);
        Task RenameBlobAsync(string pBlobContainerName, string pBlobName, string pNewBlobName);
        Task CopyBlobAsync(string pBlobContainerName, string pBlobName, string pNewBlobName);
        #endregion
    }
}
