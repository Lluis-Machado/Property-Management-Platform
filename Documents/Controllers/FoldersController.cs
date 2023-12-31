using DocumentsAPI.DTOs;
using DocumentsAPI.Models;
using DocumentsAPI.Repositories;
using DocumentsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Documents.Controllers
{
#if PRODUCTION
    [Authorize]
#endif
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private readonly ILogger<DocumentsController> _logger;
        private readonly IConfiguration _config;
        private readonly IFolderRepository _folderRepository;
        private readonly IFoldersService _foldersService;

        public FoldersController(IConfiguration config, IFoldersService foldersService, IFolderRepository folderRepository, ILogger<DocumentsController> logger)
        {
            _config = config;
            _folderRepository = folderRepository;
            _logger = logger;
            _foldersService = foldersService;
        }

        // POST: Create folder
        [HttpPost]
        [Route("{archiveId}/folders")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.MultiStatus)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<TreeFolderItem>> CreateAsync(Guid archiveId, [FromBody] CreateFolderDTO createFolderDTO)
        {
            if (createFolderDTO == null) return new BadRequestObjectResult("Incorrect body format");

            string userName = User?.Identity?.Name ?? "na";

            var folderCreated = await _foldersService.CreateFolderAsync(archiveId, createFolderDTO, userName);
            var folderCreatedTreeItem = new TreeFolderItem(folderCreated);
            return Created($"{archiveId}/folders/{folderCreated.Id}", folderCreatedTreeItem);
        }

        //GET: Get Folder(s) by Archive ID
        [HttpGet]
        [Route("{archiveId}/folders")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<TreeFolderItem>>> GetAsync(Guid archiveId, [FromQuery] bool includeDeleted = false)
        {
            var folders = await _foldersService.GetFoldersAsync(archiveId, includeDeleted);
            //folders = _folderRepository.ToFolderTreeView(folders.ToList());
            return Ok(folders);
        }

        //GET: Get All Folder(s)
        [HttpGet]
        [Route("folders")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<TreeFolderItem>>> GetAllAsync([FromQuery] bool includeDeleted = false)
        {
            var folders = await _foldersService.GetFoldersAsync(null, includeDeleted);
            //folders = _folderRepository.ToFolderTreeView(folders.ToList());
            return Ok(folders);
        }

        //PATCH: update Folder
        [HttpPatch]
        [Route("{archiveId}/folders/{folderId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<FolderDTO>> UpdateAsync(Guid archiveId, Guid folderId, [FromBody] UpdateFolderDTO folderDTO)
        {
            // request validations
            if (folderDTO == null) return BadRequest("Incorrect body format");

            // If no new ArchiveId is specified in the update DTO, leave the old one
            if (folderDTO.ArchiveId == Guid.Empty) folderDTO.ArchiveId = archiveId;

            //var exist = await _foldersService.CheckFolderExist(folderId);
            //if (!exist) return NotFound("Folder not found");

            string userName = User?.Identity?.Name ?? "na";

            var currentFolder = await _foldersService.GetFolderByIdAsync(folderId);
            if (currentFolder == null) return NotFound();
            if (folderDTO.ParentId == null) folderDTO.ParentId = currentFolder.ParentId;
            Guid currentFolderArchive = currentFolder.ArchiveId;

            var result = await _foldersService.UpdateFolderAsync(folderDTO, folderId, userName);
            if (currentFolderArchive != folderDTO.ArchiveId) await _foldersService.UpdateChildrenArchiveAsync(parentId: folderId, oldArchiveId: currentFolderArchive, newArchiveId: folderDTO.ArchiveId);
            return Ok(result);
        }

        //POST: Copy Folder to another Archive and/or Folder
        [HttpPost]
        [Route("{archiveId}/folders/{folderId}/copy")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult<TreeFolderItem>> CopyAsync(Guid archiveId, Guid folderId, [FromBody] UpdateFolderDTO folderDTO)
        {
            string userName = User?.Identity?.Name ?? "na";
            var currentFolder = await _foldersService.GetFolderByIdAsync(folderId);
            if (currentFolder == null)
            {
                _logger.LogWarning($"Folder CopyAsync - Folder with id {folderId} was not found in DB, returning 404");
                return NotFound($"Folder with ID {folderId} not found");
            }

            //_logger.LogInformation($"Copying folder with ID: {folderId} from archive {currentFolder.ArchiveId} to {folderDTO.ArchiveId}");

            var result = await _foldersService.CopyFolderAndChildren(currentFolder, folderDTO, userName);

            return Ok(result);
        }

        //POST: Copy Folder to another Archive and/or Folder
        [HttpPost]
        [Route("{archiveId}/folders/{folderId}/move")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult<TreeFolderItem>> MoveAsync(Guid archiveId, Guid folderId, [FromBody] UpdateFolderDTO folderDTO)
        {
            string userName = User?.Identity?.Name ?? "na";
            var currentFolder = await _foldersService.GetFolderByIdAsync(folderId);
            if (currentFolder == null)
            {
                _logger.LogWarning($"Folder CopyAsync - Folder with id {folderId} was not found in DB, returning 404");
                return NotFound($"Folder with ID {folderId} not found");
            }

            //_logger.LogInformation($"Copying folder with ID: {folderId} from archive {currentFolder.ArchiveId} to {folderDTO.ArchiveId}");

            var result = await _foldersService.CopyFolderAndChildren(currentFolder, folderDTO, userName, deleteOriginal: true);

            return Ok(result);
        }


        //PATCH: Move Folder to another Archive and/or Folder and delete the original
        /**
         * 1) Patch folder with specified Archive and Parent IDs
         * 2) For every child, update ArchiveID if it has changed
         * 3) If ArchiveID has changed, use DocumentsService to move all documents with FolderID and every level below recursively
         * */

        //DELETE: delete folder
        [HttpDelete]
        [Route("{archiveId}/folders/{folderId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> DeleteAsync(Guid folderId)
        {
            var exist = await _foldersService.CheckFolderExist(folderId);
            if (!exist) return NotFound("Folder not found");

            string userName = User?.Identity?.Name ?? "na";

            var result = await _foldersService.DeleteFolderAsync(folderId, userName);
            return Ok(result);
        }

        //PATCH: Undelete folder
        [HttpPatch]
        [Route("{archiveId}/folders/{folderId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<FolderDTO>> UndeleteAsync(Guid folderId)
        {
            var exist = await _foldersService.CheckFolderExist(folderId);
            if (!exist) return NotFound("Folder not found");

            string userName = User?.Identity?.Name ?? "na";

            var result = await _foldersService.UnDeleteFolderAsync(folderId, userName);
            return Ok(result);
        }

    }
}
