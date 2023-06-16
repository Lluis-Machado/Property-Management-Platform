﻿using DocumentsAPI.Models;

namespace DocumentsAPI.Repositories
{
    public interface IFolderRepository
    {
        Task<Folder> GetFolderByIdAsync(Guid archiveId, int folderId);
        Task<Folder> GetFolderById(Guid folderId);
        Task<IEnumerable<Folder>> GetFoldersAsync(Guid archiveId, bool includeDeleted = false);
        Task<Folder> InsertFolderAsync(Folder folder);
        Task<Folder> SetDeleteFolderAsync(Guid id, bool deleted, string userName);
        Task<Folder> UpdateFolderAsync(Folder folder);
        List<TreeFolderItem> ToFolderTreeView(List<Folder> folders);
    }
}
