using Documents.Models;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Archives.Services;
using Documents.Validators;

namespace Archives.Controllers
{
    [Authorize]
    [ApiController]
    public class ArchivesController : ControllerBase
    {
        private readonly ArchivesService _archivesService;
        private readonly IValidator<Archive> _archiveValidator;


        public ArchivesController(ArchivesService archivesService,
            IValidator<Archive> archiveValidator)
        {
            _archivesService = archivesService;
            _archiveValidator = archiveValidator;
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
