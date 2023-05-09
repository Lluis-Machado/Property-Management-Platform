﻿using LogsAPI.Models;
using LogsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LogsAPI.Controllers
{
    [Authorize]
    public class LogsController : Controller
    {
        private readonly LogsService _logsService;

        public LogsController(LogsService logsService) =>
            _logsService = logsService;

        [HttpGet]
        [Route("logs")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<List<Log>> Get() =>
         await _logsService.GetAsync();
    }
}
