﻿using DocumentsAPI.DTOs;
using DocumentsAPI.Models;
using DocumentsAPI.Repositories;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using Microsoft.AspNetCore.Mvc;
using MimeDetective;
using System.Net;

namespace DocumentsAPI.Services
{
    public class DocumentsService : IDocumentsService
    {
        private readonly IConfiguration _config;
        private readonly IDocumentRepository _documentsRepository;
        private readonly IBlobMetadataRepository _blobMetadataRepository;
        private readonly ILogger<DocumentsService> _logger;

        public DocumentsService(IConfiguration config, IDocumentRepository documentsRepository, ILogger<DocumentsService> logger, IBlobMetadataRepository blobMetadataRepository)
        {
            _config = config;
            _documentsRepository = documentsRepository;
            _blobMetadataRepository = blobMetadataRepository;
            _logger = logger;
        }

        public async Task<List<CreateDocumentStatus>> UploadAsync(Guid archiveId, IFormFile[] files, Guid? folderId = null)
        {

            //_logger.LogInformation($"Documents - Starting upload of {files.Length} documents to archive {archiveId} - FolderID: {folderId}");

            var documents = new List<CreateDocumentStatus>();

            await Parallel.ForEachAsync(files, parallelOptions: new ParallelOptions { MaxDegreeOfParallelism = 4 }, async (file, CancellationToken) =>
            {
                Stream fileStream = file.OpenReadStream();
                fileStream.Position = 0;
                var uploadedDoc = await _documentsRepository.UploadDocumentAsync(archiveId, file.FileName, fileStream, file.ContentType, folderId);
                fileStream.Flush();
                documents.Add(new CreateDocumentStatus(file.FileName, (int)uploadedDoc.statusCode));
                fileStream.Dispose();
                //_logger.LogInformation($"**Documents - Uploaded file {file.FileName} - Status: {(int)status}");
                // Add metadata
                await _blobMetadataRepository.InsertAsync(new BlobMetadata() { BlobId = uploadedDoc.documentId, ContainerId = archiveId, DisplayName = file.FileName, FolderId = folderId });
            });

            _logger.LogInformation($"Documents - Upload completed, {documents.FindAll(d => d.Status == (int)HttpStatusCode.Created).Count}/{files.Length} files OK");


            return documents;

        }

        public async Task<IEnumerable<Document>> GetDocumentsAsync(Guid archiveId, int? pSize = 100, Guid? folderId = null, bool includeDeleted = false)
        {
            return await _documentsRepository.GetDocumentsFlatListingAsync(archiveId, pSize, folderId, includeDeleted);
        }

        public async Task<IEnumerable<Document>> SearchDocumentsTagsAsync(string query, bool includeDeleted = false)
        {
            return await _documentsRepository.SearchDocumentsTagsAsync(query, includeDeleted);
        }

        public async Task<Document?> GetDocumentByIdAsync(Guid archiveId, Guid documentId)
        {
            return await _documentsRepository.GetDocumentByIdAsync(archiveId, documentId);
        }

        public async Task<string?> GetDocumentUrlByIdAsync(Guid archiveId, Guid documentId)
        {
            return await _documentsRepository.GetDocumentUrlByIdAsync(archiveId, documentId);
        }

        //public async Task<FileContentResult> DownloadAsync(Guid archiveId, Guid documentId)
        //{
        //    var file = await _documentsRepository.DownloadDocumentRawAsync(archiveId, documentId);

        //    _logger.LogInformation($"ContentType: {file.Value.Details.ContentType}");

        //    return new FileContentResult(file.Value.Content.ToArray(), file.Value.Details.ContentType ?? "application/pdf");
        //}

        public async Task<FileContentResult> DownloadAsync(Guid archiveId, Guid documentId)
        {
            byte[] byteArray = await _documentsRepository.DownloadDocumentAsync(archiveId, documentId);

            var Inspector = new ContentInspectorBuilder()
            {
                Definitions = new MimeDetective.Definitions.CondensedBuilder()
                {
                    UsageType = MimeDetective.Definitions.Licensing.UsageType.PersonalNonCommercial
                }.Build()
            }.Build();

            var results = Inspector.Inspect(byteArray).ByMimeType();

            return new FileContentResult(byteArray, results.MaxBy(e => e.MimeType.Length)?.MimeType?.ToString() ?? "application/octet-stream");
        }

        public async Task<IActionResult> DeleteAsync(Guid archiveId, Guid documentId)
        {
            Task.WaitAll(new Task[] {
                _documentsRepository.DeleteDocumentAsync(archiveId, documentId),
                _blobMetadataRepository.DeleteAsync(documentId)
            });
            return new NoContentResult();
        }

        public async Task<IActionResult> UndeleteAsync(Guid archiveId, Guid documentId)
        {
            Task.WaitAll(new Task[] {
                _documentsRepository.UndeleteDocumentAsync(archiveId, documentId),
                _blobMetadataRepository.UndeleteAsync(documentId)
            });
            return new NoContentResult();
        }

        public async Task<IActionResult> RenameAsync(Guid archiveId, Guid documentId, string documentName)
        {
            Task.WaitAll(new Task[] {
                _documentsRepository.RenameDocumentAsync(archiveId, documentId, documentName),
                _blobMetadataRepository.UpdateAsync(new BlobMetadata() { BlobId = documentId, ContainerId = archiveId, DisplayName = documentName })
            });
            return new NoContentResult();
        }

        public async Task<Guid> CopyAsync(Guid sourceArchive, Guid destinationArchive, Guid documentId, string documentName, Guid? folderId = null)
        {
            var newDocGuid = await _documentsRepository.CopyDocumentAsync(sourceArchive, destinationArchive, documentId, documentName, folderId);
            await _blobMetadataRepository.InsertAsync(new BlobMetadata() { BlobId = newDocGuid, ContainerId = destinationArchive, DisplayName = documentName, FolderId = folderId });
            return newDocGuid;
        }

        public async Task<IActionResult> MoveAsync(Guid sourceArchive, Guid destinationArchive, Guid documentId, string documentName, Guid? folderId = null)
        {
            await _documentsRepository.MoveDocumentAsync(sourceArchive, destinationArchive, documentId, documentName, folderId);
            await _blobMetadataRepository.UpdateAsync(new BlobMetadata() { BlobId = documentId, ContainerId = destinationArchive, DisplayName = documentName, FolderId = folderId });
            return new NoContentResult();
        }

        public async Task<IEnumerable<BlobMetadata>> SearchMetadataAsync(string? displayName, Guid? folderId = null, Guid? containerId = null, bool includeDeleted = false)
        {
            return await _blobMetadataRepository.SearchMetadata(displayName, folderId, containerId, includeDeleted);
        }

        [Obsolete("Use SplitBlobAsync instead to directly split an uploaded blob PDF file")]
        public async Task<IEnumerable<FileContentResult>> SplitAsync(IFormFile file, DocSplitInterval[]? pageRanges)
        {

            List<byte[]> pdfByteArrays = new();
            List<FileContentResult> res = new();

            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            memoryStream.Position = 0;

            // Create a MemoryStream from the original PDF byte array
            {
                // Open the original PDF document
                using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(memoryStream)))
                {
                    if (pdfDocument.GetNumberOfPages() == 1) throw new Exception("Cannot split a 1-page document");

                    if (pageRanges == null || pageRanges.Length == 0)
                    {
                        // If the pageRanges array is null or empty, split each page
                        for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                        {

                            // Create a new MemoryStream to store the split PDF content
                            using (MemoryStream splitMemoryStream = new MemoryStream())
                            {
                                // Create a new PdfDocument for each page
                                using (PdfDocument splitPdfDocument = new PdfDocument(new PdfWriter(splitMemoryStream)))
                                {
                                    //splitMemoryStream.Position = 0;

                                    // Copy the current page to the new PdfDocument
                                    pdfDocument.CopyPagesTo(i, i, splitPdfDocument);

                                    splitMemoryStream.Position = 0;

                                    splitPdfDocument.Close();

                                    // Save the content to a byte array
                                    pdfByteArrays.Add(splitMemoryStream.ToArray());
                                }
                            }
                        }
                    }
                    else
                    {
                        // If the pageRanges array is not null, split according to the specified ranges
                        foreach (DocSplitInterval range in pageRanges)
                        {
                            // Ensure the range is within valid bounds
                            int start = Math.Max(1, range.start);
                            int end = Math.Min(pdfDocument.GetNumberOfPages(), range.end);

                            if (start <= end)
                            {
                                // Create a new MemoryStream to store the split PDF content
                                using (MemoryStream splitMemoryStream = new MemoryStream())
                                {
                                    // Create a new PdfDocument for the current range
                                    using (PdfDocument splitPdfDocument = new PdfDocument(new PdfWriter(splitMemoryStream)))
                                    {
                                        // Copy the specified range to the new PdfDocument
                                        pdfDocument.CopyPagesTo(start, end, splitPdfDocument);

                                        splitPdfDocument.Close();

                                        // Save the content to a byte array
                                        pdfByteArrays.Add(splitMemoryStream.ToArray());
                                    }
                                }
                            }
                        }
                    }
                }

            }

            for (int i = 0; i < pdfByteArrays.Count; i++)
            {
                res.Add(new FileContentResult(pdfByteArrays[i], "application/pdf") { FileDownloadName = $"{file.FileName}_{i + 1}" });
            }

            return res;

        }

        public async Task<IEnumerable<string>> SplitBlobAsync(Guid archiveId, Guid documentId, DocSplitInterval[]? pageRanges)
        {
            var document = _documentsRepository.GetDocumentByIdAsync(archiveId, documentId);
            var docBytes = _documentsRepository.DownloadDocumentAsync(archiveId, documentId);

            Task.WaitAll(new Task[] { document, docBytes });

            MemoryStream memoryStream = new MemoryStream(await docBytes);
            memoryStream.Position = 0;

            List<string> docIds = new();

            // Create a MemoryStream from the original PDF byte array
            {
                // Open the original PDF document
                using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(memoryStream)))
                {

                    if (pdfDocument.GetNumberOfPages() == 1) throw new Exception("Cannot split a 1-page document");

                    if (pageRanges == null || pageRanges.Length == 0)
                    {
                        // If the pageRanges array is null or empty, split each page
                        for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                        {
                            // Create a new MemoryStream to store the split PDF content
                            using (MemoryStream splitMemoryStream = new MemoryStream())
                            {

                                // Create a new PdfDocument for each page
                                using (PdfDocument splitPdfDocument = new PdfDocument(new PdfWriter(splitMemoryStream)))
                                {
                                    // Copy the current page to the new PdfDocument
                                    pdfDocument.CopyPagesTo(i, i, splitPdfDocument);

                                    splitPdfDocument.Close();

                                    byte[] splitPdf = splitMemoryStream.ToArray();

                                    // Upload file
                                    string docName = (await document)!.Name!.Replace(".pdf", "", StringComparison.InvariantCultureIgnoreCase);
                                    Guid? folderId = (await document)!.FolderId;

                                    var addedDoc = await _documentsRepository.UploadDocumentAsync(archiveId, $"{docName}_[{i}].pdf", splitPdf, "application/pdf", folderId);

                                    await _blobMetadataRepository.InsertAsync(new BlobMetadata() { BlobId = addedDoc.documentId, ContainerId = archiveId, DisplayName = $"{docName}_[{i}].pdf", FolderId = folderId });

                                    docIds.Add(addedDoc.documentId.ToString());
                                }

                            }
                        }
                    }
                    else
                    {
                        // If the pageRanges array is not null, split according to the specified ranges
                        foreach (DocSplitInterval range in pageRanges)
                        {
                            // Ensure the range is within valid bounds
                            int start = Math.Max(1, range.start);
                            int end = Math.Min(pdfDocument.GetNumberOfPages(), range.end);

                            if (start <= end)
                            {
                                // Create a new MemoryStream to store the split PDF content
                                using (MemoryStream splitMemoryStream = new MemoryStream())
                                {
                                    // Create a new PdfDocument for the current range
                                    using (PdfDocument splitPdfDocument = new PdfDocument(new PdfWriter(splitMemoryStream)))
                                    {
                                        // Copy the current page to the new PdfDocument
                                        pdfDocument.CopyPagesTo(start, end, splitPdfDocument);

                                        splitPdfDocument.Close();

                                        byte[] splitPdf = splitMemoryStream.ToArray();

                                        // Upload file
                                        string docName = (await document)!.Name!.Replace(".pdf", "", StringComparison.InvariantCultureIgnoreCase);
                                        Guid? folderId = (await document)!.FolderId;

                                        var addedDoc = await _documentsRepository.UploadDocumentAsync(archiveId, $"{docName}_[{start}-{end}].pdf", splitPdf, "application/pdf", folderId);

                                        await _blobMetadataRepository.InsertAsync(new BlobMetadata() { BlobId = addedDoc.documentId, ContainerId = archiveId, DisplayName = $"{docName}_[{start}-{end}].pdf", FolderId = folderId });

                                        docIds.Add(addedDoc.documentId.ToString());
                                    }
                                }
                            }
                        }
                    }
                }

            }

            return docIds;

        }

        public async Task<string> JoinBlobsAsync(Guid archiveId, string[] documentIds)
        {
            string joinedDocId = "";
            Dictionary<Document, byte[]> docsKeyVals = new();

            foreach (var docId in documentIds)
            {
                var document = _documentsRepository.GetDocumentByIdAsync(archiveId, Guid.Parse(docId));
                var docBytes = _documentsRepository.DownloadDocumentAsync(archiveId, Guid.Parse(docId));

                Task.WaitAll(new Task[] { document, docBytes });

                if (await document is null) throw new Exception($"Could not get details for document {docId}");

                docsKeyVals.Add((await document)!, await docBytes);
            }

            foreach (var doc in docsKeyVals)
            {
                if (!(doc.Key!.Extension!.Equals(".pdf", StringComparison.OrdinalIgnoreCase))) throw new Exception("One of the documents is not a PDF file");
            }

            using (var writerMemoryStream = new MemoryStream())
            {
                using (var writer = new PdfWriter(writerMemoryStream))
                {
                    using (var mergedDocument = new PdfDocument(writer))
                    {
                        var merger = new PdfMerger(mergedDocument);

                        foreach (var pdf in docsKeyVals)
                        {
                            using (var copyFromMemoryStream = new MemoryStream(pdf.Value))
                            {
                                using (var reader = new PdfReader(copyFromMemoryStream))
                                {
                                    using (var copyFromDocument = new PdfDocument(reader))
                                    {
                                        merger.Merge(copyFromDocument, 1, copyFromDocument.GetNumberOfPages());
                                    }
                                }
                            }
                        }
                    }
                    writer.Close();
                }

                var joinedDocBytes = writerMemoryStream.ToArray();

                string docName = $"JoinedPDF_{DateTime.Now.ToString("yyyy-MM-dd_hhmmss")}.pdf";
                Guid? folderId = docsKeyVals.First().Key.FolderId;

                var joinedDoc = await _documentsRepository.UploadDocumentAsync(archiveId, docName, joinedDocBytes, "application/pdf", folderId);

                await _blobMetadataRepository.InsertAsync(new BlobMetadata() { BlobId = joinedDoc.documentId, ContainerId = archiveId, DisplayName = docName, FolderId = folderId });

                joinedDocId = joinedDoc.documentId.ToString();

            }

            return joinedDocId;

        }
    }
}
