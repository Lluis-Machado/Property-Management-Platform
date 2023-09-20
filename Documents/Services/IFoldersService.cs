using DocumentsAPI.DTOs;
using DocumentsAPI.Models;
using DocumentsAPI.Repositories;

namespace DocumentsAPI.Services
{
    public interface IFoldersService
    {
        Task<bool> CheckFolderExist(Guid folderId);
        Task<FolderDTO> UpdateFolderAsync(UpdateFolderDTO folderDTO, Guid folderId, string userName);
        Task<FolderDTO> DeleteFolderAsync(Guid folderId, string userName);
        Task<FolderDTO> UnDeleteFolderAsync(Guid folderId, string userName);
        Task<List<TreeFolderItem>> GetFoldersAsync(Guid? archiveId, bool includeDeleted);
        Task<Folder?> GetFolderByIdAsync(Guid folderId);
        Task<Folder> CreateFolderAsync(Guid archiveId, CreateFolderDTO createFolderDTO, string userName);
        Task<bool> UpdateFolderHasDocuments(Guid folderId, bool status = true);
        Task<List<TreeFolderItem>> UpdateChildrenArchiveAsync(Guid parentId, Guid oldArchiveId, Guid newArchiveId);
        Task<TreeFolderItem> CopyFolderAndChildren(Folder sourceFolder, UpdateFolderDTO folderDTO, string username, bool deleteOriginal = false);
        IFolderRepository GetFolderRepository();

    }
}
