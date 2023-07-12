using DocumentsAPI.Services;
using DocumentsAPI.Models;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using DocumentsAPI.DTOs;
using AutoMapper;

namespace Archives.Controllers
{
#if PRODUCTION
    [Authorize]
#endif
    [ApiController]
    public class ArchivesController : ControllerBase
    {
        private readonly IArchivesService _archivesService;
        private readonly IValidator<Archive> _archiveValidator;
        private readonly IMapper _mapper;


        public ArchivesController(IArchivesService archivesService,
            IValidator<Archive> archiveValidator,
            IMapper mapper)
        {
            _archivesService = archivesService;
            _archiveValidator = archiveValidator;
            _mapper = mapper;
        }

        // POST: Create archive
        [HttpPost]
        [Route("archives")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateArchiveDTO archiveDTO)
        {
            //validations
            if (archiveDTO == null) return BadRequest("Incorrect body format");

            var archive = _mapper.Map<CreateArchiveDTO, Archive>(archiveDTO);

            // TODO: Move validation out of controller!
            ValidationResult validationResult = await _archiveValidator.ValidateAsync(archive);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            if (archive.Name == null) return BadRequest("Archive Name is empty");

            Archive createdArchive = await _archivesService.CreateArchiveAsync(archive);
            return Created($"archives/{createdArchive.Name}", createdArchive);
        }

        // GET: Get archive(s)
        [HttpGet]
        [Route("archives")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Archive>>> GetAsync([FromQuery] bool includeDeleted = false)
        {
            IEnumerable<Archive> archives = await _archivesService.GetArchivesAsync(includeDeleted);
            return Ok(archives);
        }

        // PATCH: Update archive
        [HttpPatch]
        [Route("archives/{archiveId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateAsync(Guid archiveId, string newName)
        {
            await _archivesService.UpdateArchiveAsync(archiveId, newName);
            return NoContent();
        }

        // DELETE: Delete archive
        [HttpDelete]
        [Route("archives/{archiveId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid archiveId)
        {
            await _archivesService.DeleteArchiveAsync(archiveId);
            return NoContent();
        }

        // POST: Undelete archive
        [HttpPatch]
        [Route("archives/{archiveId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid archiveId)
        {
            await _archivesService.UndeleteArchiveAsync(archiveId);
            return NoContent();
        }
    }
}
