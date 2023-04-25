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
            if (files == null) return BadRequest("Files not found");

            // max nb of files validation
            int maxNbOfFiles = _config.GetValue<int>("Files:MaxNbOfUploadFiles");
            if (Request.Form.Files.Count > maxNbOfFiles) return BadRequest($"Maximal number of files ({maxNbOfFiles}) exceeded");

            //validate tenant
            if (!await _azureBlobStorage.BlobContainerExistsAsync(tenantName)) return NotFound("Tenant not found");

            var documents = new List<CreateDocumentStatus>();

            await Parallel.ForEachAsync(files, async (file, CancellationToken) =>
            {
                HttpStatusCode status = await _azureBlobStorage.UploadAsync(tenantName, file.FileName, file.OpenReadStream());
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
            //validate tenant
            if (!await _azureBlobStorage.BlobContainerExistsAsync(tenantName)) return NotFound("Tenant not found");

            return Ok(await _azureBlobStorage.ListBlobsFlatListingAsync(tenantName, 100, includeDeleted));
        }

        // GET: Download document
        [Authorize]
        [HttpGet]
        [Route("{tenantName}/documents/{documentId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<byte[]>> DownloadAsync(string tenantName, string documentId)
        {
            //validate document
            if (!await _azureBlobStorage.BlobExistsAsync(tenantName, documentId)) return NotFound("Document not found");

            byte[] byteArray = await _azureBlobStorage.DownloadBlobAsync(tenantName, documentId);
            return File(byteArray, "application/pdf");
        }

        // DELETE: Delete document
        [Authorize]
        [HttpDelete]
        [Route("{tenantName}/documents/{documentId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(string tenantName, string documentId)
        {
            bool deleted = await _azureBlobStorage.DeleteBlobAsync(tenantName, documentId);

            if(!deleted) return NotFound("Document not found");

            return Ok();
        }

        // POST: Undelete document
        [Authorize]
        [HttpPost]
        [Route("{tenantName}/documents/{documentId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(string tenantName, string documentId)
        {
            bool undeleted = await _azureBlobStorage.UndeleteBlobAsync(tenantName, documentId);

            if (!undeleted) return NotFound("Document not found");

            return Ok();
        }

        // POST: Rename document
        [Authorize]
        [HttpPost]
        [Route("{tenantName}/documents/{documentId}/rename")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RenameAsync(string tenantName, string documentId, [FromForm] string name)
        {
            bool renamed = await _azureBlobStorage.RenameBlobAsync(tenantName, documentId, name);

            if (!renamed) return NotFound("Document not found");

            return Ok();
        }

        // POST: Copy document
        [Authorize]
        [HttpPost]
        [Route("{tenantName}/documents/{documentId}/copy")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CopyAsync(string tenantName, string documentId, [FromForm] string name)
        {
            bool copied = await _azureBlobStorage.CopyBlobAsync(tenantName, documentId, name);

            if (!copied) return NotFound("Document not found");

            return Ok();
        }

    }
}