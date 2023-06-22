﻿using DocumentsAPI.DTOs;
using DocumentsAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentsAPI.Services
{
    public interface IFoldersService
    {
        bool CheckFolderExist(Guid folderId);
        Task<ActionResult<FolderDTO>> UpdateFolderAsync(FolderDTO folderDTO, string userName);
        Task<FolderDTO> DeleteFolderAsync(Guid folderId, string userName);
        Task<FolderDTO> UnDeleteFolderAsync(Guid folderId, string userName);
        Task<List<TreeFolderItem>> GetFoldersAsync(Guid archiveId, bool includeDeleted);
        Task<FolderDTO> CreateFolderAsync(Guid ArchiveId, FolderDTO folderDTO, string userName);

    }
}