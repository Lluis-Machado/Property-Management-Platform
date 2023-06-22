using AccountingAPI.DTOs;
using AccountingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountingAPI.Controllers
{
    [Authorize]
    public class FixedAssetsController : Controller
    {
        private readonly IDepreciationService _depreciationService;
        private readonly ILogger<FixedAssetsController> _logger;

        public FixedAssetsController(IDepreciationService depreciationService, ILogger<FixedAssetsController> logger)
        {
            _depreciationService = depreciationService;
            _logger = logger;
        }

        // GET: Get fixedAsset(s)
        [HttpGet]
        [Route("tenants/{tenantId}/fixedAssets/{year:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<FixedAssetDTO>>> GetFixedAssetsAsync(Guid tenantId,int year, [FromQuery] bool includeDeleted = false)
        {
            return Ok(await _depreciationService.GetFixedAssetsYearDetailsAsync(tenantId, year));
        }
    }
}
