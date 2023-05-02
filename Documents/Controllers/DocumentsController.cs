using Documents.Models;
using Documents.Services.AzureBlobStorage;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaxManagement.Security;

namespace Documents.Controllers
{
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly ILogger<DocumentsController> _logger;
        private readonly IConfiguration _config ;
        private readonly IAzureBlobStorage _azureBlobStorage;

        public DocumentsController(IConfiguration config, IAzureBlobStorage azureBlobStorage, ILogger<DocumentsController> logger)
        {
            _config = config;
            _azureBlobStorage = azureBlobStorage;
            _logger = logger;
        }

        // POST: Upload document(s)
        [Authorize]
        [HttpPost]
        [Route("{tenantName}/documents")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.MultiStatus)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<CreateDocumentStatus>>> UploadAsync(string tenantName, IFormFile[] files)
        {
            // empty request validation
            if (files.Length == 0) return BadRequest("Files not found");

            // max nb of files validation
            int maxNbOfFiles = _config.GetValue<int>("Files:MaxNbOfUploadFiles");
            if (files.Length > maxNbOfFiles) return BadRequest($"Maximal number of files ({maxNbOfFiles}) exceeded");

            var documents = new List<CreateDocumentStatus>();

            await Parallel.ForEachAsync(files, async (file, CancellationToken) =>
            {
                Stream fileStream = file.OpenReadStream();
                HttpStatusCode status = await _azureBlobStorage.UploadAsync(tenantName, file.FileName, fileStream);
                documents.Add(new CreateDocumentStatus(file.FileName, status));
            });

            if (documents.Any(doc => doc.Status != HttpStatusCode.OK))
            {
                // some documents failed
                this.HttpContext.Response.StatusCode = (int)HttpStatusCode.MultiStatus;
                return documents;
            }
            // all documents ok
            return Ok(documents);
        }

        // GET: Get document(s)
        [Authorize]
        [HttpGet]
        [Route("{tenantName}/documents")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Document>>> GetAsync(string tenantName,[FromQuery] bool includeDeleted = false)
        {
            return Ok(await _azureBlobStorage.ListBlobsFlatListingAsync(tenantName, 100, includeDeleted));
        }

        // GET: Download document
        [Authorize]
        [HttpGet]
        [Route("{tenantName}/documents/{documentId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<FileContentResult> DownloadAsync(string tenantName, string documentId)
        {
            byte[] byteArray = await _azureBlobStorage.DownloadBlobAsync(tenantName, documentId);
            return File(byteArray, "application/pdf");
        }

        // DELETE: Delete document
        [Authorize]
        [HttpDelete]
        [Route("{tenantName}/documents/{documentId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(string tenantName, string documentId)
        {
            await _azureBlobStorage.DeleteBlobAsync(tenantName, documentId);
            return NoContent();
        }

        // POST: Undelete document
        [Authorize]
        [HttpPost]
        [Route("{tenantName}/documents/{documentId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(string tenantName, string documentId)
        {
            await _azureBlobStorage.UndeleteBlobAsync(tenantName, documentId);
            return NoContent();
        }

        // POST: Rename document
        [Authorize]
        [HttpPost]
        [Route("{tenantName}/documents/{documentId}/rename")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RenameAsync(string tenantName, string documentId, [FromForm] string name)
        {
            await _azureBlobStorage.RenameBlobAsync(tenantName, documentId, name);
            return NoContent();
        }

        // POST: Copy document
        [Authorize]
        [HttpPost]
        [Route("{tenantName}/documents/{documentId}/copy")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CopyAsync(string tenantName, string documentId, [FromForm] string name)
        {
            await _azureBlobStorage.CopyBlobAsync(tenantName, documentId, name);
            return NoContent();
        }

    }
}