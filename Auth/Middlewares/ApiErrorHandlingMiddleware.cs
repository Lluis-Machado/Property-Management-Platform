using Authentication.Utils;

namespace Authentication.Middlewares
{
    public class ApiErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ApiErrorHandlingMiddleware> _logger;

        public ApiErrorHandlingMiddleware(ILogger<ApiErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ApiException e)
            {
                _logger.LogError("ApiException occurred: {@ErrorMessage}", e.ErrorMessage + "\n" + e.StackTrace);
                context.Response.StatusCode = (int)e.StatusCode;

#if DEVELOPMENT || STAGE
                context.Response.ContentType = "text/html";
                string responseContent = $"An error occurred: {e.Message}\n\nStack Trace:\n{e.StackTrace}";
                await context.Response.WriteAsync(responseContent);
#endif
            }
        }
    }
}
