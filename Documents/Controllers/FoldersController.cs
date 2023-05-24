using Documents.Models;
using Documents.Services.AzureBlobStorage;
using DocumentsAPI.Models;
using DocumentsAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Documents.Controllers
{
    //[Authorize]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private readonly ILogger<DocumentsController> _logger;
        private readonly IConfiguration _config ;
        private readonly IFolderRepository _folderRepository;

        public FoldersController(IConfiguration config, IFolderRepository folderRepository, ILogger<DocumentsController> logger)
        {
            _config = config;
            _folderRepository = folderRepository;
            _logger = logger;
        }

        // POST: Create folder
        [HttpPost]
        [Route("{archiveId}/folders")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.MultiStatus)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<CreateDocumentStatus>>> UploadAsync(Guid archiveId, string folderName, Guid? parentId)
        {

            // Tenant validation

            // folder validation
            //ValidationResult validationResult = await _businessPartnerValidator.ValidateAsync(businessPartner);
            //if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));
            Folder folder = new()
            {
                ArchiveId = archiveId,
                Name = folderName,
                ParentId = parentId,
            };

            folder = await _folderRepository.InsertFolderAsync(folder);
            return Created($"{archiveId}/folders/{folder.Id}", folder);
        }

        //GET: Get Folder(s)
        [HttpGet]
        [Route("{archiveId}/folders")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Folder>>> GetAsync(Guid archiveId, [FromQuery] bool includeDeleted = false)
        {
            IEnumerable<Folder> folders = await _folderRepository.GetFoldersAsync(archiveId, includeDeleted);
            folders = _folderRepository.ToFolderTreeView(folders.ToList());
            return Ok(folders);
        }

        //PATCH: update Folder
        [HttpPatch]
        [Route("{archiveId}/folders/{folderId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateAsync(Guid archiveId, Guid folderId, [FromBody] Folder folder)
        {
            // request validations
            if (folder == null) return BadRequest("Incorrect body format");
            if (folder.Id != folderId) return BadRequest("folder Id from body incorrect");

            // businessPartner validation
            //ValidationResult validationResult = await _businessPartnerValidator.ValidateAsync(businessPartner);
            //if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            folder.Id = folderId; // copy id to folder object

            int result = await _folderRepository.UpdateFolderAsync(folder);
            if (result == 0) return NotFound("Folder not found");
            return NoContent();
        }

        //DELETE: delete folder
        [HttpDelete]
        [Route("{archiveId}/folders/{folderId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid folderId)
        {
            int result = await _folderRepository.SetDeleteFolderAsync(folderId, true);
            if (result == 0) return NotFound("Folder not found");
            return NoContent();
        }

        //PATCH: Undelete folder
        [HttpPatch]
        [Route("{archiveId}/folders/{folderId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid folderId)
        {
            int result = await _folderRepository.SetDeleteFolderAsync(folderId, false);
            if (result == 0) return NotFound("Folder not found");
            return NoContent();
        }

    }
}