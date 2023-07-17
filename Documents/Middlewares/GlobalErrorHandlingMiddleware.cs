using System.Net;

namespace DocumentsAPI.Middlewares
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

                bool hasDeveloperPermission = context.User.Claims.Any(c => c.Type == "permissions" && c.Value == "admin");

                if (hasDeveloperPermission)
                {
                    context.Response.ContentType = "text/plain";
                    string responseContent = $"An error occurred: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}";
                    await context.Response.WriteAsync(responseContent);
                }
            }
        }
    }
}
