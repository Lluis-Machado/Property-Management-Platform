using AuditsAPI.Models;
using MongoDB.Driver;

namespace AuditsAPI.Repositories
{
    public interface IAuditRepository
    {
        Task<Audit> InsertOneAsync(Audit audit);
        Task<List<Audit>> GetAsync(bool includeDeleted = false);
        Task<Audit> UpdateAsync(Audit audit);
        Task<UpdateResult> SetDeleteAsync(Guid id, bool deleted, string lastUser);
        Task<Audit> GetByIdAsync(Guid id);
        Task<bool> CheckIfAnyExistAsync(Guid id);
    }
}