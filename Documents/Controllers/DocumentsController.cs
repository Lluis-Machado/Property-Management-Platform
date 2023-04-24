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
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<List<CreateDocumentStatus>>> UploadAsync(string tenantName, IFormFile[] files)
        {
            try
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
            catch (Exception e)
            {
                _logger.LogError("Internal exception while uploading document(s) from tenant {Tenant}: {@RQ} {@Exception}",tenantName, files.SelectMany(f => f.FileName), e);
                return Conflict("Internal exception ocurred");
            }
        }

        // GET: Get document(s)
        [Authorize]
        [HttpGet]
        [Route("{tenantName}/documents")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<Document>>> GetAsync(string tenantName,[FromQuery] bool includeDeleted = false)
        {
            try
            {
                //validate tenant
                if (!await _azureBlobStorage.BlobContainerExistsAsync(tenantName)) return NotFound("Tenant not found");

                IEnumerable<Document> documents = await _azureBlobStorage.ListBlobsFlatListingAsync(tenantName, 100, includeDeleted);
                return Ok(documents);
            }
            catch (Exception e)
            {
                _logger.LogError("Internal exception while get document(s) from tenant {Tenant}: {@RQ} {@Exception}", tenantName, includeDeleted, e);
                return Conflict("Internal exception ocurred");
            }
        }

        // GET: Download document
        [Authorize]
        [HttpGet]
        [Route("{tenantName}/documents/{documentId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<byte[]>> DownloadAsync(string tenantName, string documentId)
        {
            try
            {
                //validate document
                if (!await _azureBlobStorage.BlobExistsAsync(tenantName, documentId)) return NotFound("Document not found");

                byte[] byteArray = await _azureBlobStorage.DownloadBlobAsync(tenantName, documentId);
                return File(byteArray, "application/pdf");
            }
            catch (Exception e)
            {
                _logger.LogError("Exception while downloading document from tentant {Tenant}: {@documentId} {@exception}",tenantName, documentId, e);
                return Conflict("Internal exception ocurred");
            }
        }

        // DELETE: Delete document
        [Authorize]
        [HttpDelete]
        [Route("{tenantName}/documents/{documentId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> DeleteAsync(string tenantName, string documentId)
        {
            try
            {
                bool deleted = await _azureBlobStorage.DeleteBlobAsync(tenantName, documentId);

                if(!deleted) return NotFound("Document not found");

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError("Exception while deleting document from tentant {Tenant}: {@documentId} {@exception}",tenantName, documentId, e);
                return Conflict("Internal exception ocurred");
            }
        }

        // POST: Undelete document
        [Authorize]
        [HttpPost]
        [Route("{tenantName}/documents/{documentId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> UndeleteAsync(string tenantName, string documentId)
        {
            try
            {
                bool undeleted = await _azureBlobStorage.UndeleteBlobAsync(tenantName, documentId);

                if (!undeleted) return NotFound("Document not found");

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError("Exception while undeleting document from tenant {tenant}: {documentId} {@exception}",tenantName, documentId, e);
                return Conflict("Internal exception ocurred");
            }
        }

        // POST: Rename document
        [Authorize]
        [HttpPost]
        [Route("{tenantName}/documents/{documentId}/rename")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> RenameAsync(string tenantName, string documentId, [FromForm] string name)
        {
            try
            {
                bool renamed = await _azureBlobStorage.RenameBlobAsync(tenantName, documentId, name);

                if (!renamed) return NotFound("Document not found");

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError("Exception while renaming document from tenant {tenant}: {documentId} {@exception}", tenantName, documentId, e);
                return Conflict("Internal exception ocurred");
            }
        }

        // POST: Copy document
        [Authorize]
        [HttpPost]
        [Route("{tenantName}/documents/{documentId}/copy")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CopyAsync(string tenantName, string documentId, [FromForm] string name)
        {
            try
            {

                bool copied = await _azureBlobStorage.CopyBlobAsync(tenantName, documentId, name);

                if (!copied) return NotFound("Document not found");

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError("Exception while copying document from tenant {tenant}: {documentId} {@exception}", tenantName, documentId, e);
                return Conflict("Internal exception ocurred");
            }
        }

    }
}