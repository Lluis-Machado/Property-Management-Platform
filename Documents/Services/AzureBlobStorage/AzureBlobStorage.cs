using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Documents.Models;
using DocumentsAPI.Contexts;
using DocumentsAPI.Repositories;
using System.Net;

namespace Documents.Services.AzureBlobStorage
{

    public class AzureBlobStorage: IDocumentRepository, IArchiveRepository
    {
        private AzureBlobStorageContext _context { get; set;}

        public AzureBlobStorage(AzureBlobStorageContext context)
        {
            _context = context;
        }

        #region Archives

        public async Task CreateArchiveAsync(Archive archive)
        {
            Dictionary<string, string> metadata = new();

            if(archive.Name != null) metadata.Add("display_name", archive.Name);

            BlobContainerClient blobContainerClient = _context.GetBlobContainerClient(archive.Id.ToString());
            await blobContainerClient.CreateAsync(default, metadata);
        }

        public async Task<IEnumerable<Archive>> GetArchivesAsync(int? segmentSize, bool includeDeleted = false)
        {
            BlobServiceClient blobServiceClient = _context.GetBlobServiceClient();
            BlobContainerStates blobContainerStates = includeDeleted ? BlobContainerStates.Deleted : BlobContainerStates.None;

            var resultSegment = blobServiceClient.GetBlobContainersAsync(BlobContainerTraits.Metadata, blobContainerStates).AsPages(default, segmentSize);
            //Enumerate the blobs containers returned for each page.
            List<Archive> archives = new();
            await foreach (Page<BlobContainerItem> blobPage in resultSegment)
            {
                foreach (BlobContainerItem blobContainerItem in blobPage.Values)
                {
                    Archive archive = MapArchive(blobContainerItem);
                    archives.Add(archive);
                }
            }
            return archives;
        }

        public async Task<bool> ArchiveExistsAsync(Guid archiveId)
        {
            BlobContainerClient blobContainerClient = _context.GetBlobContainerClient(archiveId.ToString());

            return await blobContainerClient.ExistsAsync();
        }

        public async Task DeleteArchiveAsync(Guid archiveId)
        {

            BlobContainerClient blobContainerClient = _context.GetBlobContainerClient(archiveId.ToString());

            Response response = await blobContainerClient.DeleteAsync();

            _context.CheckResponse(response);
        }

        public async Task UndeleteArchiveAsync(Guid archiveId)
        {
            BlobServiceClient blobServiceClient = _context.GetBlobServiceClient();

            Response<BlobContainerClient> blobContainerResponse = await blobServiceClient.UndeleteBlobContainerAsync(archiveId.ToString(), null);
            Response response = blobContainerResponse.GetRawResponse();

            _context.CheckResponse(response);
        }

        #endregion

        #region Documents 

        public async Task<HttpStatusCode> UploadDocumentAsync(Guid archiveId, string fileName, Stream fileContent, Guid? folderId = null)
        {
            BlobClient blobClient = _context.GetBlobClient(archiveId.ToString(), Guid.NewGuid().ToString());

            Dictionary<string, string> blobMetadata = new()
            {
                {"display_name", fileName},
                {"folder_id" , folderId.ToString()}
            };

            BlobUploadOptions blobUploadOptions = new()
            {
                Metadata = blobMetadata
            };

            Response<BlobContentInfo> blobContentInfo = await blobClient.UploadAsync(fileContent, blobUploadOptions);

            Response response = blobContentInfo.GetRawResponse();

            return (HttpStatusCode)response.Status;
        }

        public async Task<IEnumerable<Document>> GetDocumentsFlatListingAsync(Guid archiveId, int? segmentSize, bool includeDeleted = false)
        {
            BlobContainerClient blobContainerClient = _context.GetBlobContainerClient(archiveId.ToString());

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

        public async Task<bool> DocumentExistsAsync(Guid archiveId, Guid documentId)
        {
            BlobClient blobClient = _context.GetBlobClient(archiveId.ToString(), documentId.ToString());
            return await blobClient.ExistsAsync();
        }

        public async Task<byte[]> DownloadDocumentAsync(Guid archiveId, Guid documentId)
        {
            BlobClient blobClient = _context.GetBlobClient(archiveId.ToString(), documentId.ToString());

            Response<BlobDownloadResult> blobDownloadResponse = await blobClient.DownloadContentAsync();
            Response response = blobDownloadResponse.GetRawResponse();
            _context.CheckResponse(response);
            return blobDownloadResponse.Value.Content.ToArray();
        }

        public async Task DeleteDocumentAsync(Guid archiveId, Guid documentId)
        {
            BlobClient blobClient = _context.GetBlobClient(archiveId.ToString(), documentId.ToString());

            Response response = await blobClient.DeleteAsync();
            if (response.IsError) throw new Exception(response.ReasonPhrase);
        }

        public async Task UndeleteDocumentAsync(Guid archiveId, Guid documentId)
        {
            BlobClient blobClient = _context.GetBlobClient(archiveId.ToString(), documentId.ToString());

            Response response = await blobClient.UndeleteAsync();
            if (response.IsError) throw new Exception(response.ReasonPhrase);
        }

        public async Task RenameDocumentAsync(Guid archiveId, Guid documentId, string newDocumentName)
        {
            // source blob
            BlobClient blobClient = _context.GetBlobClient(archiveId.ToString(), documentId.ToString());

            // destinatio blob metadata
            Dictionary<string, string> blobMetadata = new()
            {
                {"display_name", newDocumentName},
            };
  
            // delete
            Response<BlobInfo> responseBlobInfo = await blobClient.SetMetadataAsync(blobMetadata);
            Response response = responseBlobInfo.GetRawResponse();
            _context.CheckResponse(response);
        }


        public async Task CopyDocumentAsync(Guid archiveId, Guid documentId, string newDocumentName)
        {
            // source blob
            BlobClient sourceBlobClient = _context.GetBlobClient(archiveId.ToString(), documentId.ToString());

            // destination blob
            BlobClient destinationBlobClient = _context.GetBlobClient(archiveId.ToString(), Guid.NewGuid().ToString());

            // destinatio blob metadata
            Dictionary<string, string> blobMetadata = new()
            {
                {"display_name", newDocumentName},
            };

            BlobCopyFromUriOptions blobCopyFromUriOptions = new()
            {
                Metadata = blobMetadata
            };

            // copy
            CopyFromUriOperation copyFromUriOperation = await destinationBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri, blobCopyFromUriOptions);
            await copyFromUriOperation.WaitForCompletionAsync();
        }
        #endregion

        #region Helpers
        private static Archive MapArchive(BlobContainerItem blobContainerItem)
        {
            string? archiveDisplayName = null;
            if (blobContainerItem.Properties.Metadata != null)
            {
                archiveDisplayName = blobContainerItem.Properties.Metadata["display_name"];
            }

            Archive archive = new()
            {
                Id = Guid.Parse(blobContainerItem.Name),
                Name = archiveDisplayName
            };
            return archive;
        }

        private static Document MapDocument(BlobItem blobItem)
        {
            string documentName = blobItem.Metadata["display_Name"];
            Document document = new()
            {
                Id = blobItem.Name,
                Name = documentName,
                Extension = documentName.Contains('.') ? documentName[documentName.LastIndexOf('.')..] : "",
                ContentLength = blobItem.Properties.ContentLength,
                CreatedOn = blobItem.Properties.CreatedOn,
                LastModified = blobItem.Properties.LastModified,

            };
            return document;
        }
        #endregion

    }

}
