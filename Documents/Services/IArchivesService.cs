using DocumentsAPI.Models;
using static DocumentsAPI.Models.Archive;

namespace DocumentsAPI.Services
{
    public interface IArchivesService
    {
        Task<Archive> CreateArchiveAsync(Archive archive, ARCHIVE_TYPE type = ARCHIVE_TYPE.NONE, Guid? objectId = null);
        Task<IEnumerable<Archive>> GetArchivesAsync(bool includeDeleted = false);
        Task UpdateArchiveAsync(Guid archiveId, string newName);
        Task DeleteArchiveAsync(Guid archiveId);
        Task UndeleteArchiveAsync(Guid archiveId);
    }
}
