﻿using DocumentsAPI.Models;

namespace DocumentsAPI.Repositories
{
    public interface IFolderRepository
    {
        List<TreeFolderItem> ToFolderTreeView(List<Folder> folders);
        Task<bool> CheckFolderExists(Guid folderId);
        Task<bool> UpdateFolderHasDocumentsAsync(Guid folderId, bool status = true);
        Task<Folder> GetFolderByIdAsync(Guid? archiveId, Guid folderId);
        Task<Folder> InsertFolderAsync(Folder folder);
        Task<Folder> SetDeleteFolderAsync(Guid id, bool deleted, string userName);
        Task<Folder> UpdateFolderAsync(Folder folder);
        Task<IEnumerable<Folder>> GetChildrenAsync(Guid folderId, bool includeDeleted = false);
        Task<IEnumerable<Folder>> GetFoldersAsync(Guid? archiveId, bool includeDeleted = false);
        Task<IEnumerable<Folder>> UpdateChildrenArchiveAsync(Guid parentId, Guid oldArchiveId, Guid newArchiveId);
    }
}
