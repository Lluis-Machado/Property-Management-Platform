using Authentication.Utils;
using Documents.Models;
using Documents.Services.AzureBlobStorage;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Documents.Controllers
{
    [ApiController]
    [Route("api/dms")]
    public class DocumentsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAzureBlobStorage _azureBlobStorage;

        public DocumentsController(IConfiguration config, IAzureBlobStorage azureBlobStorage)
        {
            _config = config;
            _azureBlobStorage = azureBlobStorage;
        }

        // POST: Create document
        [HttpPost]
        [Route("{tenantName}/documents")]
        [ProducesResponseType((int)HttpStatusCode.MultiStatus)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<string>),(int)HttpStatusCode.OK)]
        [SecurityControl]
        public async Task<IActionResult> UploadAsync(string tenantName, IFormFile[] files)
        {
            try
            {
                // empty request validation
                if (files == null) return BadRequest("Files not found");
                // max nb of files validation
                int maxNbOfFiles = _config.GetValue<int>("Files:MaxNbOfUploadFiles");
                if (Request.Form.Files.Count > maxNbOfFiles) return BadRequest($"Maximal number of files ({maxNbOfFiles}) exceeded");

                List<string> documents = new();

                await Parallel.ForEachAsync(Request.Form.Files, async (file, CancellationToken) =>
                {
                    string? documentId = await _azureBlobStorage.UploadAsync(tenantName, file.FileName, file.OpenReadStream());
                    if(!string.IsNullOrEmpty(documentId)) documents.Add(documentId);
                });

                return Ok(documents);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: Get document(s)
        [HttpGet]
        [Route("{tenantName}/documents")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<Document>),(int)HttpStatusCode.OK)]
        [SecurityControl]
        public async Task<IActionResult> GetAsync(string tenantName,[FromQuery] bool includeDeleted = false)
        {
            try
            {
                List<Document> documents = await _azureBlobStorage.ListBlobsFlatListingAsync(tenantName, 100, includeDeleted);
                return Ok(documents);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: Download document
        [HttpGet]
        [Route("{tenantName}/documents/{documentId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.OK)]
        [SecurityControl]
        public async Task<IActionResult> DownloadAsync(string tenantName, string documentId)
        {
            try
            {
                byte[] byteArray = await _azureBlobStorage.DownloadBlobAsync(tenantName, documentId);
                return File(byteArray, "application/pdf");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: Delete document
        [HttpDelete]
        [Route("{tenantName}/documents/{documentId}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [SecurityControl]
        public async Task<IActionResult> DeleteAsync(string tenantName, string documentId)
        {
            try
            {
                await _azureBlobStorage.DeleteBlobAsync(tenantName, documentId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: Undelete document
        [HttpPost]
        [Route("{tenantName}/documents/{documentId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [SecurityControl]
        public async Task<IActionResult> UndeleteAsync(string tenantName, string documentId)
        {
            try
            {
                await _azureBlobStorage.UndeleteBlobAsync(tenantName, documentId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: Rename document
        [HttpPost]
        [Route("{tenantName}/documents/{documentId}/rename")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [SecurityControl]
        public async Task<IActionResult> RenameAsync(string tenantName, string documentId, [FromForm] string name)
        {
            try
            {
                await _azureBlobStorage.RenameBlobAsync(tenantName, documentId, name);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: Copy document
        [HttpPost]
        [Route("{tenantName}/documents/{documentId}/copy")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [SecurityControl]
        public async Task<IActionResult> CopyAsync(string tenantName, string documentId, [FromForm] string name)
        {
            try
            {
                await _azureBlobStorage.RenameBlobAsync(tenantName, documentId, name);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}