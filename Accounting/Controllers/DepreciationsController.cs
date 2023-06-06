using Accounting.Models;
using Accounting.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Accounting.Controllers
{
    [Authorize]
    public class DepreciationsController : Controller
    {
        private readonly IDepreciationRepository _depreciationRepo;
        private readonly IFixedAssetRepository _fixedAssetRepo;
        private readonly IValidator<Depreciation> _depreciationValidator;
        private readonly ILogger<DepreciationsController> _logger;

        public DepreciationsController(IDepreciationRepository depreciationRepo, IFixedAssetRepository fixedAssetRepo, IValidator<Depreciation> depreciationValidator, ILogger<DepreciationsController> logger)
        {
            _depreciationRepo = depreciationRepo;
            _fixedAssetRepo = fixedAssetRepo;
            _depreciationValidator = depreciationValidator;
            _logger = logger;
        }

        // GET: Get depreciation(s)
        [HttpGet]
        [Route("{tenantId}/depreciations")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Depreciation>>> GetAsync([FromQuery] bool includeDeleted = false)
        {
            return Ok(await _depreciationRepo.GetDepreciationsAsync(includeDeleted));
        }

        private async Task<bool> FixedAssetExists(Guid fixedAssetId)
        {
            FixedAsset? fixedAsset = await _fixedAssetRepo.GetFixedAssetByIdAsync(fixedAssetId);
            return (fixedAsset != null && fixedAsset?.Deleted == false);
        }

    }
}
