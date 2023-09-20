using LogsAPI.Models;
using LogsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LogsAPI.Controllers
{
#if PRODUCTION
    [Authorize]
#endif
    public class LogsController : Controller
    {
        private readonly LogsService _logsService;

        public LogsController(LogsService logsService)
        {
            _logsService = logsService;
        }

        [HttpGet]
        [Route("logs")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<List<Log>> Get([FromQuery] DateTime? periodStart = null, [FromQuery] DateTime? periodEnd = null)
        {
            return await _logsService.GetLogsAsync(periodStart, periodEnd);
        }
    }
}
