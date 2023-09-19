using MongoDB.Driver;
using OwnershipAPI.Models;

namespace OwnershipAPI.Repositories
{
    public interface IOwnershipRepository
    {
        Task<Ownership> InsertOneAsync(Ownership ownership);
        Task<List<Ownership>> GetAsync();
        Task<Ownership> UpdateAsync(Ownership ownership);
        Task<UpdateResult> SetDeleteAsync(Guid ownership, bool deleted);
        Task<Ownership> GetOwnershipByIdAsync(Guid id);
        Task<List<Ownership>> GetWithContactIdAsync(Guid id);
        Task<List<Ownership>> GetWithCompanyIdAsync(Guid id);

        Task<List<Ownership>> GetWithPropertyIdAsync(Guid id);


    }
}