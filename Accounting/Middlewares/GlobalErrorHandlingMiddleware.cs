﻿using System.Diagnostics;
using System.Net;

namespace AccountingAPI.Middlewares
{
    public class GlobalErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

        public GlobalErrorHandlingMiddleware(ILogger<GlobalErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal exception occurred: {@Exception}", ex);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}
