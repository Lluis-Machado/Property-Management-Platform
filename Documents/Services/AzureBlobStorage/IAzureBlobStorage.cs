using Documents.Models;

namespace Documents.Services.AzureBlobStorage
{
    public interface IAzureBlobStorage
    {
        string ConnectionString { get; set; }

        Task CreateBlobContainerAsync(string pBlobContainerName);
        Task<List<Tenant>> GetBlobContainers(int? pSegmentSize, bool includeDeleted = false);
        Task DeleteBlobContainerAsync(string pBlobContainerName);
        Task UndeleteBlobContainerAsync(string pBlobContainerName);
        Task<string?> UploadAsync(string pBlobContainerName, string pFileName, Stream pFileContent);
        Task<List<Document>> ListBlobsFlatListingAsync(string pBlobContainerName, int? pSegmentSize, bool includeDeleted = false);
        Task<byte[]> DownloadBlobAsync(string pBlobContainerName, string pBlobName);
        Task DeleteBlobAsync(string pBlobContainerName, string pBlobName);
        Task UndeleteBlobAsync(string pBlobContainerName, string pBlobName);
        Task RenameBlobAsync(string pBlobContainerName, string pBlobName, string pNewBlobName);
        Task CopyBlobAsync(string pBlobContainerName, string pBlobName, string pNewBlobName);
    }
}
