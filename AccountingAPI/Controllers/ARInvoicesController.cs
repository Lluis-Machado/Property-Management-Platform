using AccountingAPI.DTOs;
using AccountingAPI.Services;
using AccountingAPI.Validators;
using AuthorizeAPI;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountingAPI.Controllers
{
    [Authorize]
    public class ARInvoicesController : Controller
    {
        private readonly IARInvoiceService _arInvoiceService;
        private readonly ILogger<ARInvoicesController> _logger;

        public ARInvoicesController(IARInvoiceService arInvoiceService, ILogger<ARInvoicesController> logger)
        {
            _arInvoiceService = arInvoiceService;
            _logger = logger;
        }

        // POST: Create ARInvoice
        [HttpPost]
        [Route("tenants/{tenantId}/businesspartners/{businessPartnerId}/arinvoices")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ARInvoiceDTO>> CreateARInvoiceAsync(Guid tenantId, Guid businessPartnerId, [FromBody] CreateARInvoiceDTO createARInvoiceDTO)
        {
            // request validations
            if (createARInvoiceDTO is null) return BadRequest("Incorrect body format");

            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            ARInvoiceDTO invoiceDTO = await _arInvoiceService.CreateARInvoiceAndLinesAsync(tenantId, businessPartnerId, createARInvoiceDTO, userName);
            return Created($"arinvoices/{invoiceDTO.Id}", invoiceDTO);
        }

        // GET: Get AR invoice(s)
        [HttpGet]
        [Route("tenants/{tenantId}/arinvoices")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ARInvoiceDTO>>> GetARInvoicesAsync(Guid tenantId, [FromQuery] bool includeDeleted = false, [FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            return Ok(await _arInvoiceService.GetARInvoicesAsync(tenantId, includeDeleted, page, pageSize));
        }

        // GET: Get AR invoice by Id
        [HttpGet]
        [Route("tenants/{tenantId}/arinvoices/{invoiceId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<ARInvoiceDTO>>> GetARInvoiceByIdAsync(Guid tenantId, Guid invoiceId)
        {
            return Ok(await _arInvoiceService.GetARInvoiceByIdAsync(tenantId, invoiceId));
        }

        // PATCH: update invoice
        [HttpPatch]
        [Route("tenants/{tenantId}/arinvoices/{invoiceId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> UpdateARInvoiceAsync(Guid tenantId, Guid invoiceId, [FromBody] UpdateARInvoiceDTO updateARInvoiceDTO)
        {
            // request validations
            if (updateARInvoiceDTO is null) return BadRequest("Incorrect body format");

            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            ARInvoiceDTO invoiceDTO = await _arInvoiceService.UpdateARInvoiceAndLinesAsync(tenantId, invoiceId, updateARInvoiceDTO, userName);
            return Ok(invoiceDTO);
        }

        // DELETE: delete ap invoice
        [HttpDelete]
        [Route("tenants/{tenantId}/arinvoices/{invoiceId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid tenantId, Guid invoiceId)
        {
            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            await _arInvoiceService.SetDeletedARInvoiceAsync(tenantId, invoiceId, true, userName);

            return NoContent();
        }

        // POST: undelete invoice
        [HttpPost]
        [Route("tenants/{tenantId}/arinvoices/{invoiceId}/undelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UndeleteAsync(Guid tenantId, Guid invoiceId)
        {
            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            await _arInvoiceService.SetDeletedARInvoiceAsync(tenantId, invoiceId, false, userName);

            return NoContent();
        }
    }
}
