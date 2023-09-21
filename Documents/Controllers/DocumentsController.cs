using DocumentsAPI.DTOs;
using DocumentsAPI.Models;
using DocumentsAPI.Repositories;
using DocumentsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Document = DocumentsAPI.Models.Document;

namespace Documents.Controllers
{
#if PRODUCTION
    [Authorize]
#endif
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly ILogger<DocumentsController> _logger;
        private readonly IConfiguration _config;
        private readonly IDocumentRepository _documentsRepository;
        private readonly IDocumentsService _documentsService;
        private readonly IFoldersService _foldersService;

        public DocumentsController(IConfiguration config, IDocumentRepository documentsRepository, IFoldersService foldersService, ILogger<DocumentsController> logger, IDocumentsService documentsService)
        {
            _config = config;
            _documentsRepository = documentsRepository;
            _logger = logger;
            _documentsService = documentsService;
            _foldersService = foldersService;
        }

        // POST: Upload document(s)
        [HttpPost]
        [Route("{archiveId}/documents")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.MultiStatus)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<CreateDocumentStatus>>> UploadAsync(Guid archiveId, IFormFile[] files, [FromQuery] Guid? folderId = null)
        {
            // empty request validation
            if (files.Length == 0) return BadRequest("Files not found");

            // max nb of files validation
            int maxNbOfFiles = _config.GetValue<int>("Files:MaxNbOfUploadFiles");
            if (files.Length > maxNbOfFiles) return BadRequest($"Maximal number of files ({maxNbOfFiles}) exceeded");

            List<CreateDocumentStatus> documents = await _documentsService.UploadAsync(archiveId, files, folderId);

            if (documents.Any(doc => doc.Status == HttpStatusCode.OK))
            {
                if (folderId != null) await _foldersService.UpdateFolderHasDocuments((Guid)folderId, true);
            }
            if (documents.Any(doc => doc.Status != HttpStatusCode.OK))
            {
                // some documents failed
                this.HttpContext.Response.StatusCode = (int)HttpStatusCode.MultiStatus;
                return documents;
            }

            // all documents ok
            return Ok(documents);
        }

        // POST: Split document
        [Obsolete("Use SplitBlobAsync to split a PDF document already uploaded to Blob Storage")]
        [HttpPost]
        [Route("split")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<FileContentResult>>> SplitAsync(IFormFile file, [FromQuery] string? ranges = null)
        {
            // empty request validation
            if (file == null) return BadRequest();

            DocSplitInterval[]? intervals = DocSplitInterval.FromRanges(ranges);

            if (file == null || file.ContentType != "application/pdf") return BadRequest("Unsupported content type. Make sure the file is of type \"application/pdf\"");

            var pdfFiles = await _documentsService.SplitAsync(file, intervals);

            var pdfBase64Strings = new List<string>();
            foreach (var pdf in pdfFiles)
            {
                // Convert each PDF to base64 string
                string base64String = Convert.ToBase64String(pdf.FileContents);
                pdfBase64Strings.Add(base64String);
            }

            // Return the list of base64 strings in JSON format
            return Ok(new { SplitFiles = pdfBase64Strings });
        }

        // POST: Split document already present in Blob Storage
        [HttpPost]
        [Route("{archiveId}/documents/{documentId}/split")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<FileContentResult>>> SplitBlobAsync(Guid archiveId, Guid documentId, [FromQuery] string? ranges = null)
        {

            DocSplitInterval[]? intervals = DocSplitInterval.FromRanges(ranges);
            try
            {
                var splitIds = await _documentsService.SplitBlobAsync(archiveId, documentId, intervals);
                return Ok(splitIds);

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("1-page")) return BadRequest(ex.Message);
                else return StatusCode(500, ex.Message);
            }

        }

        // POST: Split document already present in Blob Storage
        [HttpPost]
        [Route("{archiveId}/documents/join")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<FileContentResult>>> JoinBlobsAsync(Guid archiveId, [FromBody] string[] documentIds)
        {

            if (documentIds is null || documentIds.Length == 0) return BadRequest();
            if (documentIds.Length == 1) return BadRequest("Send two or more IDs of documents to join");

            try
            {
                var newBlobId = await _documentsService.JoinBlobsAsync(archiveId, documentIds);
                return Ok(newBlobId);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not a PDF file")) return BadRequest(ex.Message);
                else return StatusCode(500, ex.Message);
            }

        }



        // GET: Get document(s)
        [HttpGet]
        [Route("{archiveId}/documents")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Document>>> GetDocumentsAsync(Guid archiveId, [FromQuery] Guid? folderId = null, [FromQuery] bool includeDeleted = false)
        {
            return Ok(await _documentsService.GetDocumentsAsync(archiveId, 100, folderId, includeDeleted));
        }

        // GET: Search documents
        [HttpGet]
        [Route("searchTags")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Document>>> SearchDocumentsTagsAsync([FromQuery] string query, [FromQuery] bool includeDeleted = false)
        {
            return Ok(await _documentsService.SearchDocumentsTagsAsync(query, includeDeleted));
        }

        // GET: Search documents by metadata
        [HttpGet]
        [Route("{archiveId}/documents/search")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<BlobMetadata>>> SearchDocumentsMetadataAsync(Guid archiveId, [FromQuery] string? displayName, [FromQuery] Guid? folderId = null, [FromQuery] bool includeDeleted = false)
        {
            return Ok(await _documentsService.SearchMetadataAsync(displayName, folderId, archiveId, includeDeleted));
        }

        // GET: Search documents by metadata across all containers
        [HttpGet]
        [Route("search")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<BlobMetadata>>> SearchDocumentsMetadataAsync([FromQuery] string? displayName, [FromQuery] Guid? folderId = null, [FromQuery] Guid? archiveId = null, [FromQuery] bool includeDeleted = false)
        {
            return Ok(await _documentsService.SearchMetadataAsync(displayName, folderId, archiveId, includeDeleted));
        }

        // GET: Download document
        [HttpGet]
        [Route("{archiveId}/documents/{documentId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<FileContentResult> DownloadAsync(Guid archiveId, Guid documentId)
        {
            var results = await _documentsService.DownloadAsync(archiveId, documentId);
            byte[] byteArray = results.FileContents;
            return File(byteArray, results.ContentType);
        }

        // GET: Download document
        [HttpGet]
        [Route("{archiveId}/documents/{documentId}/url")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> GetDocumentUrlAsync(Guid archiveId, Guid documentId)
        {
            var results = await _documentsService.GetDocumentUrlByIdAsync(archiveId, documentId);
            return Ok(results);
        }


        // DELETE: Delete document
        [HttpDelete]
        [Route("{archiveId}/documents/{documentId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid archiveId, Guid documentId)
        {
            var document = await _documentsService.GetDocumentByIdAsync(archiveId, documentId);
            if (document == null || document == new Document()) { throw new Exception($"Document with ID {documentId} not found"); }
            var docsInFolder = await _documentsService.GetDocumentsAsync(archiveId, 100, document.FolderId, false);
            await _documentsService.DeleteAsync(archiveId, documentId);
            // If only one document remains in the folder, mark the folder as empty after deleting the document
            if (docsInFolder.Count() == 1 && document.FolderId != null)
                await _foldersService.UpdateFolderHasDocuments((Guid)document.FolderId, false);

            return NoContent();
        }

        // POST: Undelete document
        [HttpPatch]
        [Route("{archiveId}/documents/{documentId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid archiveId, Guid documentId)
        {
            await _documentsService.UndeleteAsync(archiveId, documentId);
            var document = await _documentsService.GetDocumentByIdAsync(archiveId, documentId);
            if (document.FolderId != null) await _foldersService.UpdateFolderHasDocuments((Guid)document.FolderId, true);


            return NoContent();
        }

        // POST: Rename document
        [HttpPatch]
        [Route("{archiveId}/documents/{documentId}/rename")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RenameAsync(Guid archiveId, Guid documentId, [FromForm] string documentName)
        {
            await _documentsService.RenameAsync(archiveId, documentId, documentName);
            return NoContent();
        }

        // POST: Copy document
        [HttpPost]
        [Route("{archiveId}/documents/{documentId}/copy")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CopyAsync(Guid archiveId, Guid documentId, [FromForm] Guid destinationArchive, [FromForm] string documentName, [FromForm] Guid? folderId = null)
        {
            var newDocGuid = await _documentsService.CopyAsync(sourceArchive: archiveId, destinationArchive: destinationArchive, documentId, documentName, folderId);
            if (folderId != null) await _foldersService.UpdateFolderHasDocuments((Guid)folderId, true);

            return Created($"{archiveId}/documents/{newDocGuid}", newDocGuid);
        }

        // POST: Move document
        [HttpPost]
        [Route("{archiveId}/documents/{documentId}/move")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> MoveAsync(Guid archiveId, Guid documentId, [FromForm] Guid destinationArchive, [FromForm] string documentName, [FromForm] Guid? folderId = null)
        {
            // Check if user is trying to move a document to the same folder it's currently in
            if (folderId != null && (archiveId == destinationArchive))
            {
                var doc = await _documentsService.GetDocumentByIdAsync((Guid)archiveId, documentId);
                if (doc.FolderId == folderId) return UnprocessableEntity("Cannot move document to the same folder and archive it's currently in");
            }

            await _documentsService.MoveAsync(sourceArchive: archiveId, destinationArchive: destinationArchive, documentId, documentName, folderId);
            if (folderId != null) await _foldersService.UpdateFolderHasDocuments((Guid)folderId, true);

            return Created($"{archiveId}/documents/{documentId}", documentId);
        }


    }
}