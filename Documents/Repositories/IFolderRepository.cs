using DocumentsAPI.Models;

namespace DocumentsAPI.Repositories
{
    public interface IFolderRepository
    {
        Task<bool> CheckFolderExists(Guid folderId);
        Task<Folder> GetFolderByIdAsync(Guid? archiveId, Guid folderId);
        Task<IEnumerable<Folder>> GetFoldersAsync(Guid? archiveId, bool includeDeleted = false);
        Task<Folder> InsertFolderAsync(Folder folder);
        Task<Folder> SetDeleteFolderAsync(Guid id, bool deleted, string userName);
        Task<Folder> UpdateFolderAsync(Folder folder);
        Task<IEnumerable<Folder>> UpdateChildrenArchiveAsync(Guid parentId, Guid oldArchiveId, Guid newArchiveId);
        Task<IEnumerable<Folder>> GetChildrenAsync(Guid folderId, bool includeDeleted = false);
        List<TreeFolderItem> ToFolderTreeView(List<Folder> folders);
        Task<bool> UpdateFolderHasDocumentsAsync(Guid folderId, bool status = true);
    }
}
