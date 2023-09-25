﻿using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace CoreAPI.Middlewares
{
    public class ExecutionTimingFilter : IActionFilter
    {
        private readonly ILogger<ExecutionTimingFilter> _logger;
        private Stopwatch _stopwatch;

        public ExecutionTimingFilter(ILogger<ExecutionTimingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();
            _logger.LogInformation($"Action {context.ActionDescriptor.DisplayName} started at {DateTime.UtcNow:O}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            _logger.LogInformation($"Action {context.ActionDescriptor.DisplayName} ended at {DateTime.UtcNow:O} and took {_stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
