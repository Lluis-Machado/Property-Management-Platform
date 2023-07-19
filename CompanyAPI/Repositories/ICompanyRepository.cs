using CompanyAPI.Models;
using MongoDB.Driver;

namespace CompanyAPI.Repositories
{
    public interface ICompanyRepository
    {
        Task<Company> InsertOneAsync(Company company);
        Task<List<Company>> GetAsync(bool includeDeleted = false);
        Task<Company> UpdateAsync(Company company);
        Task<UpdateResult> SetDeleteAsync(Guid companyId, bool deleted, string lastUser);
        Task<Company> GetByIdAsync(Guid companyId);
    }
}
