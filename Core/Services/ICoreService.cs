namespace CoreAPI.Services
{
    public interface ICoreService
    {
        Task<string> CreateProperty(string requestBody, IHttpContextAccessor contextAccessor);
        Task<string> CreateArchive(string requestBody, IHttpContextAccessor contextAccessor, string? type);
        [Obsolete("Deprecated, Folder creation is handled automatically in the Documents microservice")]
        Task<string> CreateFolder(string requestBody, string archiveId, IHttpContextAccessor contextAccessor);
        Task<object> GetContact(Guid Id, IHttpContextAccessor contextAccessor);
        Task<object> GetProperty(Guid Id, IHttpContextAccessor contextAccessor);
    }
}