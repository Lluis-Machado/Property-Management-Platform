using LogsAPI.Models;
using LogsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaxManagement.Security;

namespace LogsAPI.Controllers
{
    public class LogsController : Controller
    {
        private readonly LogsRepository _logsService;

        public LogsController(LogsRepository logsService) =>
            _logsService = logsService;

        [HttpGet]
        [Authorize]
        [Route("logs")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<List<Log>> Get() =>
         await _logsService.GetAsync();
    }
}
