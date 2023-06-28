﻿using Azure;
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
            _logger.LogInformation($"Azure Connection string: {_connectionString}");
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

        private Uri GetBlobUri(string blobContainerName, string blobName)
        {
            return new($"{_connectionString}/{blobContainerName}/{blobName}");
        }

    }
}
