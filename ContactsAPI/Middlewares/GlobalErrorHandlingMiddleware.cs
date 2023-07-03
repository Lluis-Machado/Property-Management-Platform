using System.Net;

namespace ContactsAPI.Middlewares
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

                //ProblemDetails problem = new()
                //{
                //    Type = "Server error",
                //    Title = "Server error",
                //    Detail = "Internal server error ocurred"
                //};

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                //string problemJson = JsonSerializer.Serialize(problem);
                //await context.Response.WriteAsJsonAsync(problemJson);

                //context.Response.ContentType = "application/json";
            }

        }
    }
}
