using AutoMapper;
using DocumentsAPI.DTOs;
using DocumentsAPI.Models;
using DocumentsAPI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static DocumentsAPI.Models.Archive;

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
            if (archiveDTO.Name is null) return BadRequest("Archive Name is empty");

            var archive = _mapper.Map<CreateArchiveDTO, Archive>(archiveDTO);

            Archive createdArchive = await _archivesService.CreateArchiveAsync(archive);
            return Created($"archives/{createdArchive.Name}", createdArchive);
        }

        // POST: Create archive for a property
        [HttpPost]
        [Route("archives/property/{propertyId}")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateForProperty(Guid propertyId, [FromBody] CreateArchiveDTO archiveDTO)
        {
            //validations
            if (archiveDTO is null) return BadRequest("Incorrect body format");
            if (archiveDTO.Name is null) return BadRequest("Archive Name is empty");

            var archive = _mapper.Map<CreateArchiveDTO, Archive>(archiveDTO);

            Archive createdArchive = await _archivesService.CreateArchiveAsync(archive, ARCHIVE_TYPE.PROPERTY, propertyId);
            return Created($"archives/property/{createdArchive.Name}", createdArchive);
        }

        // POST: Create archive for a contact
        [HttpPost]
        [Route("archives/contact/{contactId}")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateForContact(Guid contactId, [FromBody] CreateArchiveDTO archiveDTO)
        {
            //validations
            if (archiveDTO == null) return BadRequest("Incorrect body format");
            if (archiveDTO.Name == null) return BadRequest("Archive Name is empty");

            var archive = _mapper.Map<CreateArchiveDTO, Archive>(archiveDTO);

            Archive createdArchive = await _archivesService.CreateArchiveAsync(archive, ARCHIVE_TYPE.CONTACT, contactId);
            return Created($"archives/property/{createdArchive.Name}", createdArchive);
        }

        // POST: Create archive for a company
        [HttpPost]
        [Route("archives/company/{companyId}")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateForCompany(Guid companyId, [FromBody] CreateArchiveDTO archiveDTO)
        {
            //validations
            if (archiveDTO == null) return BadRequest("Incorrect body format");
            if (archiveDTO.Name == null) return BadRequest("Archive Name is empty");

            var archive = _mapper.Map<CreateArchiveDTO, Archive>(archiveDTO);

            Archive createdArchive = await _archivesService.CreateArchiveAsync(archive, ARCHIVE_TYPE.COMPANY, companyId);
            return Created($"archives/property/{createdArchive.Name}", createdArchive);
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
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
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
