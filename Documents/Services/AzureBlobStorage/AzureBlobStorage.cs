using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DocumentsAPI.Contexts;
using DocumentsAPI.Models;
using DocumentsAPI.Repositories;
using System.Collections.Concurrent;
using System.Net;
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

        public async Task UpdateDocumentAsync(Guid archiveId, Guid documentId, Guid? folderId)
        {
            BlobClient blobClient = _context.GetBlobClient(archiveId.ToString(), documentId.ToString());

            var blobProps = await blobClient.GetPropertiesAsync();
            blobProps.Value.Metadata["folder_id"] = folderId != null ? folderId.ToString() : "";
            await blobClient.SetMetadataAsync(blobProps.Value.Metadata);
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

            //_logger.LogInformation($"DEBUG - UploadDocumentAsync - Response was {response.Status} - {Newtonsoft.Json.JsonConvert.SerializeObject(response.Content)}");

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

                    // If FolderID is null, it only returns documents which are in the root of the archive, not every single document regardless of folder!
                    string? currFolderId;
                    blobItem.Metadata.TryGetValue("folder_id", out currFolderId);
                    if ((folderId == null && string.IsNullOrEmpty(currFolderId)) ||
                        currFolderId == folderId.ToString())
                    {
                        Document document = MapDocument(blobItem);
                        documents.Add(document);
                    }
                }
            }
            return documents;
        }

        public async Task<IEnumerable<Document>> SearchDocumentsTagsAsync(string query, bool includeDeleted = false)
        {
            BlobServiceClient blobServiceClient = _context.GetBlobServiceClient();

            // TODO: Change "display_name" for "status" when it gets implemented
            var resultSegment = blobServiceClient.FindBlobsByTagsAsync($"\"display_name\"='{query}'").AsPages();
            // Enumerate the blobs returned for each page.
            var documents = new ConcurrentBag<Document>();
            await foreach (Page<TaggedBlobItem> blobPage in resultSegment)
            {
                var tasks = blobPage.Values.Select(async blobItem =>
                {
                    Document document = await GetDocumentByIdAsync(Guid.Parse(blobItem.BlobContainerName), Guid.Parse(blobItem.BlobName));
                    documents.Add(document);
                });

                await Task.WhenAll(tasks);
            }
            return documents.ToList();

        }

        public async Task<Document?> GetDocumentByIdAsync(Guid archiveId, Guid documentId)
        {
            BlobContainerClient blobContainerClient = _context.GetBlobContainerClient(archiveId.ToString());

            var resultSegment = blobContainerClient.GetBlobsAsync(states: BlobStates.DeletedWithVersions, traits: BlobTraits.Metadata, prefix: documentId.ToString()).AsPages();

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

            Document doc = await GetDocumentByIdAsync(archiveId, documentId);

            _logger.LogInformation($"Current doc ID: {documentId.ToString()} | Name: {doc.Name} | Extension: {doc.Extension} | NewName: {newDocumentName} | Condition: {newDocumentName.EndsWith(doc.Extension)}");

            if (string.IsNullOrEmpty(doc.Extension)) newDocumentName += ".pdf";    // Assume pdf file format if none specified
            else if (!newDocumentName.EndsWith(doc.Extension)) newDocumentName += doc.Extension;

            // destinatio blob metadata
            Dictionary<string, string> blobMetadata = new()
            {
                {"display_name", newDocumentName},
                {"folder_id", blobClient.GetProperties().Value.Metadata["folder_id"]}
            };

            Response<BlobInfo> responseBlobInfo = await blobClient.SetMetadataAsync(blobMetadata);
            Response response = responseBlobInfo.GetRawResponse();
            _logger.LogInformation($"Trying rename of file {documentId.ToString()} to {newDocumentName}\nResponse:{response.Status}");
            _context.CheckResponse(response);
        }


        public async Task<Guid> CopyDocumentAsync(Guid sourceArchive, Guid destinationArchive, Guid documentId, string newDocumentName, Guid? folderId = null)
        {
            // source blob
            BlobClient sourceBlobClient = _context.GetBlobClient(sourceArchive.ToString(), documentId.ToString());

            // destination blob
            Guid copyNewId = Guid.NewGuid();
            BlobClient destinationBlobClient = _context.GetBlobClient(destinationArchive.ToString(), copyNewId.ToString());

            // destinatio blob metadata
            Dictionary<string, string> blobMetadata = new()
            {
                {"display_name", newDocumentName},
                {"folder_id", folderId != null ? folderId.ToString() : ""},
            };

            BlobCopyFromUriOptions blobCopyFromUriOptions = new()
            {
                Metadata = blobMetadata
            };

            // copy
            CopyFromUriOperation copyFromUriOperation = await destinationBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri, blobCopyFromUriOptions);
            await copyFromUriOperation.WaitForCompletionAsync();
            return copyNewId;
        }

        public async Task MoveDocumentAsync(Guid sourceArchive, Guid destinationArchive, Guid documentId, string newDocumentName, Guid? folderId = null)
        {

            // source blob
            BlobClient sourceBlobClient = _context.GetBlobClient(sourceArchive.ToString(), documentId.ToString());

            if (sourceArchive == destinationArchive && folderId != null)
            {
                var test = await GetDocumentByIdAsync(sourceArchive, documentId);
                if (test == null) throw new Exception("Source document not found");

                // No need to check if the folder is the same, since we check it inside the DocumentsController method
                // Not optimal, I know, but it's the only place in which I have access to all document's metadata

                // Since the document is already inside the destination archive, simply update its metadata and return
                await UpdateDocumentAsync(sourceArchive, documentId, folderId);
                return;
            }
            else
            {
                // destination blob
                BlobClient destinationBlobClient = _context.GetBlobClient(destinationArchive.ToString(), documentId.ToString());

                // destination blob metadata
                Dictionary<string, string> blobMetadata = new()
            {
                {"display_name", newDocumentName},
                {"folder_id", folderId != null ? folderId.ToString() : ""},
            };

                BlobCopyFromUriOptions blobCopyFromUriOptions = new()
                {
                    Metadata = blobMetadata
                };

                // copy
                CopyFromUriOperation copyFromUriOperation = await destinationBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri, blobCopyFromUriOptions);
                await copyFromUriOperation.WaitForCompletionAsync();

                // Delete original
                await sourceBlobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);
            }

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
                LastUpdateAt = blobContainerItem.Properties.LastModified.DateTime,

            };
            return archive;
        }

        private static Document MapDocument(BlobItem blobItem)
        {
            bool hasMetadata = blobItem.Metadata.Count == 2;    // TODO: Change number depending on the number of fields present - Currently: 2
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

        //private static Document MapDocument(TaggedBlobItem taggedBlobItem)
        //{
        //    bool hasMetadata = taggedBlobItem.Tags.Count == 2;    // TODO: Change number depending on the number of fields present - Currently: 2
        //    string documentName = hasMetadata ? taggedBlobItem.Tags["display_name"] : "NO NAME";
        //    Guid? folderId = hasMetadata ? (!string.IsNullOrEmpty(taggedBlobItem.Tags["folder_id"]) ? new Guid(taggedBlobItem.Tags["folder_id"]) : null) : null;

        //    Document document = new()
        //    {
        //        Id = Guid.Parse(taggedBlobItem.BlobName),
        //        Name = documentName,
        //        FolderId = folderId,
        //        Extension = documentName.Contains('.') ? documentName[documentName.LastIndexOf('.')..] : "",
        //        ContentLength = blobItem.Properties.ContentLength,
        //        CreatedAt = blobItem.Properties.CreatedOn.GetValueOrDefault().DateTime,
        //        LastUpdateAt = blobItem.Properties.LastModified.GetValueOrDefault().DateTime,
        //        Deleted = blobItem.Deleted || !hasMetadata  // Some blobs marked as deleted on the Azure portal still say they are not deleted
        //    };
        //    return document;
        //}


        #endregion

    }

}
