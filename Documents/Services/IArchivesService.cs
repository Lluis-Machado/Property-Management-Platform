using Documents.Models;

namespace Archives.Services
{
    public interface IArchivesService
    {
        Task<Archive> CreateArchiveAsync(Archive archive);
        Task<IEnumerable<Archive>> GetArchivesAsync(bool includeDeleted = false);
        Task DeleteArchiveAsync(Guid archiveId);
        Task UndeleteArchiveAsync(Guid archiveId);
    }
}
