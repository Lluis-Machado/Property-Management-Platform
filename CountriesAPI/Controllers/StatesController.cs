using CountriesAPI.DTOs;
using CountriesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CountriesAPI.Controllers
{
    //[Authorize]
    public class StatesController : Controller
    {
        private readonly IStateService _stateService;
        private readonly ILogger<StatesController> _logger;

        public StatesController(IStateService stateService, ILogger<StatesController> logger)
        {
            _stateService = stateService;
            _logger = logger;
        }

        // GET: Get state(s)
        [HttpGet]
        [Route("states")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<StateDTO>>> GetStatesAsync([FromQuery] string? countryCode = null)
        {
            return Ok(await _stateService.GetStatesAsync(countryCode));
        }
    }
}
