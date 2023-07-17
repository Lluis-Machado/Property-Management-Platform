﻿using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DocumentsAPI.Contexts;
using DocumentsAPI.Models;
using DocumentsAPI.Repositories;
using System.Net;
using System.Reflection.Metadata;
using Document = DocumentsAPI.Models.Document;

namespace DocumentsAPI.Services.AzureBlobStorage
{

    public class AzureBlobStorage : IDocumentRepository, IArchiveRepository
    {
        private AzureBlobStorageContext _context { get; set; }
        private static ILogger<AzureBlobStorage> _logger { get; set; }

        public AzureBlobStorage(AzureBlobStorageContext context, ILogger<AzureBlobStorage> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Archives

        public async Task CreateArchiveAsync(Archive archive)
        {
            Dictionary<string, string> metadata = new();

            if (archive.Name != null) metadata.Add("display_name", archive.Name);

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

        public async Task UpdateArchiveAsync(Guid archiveId, string newName)
        {
            BlobContainerClient blobContainerClient = _context.GetBlobContainerClient(archiveId.ToString());

            BlobContainerProperties props = await blobContainerClient.GetPropertiesAsync();
            var metadata = props.Metadata;
            metadata["display_name"] = newName;

            var response = await blobContainerClient.SetMetadataAsync(metadata);
            _context.CheckResponse(response.GetRawResponse());
        }

        public async Task UndeleteArchiveAsync(Guid archiveId)
        {

            BlobServiceClient blobServiceClient = _context.GetBlobServiceClient();

            await foreach (BlobContainerItem item in blobServiceClient.GetBlobContainersAsync(BlobContainerTraits.Metadata, BlobContainerStates.Deleted))
            {
                if (item.Name == archiveId.ToString() && (item.IsDeleted == true))
                {
                    await blobServiceClient.UndeleteBlobContainerAsync(archiveId.ToString(), item.VersionId);
                    return;
                }
            }
        }

        #endregion

        #region Documents 

        public async Task<HttpStatusCode> UploadDocumentAsync(Guid archiveId, string fileName, Stream fileContent, Guid? folderId = null)
        {
            BlobClient blobClient = _context.GetBlobClient(archiveId.ToString(), Guid.NewGuid().ToString());

            Dictionary<string, string> blobMetadata = new()
            {
                {"display_name", fileName},
                {"folder_id", folderId.ToString() ?? ""},
            };

            BlobUploadOptions blobUploadOptions = new()
            {
                Metadata = blobMetadata
            };

            Response<BlobContentInfo> blobContentInfo = await blobClient.UploadAsync(fileContent, blobUploadOptions);

            Response response = blobContentInfo.GetRawResponse();

            return (HttpStatusCode)response.Status;
        }

        public async Task<IEnumerable<Document>> GetDocumentsFlatListingAsync(Guid archiveId, int? segmentSize, Guid? folderId = null, bool includeDeleted = false)
        {
            BlobContainerClient blobContainerClient = _context.GetBlobContainerClient(archiveId.ToString());

            // Call the listing operation and return pages of the specified size.
            BlobStates blobStates = includeDeleted ? BlobStates.DeletedWithVersions : BlobStates.None;
            var resultSegment = blobContainerClient.GetBlobsAsync(states: blobStates, traits: BlobTraits.Metadata).AsPages(default, segmentSize);

            // Enumerate the blobs returned for each page.
            List<Document> documents = new();
            await foreach (Page<BlobItem> blobPage in resultSegment)
            {
                foreach (BlobItem blobItem in blobPage.Values)
                {
                    // TODO: This is currently filtering after getting all the blobs. Ideally, we should filter in the Azure query.
                    // It can be done by configuring Azure Search, but I ain't got time for that now
                    if (folderId != null && blobItem.Metadata["folder_id"] != folderId.ToString())
                        continue;
                    Document document = MapDocument(blobItem);
                    documents.Add(document);
                }
            }
            return documents;
        }

        public async Task<Document?> GetDocumentByIdAsync(Guid archiveId, Guid documentId)
        {
            BlobContainerClient blobContainerClient = _context.GetBlobContainerClient(archiveId.ToString());

            var resultSegment = blobContainerClient.GetBlobsAsync(states: BlobStates.DeletedWithVersions, traits: BlobTraits.Metadata).AsPages();

            Document? document = null;

            await foreach (Page<BlobItem> blobPage in resultSegment)
            {
                document = MapDocument(blobPage.Values[0]);
                break;
            }

            return document;
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
            //BlobClient blobClient = _context.GetBlobClient(archiveId.ToString(), documentId.ToString());
            //Response response = await blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);

            BlobContainerClient blobContainerClient = _context.GetBlobContainerClient(archiveId.ToString());
            Response response = await blobContainerClient.DeleteBlobAsync(documentId.ToString(), DeleteSnapshotsOption.IncludeSnapshots);

            if (response.IsError) throw new Exception(response.ReasonPhrase);
        }

        public async Task UndeleteDocumentAsync(Guid archiveId, Guid documentId)
        {
            _logger.LogInformation($"Performing undelete operation | Archive: {archiveId} | Document: {documentId}");

            BlobClient blobClient = _context.GetBlobClient(archiveId.ToString(), documentId.ToString());

            // Check if Blob Container has versioning enabled
            bool versioning = _context.IsVersioningEnabled();

            if (versioning)
            {
                BlobContainerClient container = _context.GetBlobContainerClient(archiveId.ToString());
                
                // List blobs in this container that match prefix.
                // Include versions in listing.
                Pageable<BlobItem> blobItems = container.GetBlobs
                                (BlobTraits.None, BlobStates.Version, prefix: documentId.ToString());

                // Get the URI for the most recent version.
                BlobUriBuilder blobVersionUri = new BlobUriBuilder(blobClient.Uri)
                {
                    VersionId = blobItems
                                .OrderByDescending(version => version.VersionId)
                                .ElementAtOrDefault(0)?.VersionId
                };

                // Restore the most recently generated version by copying it to the base blob.
                Response response = (await blobClient.StartCopyFromUriAsync(blobVersionUri.ToUri())).GetRawResponse();
                if (response.IsError) throw new Exception(response.ReasonPhrase);
                _logger.LogInformation($"Undelete operation for document {documentId} (with versioning) successful!");
            }
            else
            {
                Response response = await blobClient.UndeleteAsync();
                if (response.IsError) throw new Exception(response.ReasonPhrase);
                _logger.LogInformation($"Undelete operation for document {documentId} successful!");
            }
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


        public async Task CopyDocumentAsync(Guid archiveId, Guid documentId, string newDocumentName, Guid? folderId = null)
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
            if (folderId != null) blobMetadata.Add("folderId", folderId.ToString());

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
                Name = archiveDisplayName,
                Deleted = blobContainerItem.IsDeleted ?? false,
                LastUpdateAt = blobContainerItem.Properties.LastModified.DateTime
            };
            return archive;
        }

        private static Document MapDocument(BlobItem blobItem)
        {
            bool hasMetadata = blobItem.Metadata.Count > 0;
            string documentName = hasMetadata ? blobItem.Metadata["display_name"] : "NO NAME";
            Guid? folderId = hasMetadata ? (!string.IsNullOrEmpty(blobItem.Metadata["folder_id"]) ? new Guid(blobItem.Metadata["folder_id"]) : null) : null;

            Document document = new()
            {
                Id = Guid.Parse(blobItem.Name),
                Name = documentName,
                FolderId = folderId,
                Extension = documentName.Contains('.') ? documentName[documentName.LastIndexOf('.')..] : "",
                ContentLength = blobItem.Properties.ContentLength,
                CreatedAt = blobItem.Properties.CreatedOn.GetValueOrDefault().DateTime,
                LastUpdateAt = blobItem.Properties.LastModified.GetValueOrDefault().DateTime,
                Deleted = blobItem.Deleted || !hasMetadata  // Some blobs marked as deleted on the Azure portal still say they are not deleted
            };
            return document;
        }
        #endregion

    }

}
