using AutoMapper;
using DocumentsAPI.DTOs;
using DocumentsAPI.Models;
using DocumentsAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DocumentsAPI.Services
{
    public class FoldersService : IFoldersService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IMapper _mapper;


        public FoldersService(IFolderRepository folderRepository, IMapper mapper)
        {
            _folderRepository = folderRepository;
            _mapper = mapper;

        }

        public async Task<bool> CheckFolderExist(Guid folderId)
        {
            return await _folderRepository.CheckFolderExists(folderId);
        }

        public async Task<FolderDTO> UpdateFolderAsync(UpdateFolderDTO folderDTO, Guid folderId, string userName)
        {
            if (!await CheckFolderExist(folderId)) throw new Exception($"Folder {folderId} not found");

            var folder = _mapper.Map<UpdateFolderDTO, Folder>(folderDTO);

            folder.Id = folderId;
            folder.LastUpdateByUser = userName;
            folder.LastUpdateAt = DateTime.Now;

            var result = await _folderRepository.UpdateFolderAsync(folder);

            FolderDTO folderDTOResult = _mapper.Map<Folder, FolderDTO>(result);

            return folderDTOResult;
        }

        public async Task<FolderDTO> DeleteFolderAsync(Guid folderId, string userName)
        {
            if (!await CheckFolderExist(folderId)) throw new Exception($"Folder {folderId} not found");

            var result = await _folderRepository.SetDeleteFolderAsync(folderId, true, userName);
            var folderDTO = _mapper.Map<Folder, FolderDTO>(result);

            return folderDTO;
        }
        public async Task<FolderDTO> UnDeleteFolderAsync(Guid folderId, string userName)
        {
            if (!await CheckFolderExist(folderId)) throw new Exception($"Folder {folderId} not found");

            var result = await _folderRepository.SetDeleteFolderAsync(folderId, false, userName);
            var folderDTO = _mapper.Map<Folder, FolderDTO>(result);

            return folderDTO;
        }

        public async Task<List<TreeFolderItem>> GetFoldersAsync(Guid? ArchiveId, bool includeDeleted = false)
        {
            var result = await _folderRepository.GetFoldersAsync(ArchiveId, includeDeleted);
            IEnumerable<FolderDTO> folderDTO = _mapper.Map<IEnumerable<Folder>, IEnumerable<FolderDTO>>(result);

            return ToFolderTreeView(result.ToList());
        }

        public async Task<Folder> CreateFolderAsync(Guid archiveId, CreateFolderDTO createFolderDTO, string userName)
        {
            var folder = _mapper.Map<CreateFolderDTO, Folder>(createFolderDTO);

            folder.ArchiveId = archiveId;
            folder.CreatedByUser = userName;
            folder.LastUpdateByUser = userName;
            folder.LastUpdateAt = DateTime.UtcNow;

            folder = await _folderRepository.InsertFolderAsync(folder);

            return folder;
        }

        public List<TreeFolderItem> ToFolderTreeView(List<Folder> folders)
        {
            List<TreeFolderItem> result = new();

            List<Folder> rootFolders = folders.FindAll(f => f.ParentId == null).ToList();
            List<Folder> childFolders = folders.FindAll(f => f.ParentId != null).ToList();

            foreach (var rootFolder in rootFolders)
            {
                TreeFolderItem parentFolder = new(rootFolder);
                AddChilds(ref parentFolder, childFolders);
                result.Add(parentFolder);
            }
            return result;
        }

        public async Task<bool> UpdateFolderHasDocuments(Guid folderId, bool status = true)
        {
            return await _folderRepository.UpdateFolderHasDocumentsAsync(folderId, status);
        }


        private void AddChilds(ref TreeFolderItem parentFolder, List<Folder> folders)
        {
            if (parentFolder == null) return;
            Guid? parentId = parentFolder.Id;
            List<Folder> childFolders = folders.FindAll(f => f.ParentId == parentId).ToList();
            List<TreeFolderItem> treeChildFolders = new();
            foreach (var childFolder in childFolders)
            {
                TreeFolderItem treeChildFolder = new(childFolder);
                AddChilds(ref treeChildFolder, folders);
                treeChildFolders.Add(treeChildFolder);
            }
            parentFolder.ChildFolders = treeChildFolders;
        }
    }
}
