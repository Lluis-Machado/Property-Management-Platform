namespace CoreAPI.Services
{
    public interface ICoreService
    {
        Task<string> CreateProperty(string requestBody);
        Task<string> CreateCompany(string requestBody);
        Task<string> CreateContact(string requestBody);
        Task<string> CreateArchive(string requestBody, string? type);

        Task<string> UpdateProperty(string requestBody);
        Task<string> UpdateCompany(string requestBody);
        Task<string> UpdateContact(string requestBody);


        Task<object> GetContact(Guid Id);
        Task<object> GetProperty(Guid Id);
        Task<object> GetCompany(Guid Id);
        Task<object> GetContacts(bool includeDeleted);
        Task<object> GetProperties(bool includeDeleted);
        Task<object> GetCompanies(bool includeDeleted);

    }
}