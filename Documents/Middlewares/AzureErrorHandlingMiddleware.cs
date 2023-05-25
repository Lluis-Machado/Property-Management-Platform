using Azure;

namespace Documents.Middelwares
{
    public class AzureErrorHandlingMiddleware: IMiddleware
    {
        private readonly ILogger<AzureErrorHandlingMiddleware> _logger;

        private Dictionary<string, string> _errors;

        public AzureErrorHandlingMiddleware(ILogger<AzureErrorHandlingMiddleware> logger)
        {
            _logger = logger;
            _errors = new Dictionary<string, string>
            {
                {"ContainerAlreadyExists", "Archive already exists"},
                {"ContainerNotFound", "Archive not found"},
                {"BlobNotFound", "Document not found"},
                
            };
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (RequestFailedException e)
            {
                if (e.ErrorCode == null) throw;
                if (!_errors.ContainsKey(e.ErrorCode)) throw;

                string errorMessage = _errors[e.ErrorCode];
                context.Response.StatusCode = e.Status;
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(errorMessage);
            }
        }
    }
}
