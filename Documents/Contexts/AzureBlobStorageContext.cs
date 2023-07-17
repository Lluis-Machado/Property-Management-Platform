using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.ResourceManager.Storage;
using Azure.Core;
using Azure.ResourceManager;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

namespace DocumentsAPI.Contexts
{
    public class AzureBlobStorageContext
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AzureBlobStorageContext> _logger;
        private readonly string _connectionString;
        public AzureBlobStorageContext(IConfiguration configuration, ILogger<AzureBlobStorageContext> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _connectionString = $"https://{_configuration.GetValue<string>("AzureBlobStorage:StorageAccount")}.blob.core.windows.net";
        }

        public void CheckResponse(Response response)
        {
            if (response.IsError) throw new Exception(response.ReasonPhrase);
        }

        public BlobServiceClient GetBlobServiceClient()
        {
            return new(new Uri(_connectionString), new DefaultAzureCredential());
        }

        public BlobContainerClient GetBlobContainerClient(string blobContainerName)
        {
            BlobServiceClient blobServiceClient = GetBlobServiceClient();
            return blobServiceClient.GetBlobContainerClient(blobContainerName);
        }

        public BlobClient GetBlobClient(string blobContainerName, string blobName)
        {
            Uri blobUri = GetBlobUri(blobContainerName, blobName);
            return new(blobUri, new DefaultAzureCredential());
        }


        public bool IsVersioningEnabled()
        {
            // Assume true if not defined in appsettings
            return _configuration.GetValue<bool?>("AzureSettings:VersioningEnabled") ?? true;
        }

        private Uri GetBlobUri(string blobContainerName, string blobName)
        {
            return new($"{_connectionString}/{blobContainerName}/{blobName}");
        }

    }
}
