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
            catch (ApiException ex)
            {
                _logger.LogError("ApiException ocurred: {@ErrorMessage}", ex.ErrorMessage);

                context.Response.StatusCode = (int)ex.StatusCode;
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(ex.ErrorMessage);
            }
        }
    }
}
