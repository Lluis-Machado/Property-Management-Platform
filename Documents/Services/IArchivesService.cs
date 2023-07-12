using DocumentsAPI.Models;

namespace DocumentsAPI.Services
{
    public interface IArchivesService
    {
        Task<Archive> CreateArchiveAsync(Archive archive);
        Task<IEnumerable<Archive>> GetArchivesAsync(bool includeDeleted = false);
        Task UpdateArchiveAsync(Guid archiveId, string newName);
        Task DeleteArchiveAsync(Guid archiveId);
        Task UndeleteArchiveAsync(Guid archiveId);
    }
}
