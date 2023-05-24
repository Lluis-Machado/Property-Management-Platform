using DocumentsAPI.Models;

namespace DocumentsAPI.Repositories
{
    public interface IFolderRepository
    {
        Task<Folder?> GetFolderByIdAsync(Guid archiveId, int folderId);
        Task<IEnumerable<Folder>> GetFoldersAsync(Guid archiveId, bool includeDeleted);
        Task<Folder> InsertFolderAsync(Folder folder);
        Task<int> SetDeleteFolderAsync(Guid id, bool deleted);
        Task<int> UpdateFolderAsync(Folder folder);
    }
}
