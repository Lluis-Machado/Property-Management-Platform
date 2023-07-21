using DocumentsAPI.Models;
using DocumentsAPI.Services;
using DocumentsAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection.Metadata;
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

        public DocumentsController(IConfiguration config, IDocumentRepository documentsRepository, IFoldersService foldersService,ILogger<DocumentsController> logger, IDocumentsService documentsService)
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
            } else if (documents.Any(doc => doc.Status != HttpStatusCode.OK))
            {
                // some documents failed
                this.HttpContext.Response.StatusCode = (int)HttpStatusCode.MultiStatus;
                return documents;
            }

            // all documents ok
            return Ok(documents);
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
        public async Task<IActionResult> CopyAsync(Guid archiveId, Guid documentId, [FromForm] string documentName, [FromForm] Guid? folderId = null)
        {
            await _documentsService.CopyAsync(archiveId, documentId, documentName, folderId);
            if (folderId != null) await _foldersService.UpdateFolderHasDocuments((Guid)folderId, true);

            return NoContent();
        }

    }
}