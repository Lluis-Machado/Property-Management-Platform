using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TaxManagement.Middelwares
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
            catch (Exception e)
            {
                _logger.LogError("Internal exception ocurred: {@Exception}", e);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

#if PRODUCTION == false
                ProblemDetails problem = new()
                {
                    Type = "Internal Server error",
                    Title = e.Message,
                    Detail = e.StackTrace
                };
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(problem);
#endif
            }

        }
    }
}
