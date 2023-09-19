using LinkAPI.Models;
using MongoDB.Driver;

namespace LinkAPI.Repositories
{
    public interface ILinkRepository
    {
        Task<Link> InsertOneAsync(Link ownership);
        Task<List<Link>> GetAsync();
        Task<Link> UpdateAsync(Link ownership);
        Task<UpdateResult> SetDeleteAsync(Guid ownership, bool deleted);
        Task<Link> GetLinkByIdAsync(Guid id);
        Task<List<Link>> GetWithObjectAIdAsync(Guid id);
        Task<List<Link>> GetWithObjectBIdAsync(Guid id);


    }
}
