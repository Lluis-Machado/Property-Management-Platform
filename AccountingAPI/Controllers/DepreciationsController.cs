﻿using AccountingAPI.DTOs;
using AccountingAPI.Services;
using AccountingAPI.Validators;
using AuthorizeAPI;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountingAPI.Controllers
{
    [Authorize]
    public class DepreciationsController : Controller
    {
        private readonly IDepreciationService _depreciationService;
        private readonly ILogger<DepreciationsController> _logger;

        public DepreciationsController(IDepreciationService depreciationService, ILogger<DepreciationsController> logger)
        {
            _depreciationService = depreciationService;
            _logger = logger;
        }

        // POST: Create depreciations
        [HttpPost]
        [Route("tenants/{tenantId}/periods/{periodId}/depreciations")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<DepreciationDTO>>> GenerateDepreciationsAsync(Guid tenantId, Guid periodId)
        {
            // Check user
            string userName = UserNameValidator.GetValidatedUserName(User?.Identity?.Name);

            return Ok(await _depreciationService.GenerateDepreciationsAsync(tenantId, periodId, userName));
        }
    }
}
