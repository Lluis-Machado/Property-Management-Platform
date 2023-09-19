using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;

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


        public string GetStorageAccount()
        {
            return _configuration.GetValue<string>("AzureBlobStorage:StorageAccount");
        }

        public string GetAccountKey()
        {
            var account = GetStorageAccount();
            string connKey = "";

            switch (account)
            {
                case "plattesdevelopment": connKey = "vy3Cr6H7MLPxd1pNKKiC3Q/TTT8rGrcKn4tSggmyj5AM44crG+8+FOUq3usXj7itqei4zyGi7OzV+AStEqLXGw=="; break;
                case "plattesstage": connKey = "Kq8pWJRl25kL1S4uUKzW3Nhey4K1y2eSIaibidLUEY563AQp75lPaarf37DvoPx/MtkoXzBe50P6+AStPMnCGQ=="; break;
                case "plattesproduction": throw new NotImplementedException("PENDING CREATION OF PRODUCTION STORAGE ACCOUNT");
            }

            if (string.IsNullOrEmpty(connKey)) throw new Exception("Could not retrieve Blob Storage account key");

            return connKey;

        }


        private Uri GetBlobUri(string blobContainerName, string blobName)
        {
            return new($"{_connectionString}/{blobContainerName}/{blobName}");
        }

    }
}
