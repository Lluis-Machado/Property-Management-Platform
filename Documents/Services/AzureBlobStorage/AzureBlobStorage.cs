using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Documents.Models;
using System.Net;

namespace Documents.Services.AzureBlobStorage
{

    public class AzureBlobStorage: IAzureBlobStorage
    {
        private string ConnectionString { get; set;}

        public AzureBlobStorage(string connectionString)
        {
            ConnectionString = connectionString;
        }

        #region Containers

        public async Task CreateBlobContainerAsync(string pBlobContainerName)
        {
            BlobServiceClient blobServiceClient = GetBlobServiceClient();
            await blobServiceClient.CreateBlobContainerAsync(pBlobContainerName);
        }

        public async Task<IEnumerable<Tenant>> GetBlobContainersAsync(int? segmentSize, bool includeDeleted = false)
        {
            BlobServiceClient blobServiceClient = GetBlobServiceClient();
            BlobContainerStates blobContainerStates = includeDeleted ? BlobContainerStates.Deleted : BlobContainerStates.None;

            var resultSegment = blobServiceClient.GetBlobContainersAsync(default, blobContainerStates).AsPages(default, segmentSize);
            //Enumerate the blobs containers returned for each page.
            List<Tenant> tenants = new();
            await foreach (Page<BlobContainerItem> blobPage in resultSegment)
            {
                foreach (BlobContainerItem blobContainerItem in blobPage.Values)
                {
                    Tenant tenant = MapTenant(blobContainerItem);
                    tenants.Add(tenant);
                }
            }
            return tenants;
        }

        public async Task<bool> BlobContainerExistsAsync(string blobContainerName)
        {
            BlobServiceClient blobServiceClient = GetBlobServiceClient();

            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);

            return await blobContainerClient.ExistsAsync();
        }

        public async Task<bool> DeleteBlobContainerAsync(string blobContainerName)
        {
            BlobServiceClient blobServiceClient = GetBlobServiceClient();

            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);

            if (!await blobContainerClient.ExistsAsync()) return false;

            Response response = await blobContainerClient.DeleteAsync();

            if (response.IsError) throw new Exception(response.ReasonPhrase);

            return true;
        }

        public async Task<bool> UndeleteBlobContainerAsync(string blobContainerName)
        {
            BlobServiceClient blobServiceClient = GetBlobServiceClient();

            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);

            if (!await blobContainerClient.ExistsAsync()) return false;

            Response<BlobContainerClient> blobContainerResponse = await blobServiceClient.UndeleteBlobContainerAsync(blobContainerName, null);
            Response response = blobContainerResponse.GetRawResponse();

            if (response.IsError) throw new Exception(response.ReasonPhrase);

            return true;
        }

        #endregion

        #region Blob 

        public async Task<HttpStatusCode> UploadAsync(string blobContainerName, string blobName, Stream blobContent)
        {
            BlobClient blobClient = GetBlobClient(blobContainerName, $"{Guid.NewGuid()}_{blobName}");

            Response<BlobContentInfo> blobContentInfo = await blobClient.UploadAsync(blobContent);

            Response response = blobContentInfo.GetRawResponse();

            return (HttpStatusCode)response.Status;
        }

        public async Task<IEnumerable<Document>> ListBlobsFlatListingAsync(string blobContainerName, int? segmentSize, bool includeDeleted = false)
        {
            BlobContainerClient blobContainerClient = GetBlobContainerClient(blobContainerName);

            // Call the listing operation and return pages of the specified size.
            BlobStates blobStates = includeDeleted ? BlobStates.Deleted : BlobStates.None;
            var resultSegment = blobContainerClient.GetBlobsAsync(default, blobStates).AsPages(default, segmentSize);

            // Enumerate the blobs returned for each page.
            List<Document> documents = new();
            await foreach (Page<BlobItem> blobPage in resultSegment)
            {
                foreach (BlobItem blobItem in blobPage.Values)
                {
                    Document document = MapDocument(blobItem);
                    documents.Add(document);
                }
            }
            return documents;
        }

        public async Task<bool> BlobExistsAsync(string blobContainerName, string blobName)
        {
            BlobClient blobClient = GetBlobClient(blobContainerName, blobName);
            return await blobClient.ExistsAsync();
        }

        public async Task<byte[]> DownloadBlobAsync(string blobContainerName, string blobName)
        {
            BlobClient blobClient = GetBlobClient(blobContainerName, blobName);

            BlobDownloadResult BlobDownloadResult = await blobClient.DownloadContentAsync();

            return BlobDownloadResult.Content.ToArray();
        }

        public async Task<bool> DeleteBlobAsync(string blobContainerName, string blobName)
        {
            BlobClient blobClient = GetBlobClient(blobContainerName, blobName);

            if (!await blobClient.ExistsAsync()) return false;

            Response response = await blobClient.DeleteAsync();

            if (response.IsError) throw new Exception(response.ReasonPhrase);
            return true;
        }

        public async Task<bool> UndeleteBlobAsync(string blobContainerName, string blobName)
        {
            BlobClient blobClient = GetBlobClient(blobContainerName, blobName);

            if (!await blobClient.ExistsAsync()) return false;

            Response response = await blobClient.UndeleteAsync();

            if (response.IsError) throw new Exception(response.ReasonPhrase);
            return true;
        }

        public async Task<bool> RenameBlobAsync(string blobContainerName, string blobName, string newName)
        {
            // source blob
            BlobClient sourceBlobClient = GetBlobClient(blobContainerName, blobName);

            if (!await sourceBlobClient.ExistsAsync()) return false;

            // destination blob
            DocumentName sourceBlobName = MapDocumentName(blobName);
            string destinationBlobName = $"{sourceBlobName.Code}_{newName}{sourceBlobName.Extension}";
            BlobClient destinationBlobClient = GetBlobClient(blobContainerName, destinationBlobName);

            // copy
            CopyFromUriOperation copyFromUriOperation = await destinationBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);
            await copyFromUriOperation.WaitForCompletionAsync();
  
            // delete
            Response responseDelete = await sourceBlobClient.DeleteAsync();
            if (responseDelete.IsError) throw new Exception(responseDelete.ReasonPhrase);
            return true;
        }


        public async Task<bool> CopyBlobAsync(string blobContainerName, string blobName, string newName)
        {
            // source blob
            BlobClient sourceBlobClient = GetBlobClient(blobContainerName, blobName);

            if (!await sourceBlobClient.ExistsAsync()) return false;

            // destination blob
            DocumentName sourceBlobName = MapDocumentName(blobName);
            string destinationBlobName = $"{Guid.NewGuid()}_{newName}{sourceBlobName.Extension}";
            BlobClient destinationBlobClient = GetBlobClient(blobContainerName, destinationBlobName);

            // copy
            CopyFromUriOperation copyFromUriOperation = await destinationBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);
            await copyFromUriOperation.WaitForCompletionAsync();
            return true;
        }

        #endregion

        #region Helpers

        private BlobServiceClient GetBlobServiceClient()
        {
            return new(new Uri(ConnectionString), new DefaultAzureCredential());
        }

        private BlobContainerClient GetBlobContainerClient(string blobContainerName)
        {
            BlobServiceClient blobServiceClient = GetBlobServiceClient();
            return blobServiceClient.GetBlobContainerClient(blobContainerName);
        }

        private BlobClient GetBlobClient(string blobContainerName, string blobName)
        {
            Uri blobUri = GetBlobUri(blobContainerName, blobName);
            return new(blobUri, new DefaultAzureCredential());
        }

        private Uri GetBlobUri(string blobContainerName, string blobName)
        {
            return new($"{ConnectionString}/{blobContainerName}/{blobName}");
        }

        private static Tenant MapTenant(BlobContainerItem pBlobContainerItem)
        {
            Tenant tenant = new()
            {
                Name = pBlobContainerItem.Name,
            };
            return tenant;
        }

        private static Document MapDocument(BlobItem pBlobItem)
        {
            DocumentName documentName = MapDocumentName(pBlobItem.Name);
            Document document = new()
            {
                Id = pBlobItem.Name,
                Code = documentName.Code,
                Name = documentName.Name,
                Extension = documentName.Extension,
                ContentLength = pBlobItem.Properties.ContentLength,
                CreatedOn = pBlobItem.Properties.CreatedOn,
                LastModified = pBlobItem.Properties.LastModified,

            };
            return document;
        }

        private static DocumentName MapDocumentName(string pBlobName)
        {
            int _idNbOfChars = 36;
            DocumentName documentName = new()
            {
                Code = pBlobName[.._idNbOfChars],
                Name = pBlobName[(_idNbOfChars + 1)..],
                Extension = pBlobName.Contains('.') ? pBlobName[pBlobName.LastIndexOf('.')..] : null,
            };
            return documentName;
        }

        #endregion

    }

}
