using CompaniesAPI.Models;
using MongoDB.Driver;

namespace CompaniesAPI.Repositories
{
    public interface ICompaniesRepository
    {
        Task<Company> InsertOneAsync(Company company);
        Task<List<Company>> GetAsync(bool includeDeleted = false);
        Task<Company> UpdateAsync(Company company);
        Task<UpdateResult> SetDeleteAsync(Guid companyId, bool deleted, string lastUser);
        Task<Company> GetByIdAsync(Guid companyId);
    }
}