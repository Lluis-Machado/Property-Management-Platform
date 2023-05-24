using Documents.Models;

namespace DocumentsAPI.Repositories
{
    public interface IArchiveRepository
    {
        Task CreateArchiveAsync(Archive archive);
        Task<IEnumerable<Archive>> GetArchivesAsync(int? segmentSize, bool includeDeleted = false);
        Task<bool> ArchiveExistsAsync(Guid archiveId);
        Task DeleteArchiveAsync(Guid archiveId);
        Task UndeleteArchiveAsync(Guid archiveId);
    }
}
