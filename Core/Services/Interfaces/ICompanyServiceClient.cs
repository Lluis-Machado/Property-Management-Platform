namespace CoreAPI.Services
{
    public interface ICompanyServiceClient
    {
        Task<string?> GetCompanyByIdAsync(Guid id);
        Task<string?> UpdateCompanyArchive(string companyId, string archiveId);
    }
}


