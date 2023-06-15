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
        private readonly IFixedAssetService _fixedAssetService;
        private readonly ILogger<FixedAssetsController> _logger;

        public FixedAssetsController(IFixedAssetService fixedAssetService, ILogger<FixedAssetsController> logger)
        {
            _fixedAssetService = fixedAssetService;
            _logger = logger;
        }

        // GET: Get fixedAsset(s)
        [HttpGet]
        [Route("tenants/{tenantId}/fixedAssets")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<FixedAssetDTO>>> GetAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _fixedAssetService.GetFixedAssetsAsync());
        }
    }
}
