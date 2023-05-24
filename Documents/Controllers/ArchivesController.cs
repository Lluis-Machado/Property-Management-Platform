using Documents.Models;
using DocumentsAPI.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Archives.Controllers
{
    //[Authorize]
    [ApiController]
    public class ArchivesController : Controller
    {
        private readonly ILogger<ArchivesController> _logger;
        private readonly IArchiveRepository _archiveRepository;
        private readonly IValidator<Archive> _archiveValidator;

        public ArchivesController(IArchiveRepository archiveRepository, IValidator<Archive> archiveValidator, ILogger<ArchivesController> logger)
        {
            _archiveRepository = archiveRepository;
            _archiveValidator = archiveValidator;
            _logger = logger;
        }

        // POST: Create archive
        [HttpPost]
        [Route("archives")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromBody] Archive archive)
        {
            //validations
            if (archive == null) return BadRequest("Incorrect body format");

            ValidationResult validationResult = await _archiveValidator.ValidateAsync(archive);
            if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

            if (archive.Name == null) return BadRequest("Archive Name is empty");

            //create archive
            archive.Id = Guid.NewGuid();
            await _archiveRepository.CreateArchiveAsync(archive);
            return Created($"archives/{archive.Name}", archive);
        }

        // GET: Get archive(s)
        [HttpGet]
        [Route("archives")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Archive>>> GetAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _archiveRepository.GetArchivesAsync(100, includeDeleted));
        }

        // DELETE: Delete archive
        [HttpDelete]
        [Route("archives/{archiveId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid archiveId)
        {
            await _archiveRepository.DeleteArchiveAsync(archiveId);
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
            await _archiveRepository.UndeleteArchiveAsync(archiveId);
            return NoContent();
        }
    }
}
