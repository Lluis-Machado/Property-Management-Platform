using DocumentsAPI.DTOs;
using DocumentsAPI.Models;
using DocumentsAPI.Repositories;
using DocumentsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Documents.Controllers
{
#if DEVELOPMENT == false
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
        public async Task<ActionResult<Folder>> CreateAsync(Guid archiveId, [FromBody] CreateFolderDTO createFolderDTO)
        {
            if (createFolderDTO == null) return new BadRequestObjectResult("Incorrect body format");

            string userName = User?.Identity?.Name ?? "na";

            var folderCreated = await _foldersService.CreateFolderAsync(archiveId, createFolderDTO, userName);
            return Created($"{archiveId}/folders/{folderCreated.Id}", folderCreated);
        }

        //GET: Get Folder(s)
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

        //PATCH: update Folder
        [HttpPatch]
        [Route("{archiveId}/folders/{folderId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<FolderDTO>> UpdateAsync(Guid archiveId, Guid folderId, [FromBody] FolderDTO folderDTO)
        {
            // request validations
            if (folderDTO == null) return BadRequest("Incorrect body format");
            if (folderDTO.Id != folderId) return BadRequest("folder Id from body incorrect");

            var exist = await _foldersService.CheckFolderExist(folderDTO.Id);
            if (!exist) return NotFound("Folder not found");

            string userName = User?.Identity?.Name ?? "na";

            var result = await _foldersService.UpdateFolderAsync(folderDTO, userName);
            return Ok(result);
        }

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