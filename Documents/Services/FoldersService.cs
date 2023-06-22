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

        public bool CheckFolderExist(Guid folderId)
        {

            var folder = _folderRepository.GetFolderById(folderId);
            return folder != null; 
        }

        public async Task<ActionResult<FolderDTO>> UpdateFolderAsync(FolderDTO folderDTO, string userName)
        {
            var folder = _mapper.Map<FolderDTO, Folder>(folderDTO);

            folder.LastUpdateByUser = userName;
            folder.LastUpdateAt = DateTime.Now;

            var result = await _folderRepository.UpdateFolderAsync(folder);

            folderDTO = _mapper.Map<Folder, FolderDTO>(result);

            return new ActionResult<FolderDTO>(folderDTO);
        }

        public async Task<FolderDTO> DeleteFolderAsync(Guid folderId, string userName)
        {
            var result = await _folderRepository.SetDeleteFolderAsync(folderId, true, userName);
            var folderDTO = _mapper.Map<Folder, FolderDTO>(result);

            return folderDTO;
        }      
        public async Task<FolderDTO> UnDeleteFolderAsync(Guid folderId, string userName)
        {
            var result = await _folderRepository.SetDeleteFolderAsync(folderId, false, userName);
            var folderDTO = _mapper.Map<Folder, FolderDTO>(result);

            return folderDTO;
        }

        public async Task<List<TreeFolderItem>> GetFoldersAsync(Guid ArchiveId, bool includeDeleted = false)
        {
            var result = await _folderRepository.GetFoldersAsync(ArchiveId, includeDeleted);
            IEnumerable<FolderDTO> folderDTO = _mapper.Map<IEnumerable<Folder>, IEnumerable<FolderDTO>>(result);

            return ToFolderTreeView(result.ToList());
        }

        public async Task<FolderDTO> CreateFolderAsync(Guid ArchiveId, FolderDTO folderDTO, string userName)
        {
            var folder = _mapper.Map<FolderDTO, Folder>(folderDTO);

            folder.CreatedByUser = userName;
            folder.LastUpdateByUser = userName;
            folder.LastUpdateAt = DateTime.UtcNow;
            folder.ArchiveId = ArchiveId;

            folder = await _folderRepository.InsertFolderAsync(folder);

            folderDTO = _mapper.Map<Folder, FolderDTO>(folder);

            return folderDTO;
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
