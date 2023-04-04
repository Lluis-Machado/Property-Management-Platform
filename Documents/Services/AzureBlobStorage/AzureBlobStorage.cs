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
        public static readonly int _idNbOfChars = 36;

        private readonly IConfiguration _config;
        public string ConnectionString { get; set;}

        public AzureBlobStorage(IConfiguration config)
        {
            _config = config;
            ConnectionString = $"https://{_config.GetValue<string>("AzureBlobStorage:StorageAccount")}.blob.core.windows.net" ;
        }

        public async Task CreateBlobContainerAsync(string pBlobContainerName)
        {
            BlobServiceClient blobServiceClient = new(new Uri(ConnectionString), new DefaultAzureCredential());
            await blobServiceClient.CreateBlobContainerAsync(pBlobContainerName);
        }

        public async Task<List<Tenant>> GetBlobContainers(int? pSegmentSize, bool includeDeleted = false)
        {
            List<Tenant> tenants = new List<Tenant>();
            BlobServiceClient blobServiceClient = new(new Uri(ConnectionString), new DefaultAzureCredential());
            BlobContainerStates blobContainerStates = includeDeleted ? BlobContainerStates.Deleted : BlobContainerStates.None;
            var resultSegment =  blobServiceClient.GetBlobContainersAsync(default, blobContainerStates).AsPages(default, pSegmentSize);

            //Enumerate the blobs containers returned for each page.
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

        public async Task DeleteBlobContainerAsync(string pBlobContainerName)
        {
            BlobServiceClient blobServiceClient = new(new Uri(ConnectionString), new DefaultAzureCredential());

            Response response = await blobServiceClient.DeleteBlobContainerAsync(pBlobContainerName);

            if (response.IsError) throw new Exception(response.ReasonPhrase);
        }

        public async Task UndeleteBlobContainerAsync(string pBlobContainerName)
        {
            BlobServiceClient blobServiceClient = new(new Uri(ConnectionString), new DefaultAzureCredential());

            Response<BlobContainerClient> blobContainerResponse = await blobServiceClient.UndeleteBlobContainerAsync(pBlobContainerName,null); //ToDO
            Response response = blobContainerResponse.GetRawResponse();

            if (response.IsError) throw new Exception(response.ReasonPhrase);
        }


        public async Task<string?> UploadAsync(string pBlobContainerName, string pBlobName, Stream pBlobContent)
        {

            BlobServiceClient blobServiceClient = new(new Uri(ConnectionString), new DefaultAzureCredential());

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(pBlobContainerName);

            string blobName = $"{Guid.NewGuid()}_{pBlobName}";

            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            Response<BlobContentInfo> blobContentInfo = await blobClient.UploadAsync(pBlobContent);

            Response response = blobContentInfo.GetRawResponse();

            if (response.IsError) return null;

            return blobName;
        }

        public async Task<List<Document>> ListBlobsFlatListingAsync(string pBlobContainerName, int? pSegmentSize, bool includeDeleted = false)
        {
            List<Document> documents = new();
            BlobServiceClient blobServiceClient = new(new Uri(ConnectionString), new DefaultAzureCredential());

            BlobStates blobStates = includeDeleted ? BlobStates.Deleted : BlobStates.None;
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(pBlobContainerName);

            // Call the listing operation and return pages of the specified size.
            var resultSegment = containerClient.GetBlobsAsync(default, blobStates).AsPages(default, pSegmentSize);

            // Enumerate the blobs returned for each page.
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

        public async Task<byte[]> DownloadBlobAsync(string pBlobContainerName, string pBlobName)
        {
            Uri blobUri = GetBlobUri(pBlobContainerName, pBlobName);
            BlobClient blobClient = new(blobUri, new DefaultAzureCredential());

            BlobDownloadResult BlobDownloadResult = await blobClient.DownloadContentAsync();

            return BlobDownloadResult.Content.ToArray();
        }

        public async Task DeleteBlobAsync(string pBlobContainerName, string pBlobName)
        {
            Uri blobUri = GetBlobUri(pBlobContainerName, pBlobName);
            BlobClient blobClient = new(blobUri, new DefaultAzureCredential());

            Response response = await blobClient.DeleteAsync();

            if (response.IsError) throw new Exception(response.ReasonPhrase);
        }

        public async Task UndeleteBlobAsync(string pBlobContainerName, string pBlobName)
        {
            Uri blobUri = GetBlobUri(pBlobContainerName, pBlobName);
            BlobClient blobClient = new(blobUri, new DefaultAzureCredential());

            Response response = await blobClient.UndeleteAsync();

            if (response.IsError) throw new Exception(response.ReasonPhrase);
        }

        public async Task RenameBlobAsync(string pBlobContainerName, string pBlobName, string pNewName)
        {
            DocumentName sourceDocumentName = MapDocumentName(pBlobName);
            Uri sourceBlobUri = GetBlobUri(pBlobContainerName, pBlobName);

            string destinationBlobName = $"{sourceDocumentName.Code}_{pNewName}{sourceDocumentName.Extension}";
            Uri destinationBlobUri = GetBlobUri(pBlobContainerName, destinationBlobName);

            BlobClient destinationBlobClient = new(destinationBlobUri, new DefaultAzureCredential());

            // copy
            CopyFromUriOperation copyFromUriOperation = await destinationBlobClient.StartCopyFromUriAsync(sourceBlobUri);
            await copyFromUriOperation.WaitForCompletionAsync();
  
            // delete
            BlobClient sourceBlobClient = new(sourceBlobUri, new DefaultAzureCredential());
            Response responseDelete = await sourceBlobClient.DeleteAsync();
            if (responseDelete.IsError) throw new Exception(responseDelete.ReasonPhrase);
        }


        public async Task CopyBlobAsync(string pBlobContainerName, string pBlobName, string pNewName)
        {
            DocumentName sourceDocumentName = MapDocumentName(pBlobName);
            Uri sourceBlobUri = GetBlobUri(pBlobContainerName, pBlobName);

            string destinationBlobName = $"{Guid.NewGuid()}_{pNewName}{sourceDocumentName.Extension}";
            Uri destinationBlobUri = GetBlobUri(pBlobContainerName, destinationBlobName);

            BlobClient destinationBlobClient = new(destinationBlobUri, new DefaultAzureCredential());

            // copy
            CopyFromUriOperation copyFromUriOperation = await destinationBlobClient.StartCopyFromUriAsync(sourceBlobUri);
            await copyFromUriOperation.WaitForCompletionAsync();
        }

        private Uri GetBlobUri(string pBlobContainerName, string pBlobName)
        {
            return new Uri($"{ConnectionString}/{pBlobContainerName}/{pBlobName}");
        }

        private Tenant MapTenant(BlobContainerItem pBlobContainerItem)
        {
            Tenant tenant = new()
            {
                Name = pBlobContainerItem.Name,
            };
            return tenant;
        }

        private Document MapDocument(BlobItem pBlobItem)
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

        private DocumentName MapDocumentName(string pBlobName)
        {
            DocumentName documentName = new()
            {
                Code = pBlobName[.._idNbOfChars],
                Name = pBlobName[(_idNbOfChars + 1)..],
                Extension = pBlobName.Contains('.') ? pBlobName[pBlobName.LastIndexOf('.')..] : null,
            };
            return documentName;
        }

    }

}
