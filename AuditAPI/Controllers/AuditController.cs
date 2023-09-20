using AuditsAPI.Models;
using AuditsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AuditsAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("audits")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Audit>>> GetAsync(bool includeDeleted = false)
        {
            return await _auditService.GetAsync(includeDeleted);
        }


        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<Dictionary<string, Tuple<object, object>>>>> GetByIdAsync(Guid id)
        {
            var differences = await _auditService.GetByIdAsync(id);
            if (differences == null)
            {
                return NotFound();
            }

            return differences;
        }

        [HttpGet("{id}/property")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<Dictionary<string, Tuple<object, object>>>>> GetByPropertyIdAsync(Guid id)
        {
            var differences = await _auditService.GetByIdAsync(id);
            if (differences == null)
            {
                return NotFound();
            }

            return differences;
        }

        [HttpGet("{id}/company")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<Dictionary<string, Tuple<object, object>>>>> GetByCompanyIdAsync(Guid id)
        {
            var differences = await _auditService.GetByCompanyIdAsync(id);
            if (differences == null)
            {
                return NotFound();
            }

            return differences;
        }

        [HttpGet("{id}/contact")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<Dictionary<string, Tuple<object, object>>>>> GetByContactIdAsync(Guid id)
        {
            var differences = await _auditService.GetByContactIdAsync(id);
            if (differences == null)
            {
                return NotFound();
            }

            return differences;
        }

        [HttpDelete]
        [Route("{contactId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var lastUser = "test";

            return await _auditService.DeleteAsync(id, lastUser);
        }

        [HttpPatch]
        [Route("{id}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UndeleteAsync(Guid id)
        {
            var lastUser = "test";

            return await _auditService.UndeleteAsync(id, lastUser);
        }
    }
}
