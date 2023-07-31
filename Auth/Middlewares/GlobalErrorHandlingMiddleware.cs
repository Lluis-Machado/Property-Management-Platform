using System.Net;

namespace Authentication.Middlewares
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
                _logger.LogError("Internal exception occurred: {@Exception}", e);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

#if DEVELOPMENT || STAGE
                context.Response.ContentType = "text/html";
                string responseContent = $"An error occurred: {e.Message}\n\nStack Trace:\n{e.StackTrace}";
                await context.Response.WriteAsync(responseContent);
#endif
            }
        }
    }
}