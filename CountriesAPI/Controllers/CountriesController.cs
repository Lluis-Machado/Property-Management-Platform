using CountriesAPI.DTOs;
using CountriesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CountriesAPI.Controllers
{
    [Authorize]
    public class CountriesController : Controller
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(ICountryService countryService, ILogger<CountriesController> logger)
        {
            _countryService = countryService;
            _logger = logger;
        }

        // GET: Get countries(s)
        [HttpGet]
        [Route("countries")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<CountryDTO>>> GetCountriesAsync([FromQuery] string? languageCode = null)
        {
            return Ok(await _countryService.GetCountriesAsync(languageCode));
        }
    }
}
