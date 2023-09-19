namespace CoreAPI.Services
{
    public interface ICoreService
    {
        Task<string> CreateProperty(string requestBody);
        Task<string> CreateCompany(string requestBody);
        Task<string> CreateContact(string requestBody);
        Task<string> CreateArchive(string requestBody, string? type);

        [Obsolete("Deprecated, Folder creation is handled automatically in the Documents microservice")]
        Task<string> CreateFolder(string requestBody, string archiveId);

        Task<object> GetContact(Guid Id);
        Task<object> GetProperty(Guid Id);
        Task<object> GetCompany(Guid Id);
        Task<object> GetContacts(bool includeDeleted);
        Task<object> GetProperties(bool includeDeleted);
        Task<object> GetCompanies(bool includeDeleted);

    }
}