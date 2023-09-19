namespace CoreAPI.Services
{
    public interface ICoreService
    {
        Task<string> CreateProperty(string requestBody, IHttpContextAccessor contextAccessor);
        Task<string> CreateCompany(string requestBody, IHttpContextAccessor contextAccessor);
        Task<string> CreateContact(string requestBody, IHttpContextAccessor contextAccessor);
        Task<string> CreateArchive(string requestBody, IHttpContextAccessor contextAccessor, string? type);

        [Obsolete("Deprecated, Folder creation is handled automatically in the Documents microservice")]
        Task<string> CreateFolder(string requestBody, string archiveId, IHttpContextAccessor contextAccessor);

        Task<object> GetContact(Guid Id, IHttpContextAccessor contextAccessor);
        Task<object> GetProperty(Guid Id, IHttpContextAccessor contextAccessor);
        Task<object> GetCompany(Guid Id, IHttpContextAccessor contextAccessor);
        Task<object> GetContacts(bool includeDeleted, IHttpContextAccessor contextAccessor);
        Task<object> GetProperties(bool includeDeleted, IHttpContextAccessor contextAccessor);
        Task<object> GetCompanies(bool includeDeleted, IHttpContextAccessor contextAccessor);

    }
}