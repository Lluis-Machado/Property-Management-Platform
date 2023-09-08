namespace CoreAPI.Services
{
    public interface ICoreService
    {
        Task<string> CreateProperty(string requestBody);
        Task<string> CreateArchive(string requestBody);
        Task<string> CreateFolder(string requestBody, string archiveId);
        Task<object> GetContact(Guid Id);
        Task<object> GetProperty(Guid Id);
    }
}