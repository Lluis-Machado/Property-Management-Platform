using AutoMapper;
using DocumentsAPI.DTOs;
using DocumentsAPI.Models;
using DocumentsAPI.Repositories;

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

        public async Task<List<TreeFolderItem>> GetFolderByIdAsync(Guid folderId)
        {
            var result = await _folderRepository.GetFolderByIdAsync(null, folderId);

            List<Folder> folderList = new()
            {
                result
            };

            return ToFolderTreeView(folderList);
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

        // Update ArchiveId for every child of a parent
        public async Task<List<TreeFolderItem>> UpdateChildrenArchiveAsync(Guid parentId, Guid oldArchiveId, Guid newArchiveId)
        {
            return ToFolderTreeView((await _folderRepository.UpdateChildrenArchiveAsync(parentId, oldArchiveId, newArchiveId)).ToList());
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





        public async Task<TreeFolderItem> CopyFolderAndChildren(Folder sourceFolder, Guid archiveId, Guid? parentId)
        {
            var idMapping = new Dictionary<Guid, Guid>(); // To keep track of the mapping between original and copied folder IDs
            var visited = new HashSet<Guid>(); // To keep track of visited folders
            var stack = new Stack<TreeFolderItem>(); // Stack for DFS traversal
            var root = new TreeFolderItem(sourceFolder); // Create a copy of the sourceFolder as the root of the copied tree

            // Create copy of source folder
            Folder sourceFolderCopy = new Folder
            {
                Name = sourceFolder.Name,
                ArchiveId = archiveId,
                ParentId = parentId ?? null,
                HasDocument = sourceFolder.HasDocument,
                LastUpdateAt = DateTime.Now,
                // LastUpdatedBy ?
            };

            var root2 = await _folderRepository.InsertFolderAsync(sourceFolderCopy);
            stack.Push(new TreeFolderItem(root2));

            while (stack.Count > 0)
            {
                var currentFolder = stack.Pop();
                visited.Add(currentFolder.Id);

                var currentCopy = GetCopyById(root, currentFolder.Id); // Get the copy of the current folder in the copied tree
                if (currentCopy != null)
                {
                    foreach (var childFolder in currentFolder.ChildFolders)
                    {
                        if (!visited.Contains(childFolder.Id))
                        {
                            var childCopy = new TreeFolderItem(childFolder); // Create a copy of the child folder using TreeFolderItem
                            currentCopy.ChildFolders.Add(childCopy); // Add the child copy to the current copy's child list
                            stack.Push(childFolder);
                        }
                    }
                }
            }

            return root;
        }

        private TreeFolderItem GetCopyById(TreeFolderItem root, Guid id)
        {
            if (root.Id == id)
                return root;

            foreach (var child in root.ChildFolders)
            {
                var found = GetCopyById(child, id);
                if (found != null)
                    return found;
            }

            return null;
        }




    }
}
