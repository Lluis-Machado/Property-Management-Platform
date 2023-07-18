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
        private readonly ILogger<FoldersService> _logger;
        private readonly IDocumentsService _documentsService;


        public FoldersService(IFolderRepository folderRepository, IMapper mapper, ILogger<FoldersService> logger, IDocumentsService documentsService)
        {
            _folderRepository = folderRepository;
            _mapper = mapper;
            _logger = logger;
            _documentsService = documentsService;
        }

        public IFolderRepository GetFolderRepository() { return _folderRepository; }

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

            List<Folder> rootFolders = folders.FindAll(f => f?.ParentId == null).ToList();
            List<Folder> childFolders = folders.FindAll(f => f?.ParentId != null).ToList();

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




        // TODO: Optimize, change 'CreatedByUser' fields
        // TODO: Add documents copy from one folder to the other
        public async Task<TreeFolderItem> CopyFolderAndChildren(Folder sourceFolder, Guid archiveId, Guid? parentId = null)
        {
            var idMapping = new Dictionary<Guid, Guid>(); // To keep track of the mapping between original and copied folder IDs
            var visited = new HashSet<Guid>(); // To keep track of visited folders
            var stack = new Stack<TreeFolderItem>(); // Stack for DFS traversal
            var root = new TreeFolderItem(sourceFolder); // Create a copy of the sourceFolder as the root of the copied tree

            _logger.LogInformation($"Starting copy of folder {sourceFolder.Id} ({sourceFolder.Name}) from archive {sourceFolder.ArchiveId} to archive {archiveId}");

            // Create copy of source folder
            Folder sourceFolderCopy = new Folder
            {
                Name = sourceFolder.Name,
                ArchiveId = archiveId,
                ParentId = parentId,
                HasDocument = sourceFolder.HasDocument,
                CreatedAt = DateTime.Now,
                CreatedByUser = "COPY TEST",
                Deleted = sourceFolder.Deleted
                // LastUpdatedBy ?
            };

            var root2 = await _folderRepository.InsertFolderAsync(sourceFolderCopy);
            idMapping.Add(sourceFolder.Id, root2.Id);
            stack.Push(new TreeFolderItem(sourceFolder));

            // Copy documents
            var docs = await _documentsService.GetDocumentsAsync(sourceFolder.ArchiveId, null, sourceFolder.Id, true);
            List<IFormFile> docBytes = new();
            foreach (var doc in docs)
            {
                var file = (await _documentsService.DownloadAsync(sourceFolder.ArchiveId, doc.Id)).FileContents;
                using (var stream = new MemoryStream(file))
                {
                    IFormFile formfile = new FormFile(stream, 0, file.Length, doc.Name, doc.Name);
                    docBytes.Add(formfile);
                }
            }
            await _documentsService.UploadAsync(archiveId, docBytes.ToArray(), parentId);

            while (stack.Count > 0)
            {
                var currentFolder = stack.Pop();
                visited.Add(currentFolder.Id);

                _logger.LogInformation($"\tProcessing folder {currentFolder.Id} ({currentFolder.Name}) - ParentID: {currentFolder.ParentId}");

                //var currentCopy = GetCopyById(root, currentFolder.Id); // Get the copy of the current folder in the copied tree
                if (currentFolder != null)
                {
                    var childFolders = await _folderRepository.GetChildrenAsync(currentFolder.Id, true);
                    _logger.LogInformation($"\t\tCurrent folder has {childFolders.Count()} children");
                    if (idMapping.ContainsKey(currentFolder.Id)) _logger.LogInformation($"\tCurrent folder has id {currentFolder.Id} mapped to {idMapping[currentFolder.Id]}");
                    else { _logger.LogWarning($"\t**Current folder has no ID mapping!!"); }

                    foreach (var childFolder in childFolders)
                    {
                        if (!visited.Contains(childFolder.Id))
                        {
                            _logger.LogInformation($"\t\t\tProcessing child {childFolder.Id} ({childFolder.Name}) - ParentID: {childFolder.ParentId}");

                            //var childCopy = new TreeFolderItem(childFolder); // Create a copy of the child folder using TreeFolderItem
                            var childCopy = new Folder
                            {
                                ArchiveId = archiveId,
                                ParentId = childFolder.ParentId != null ? idMapping[(Guid)childFolder.ParentId] : null,
                                HasDocument = childFolder.HasDocument,
                                CreatedAt = DateTime.Now,
                                Deleted = childFolder.Deleted,
                                Name = childFolder.Name,
                                CreatedByUser = "COPY TEST CHILD"
                            };

                            var grandchildren = await _folderRepository.GetChildrenAsync(childFolder.Id, true);
                            foreach (var grandchild in grandchildren)
                            {
                                stack.Push(new TreeFolderItem(grandchild));
                            }

                            childCopy = await _folderRepository.InsertFolderAsync(childCopy);


                            // Copy documents
                            var childDocs = await _documentsService.GetDocumentsAsync(childFolder.ArchiveId, null, childFolder.Id, true);
                            List<IFormFile> childDocBytes = new();
                            foreach (var doc in childDocs)
                            {
                                var file = (await _documentsService.DownloadAsync(childFolder.ArchiveId, doc.Id)).FileContents;
                                using (var stream = new MemoryStream(file))
                                {
                                    IFormFile formfile = new FormFile(stream, 0, file.Length, doc.Name, doc.Name);
                                    childDocBytes.Add(formfile);
                                }
                            }
                            await _documentsService.UploadAsync(archiveId, docBytes.ToArray(), childCopy.Id);


                            currentFolder.ChildFolders.Add(new TreeFolderItem(childCopy)); // Add the child copy to the current copy's child list
                            idMapping.Add(childFolder.Id, childCopy.Id);
                            var childf = new TreeFolderItem(childFolder);
                            stack.Push(childf);
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
