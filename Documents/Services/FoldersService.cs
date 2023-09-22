using AutoMapper;
using DocumentsAPI.DTOs;
using DocumentsAPI.Models;
using DocumentsAPI.Repositories;
using System.Text;

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

            // TODO: Delete children and documents!
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

        public async Task<Folder?> GetFolderByIdAsync(Guid folderId)
        {
            return await _folderRepository.GetFolderByIdAsync(null, folderId);
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
            folder.CreatedAt = DateTime.Now;
            folder.LastUpdateAt = DateTime.Now;

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

        public async Task<List<TreeFolderItem>> ToFolderTreeView_Recursive(List<TreeFolderItem> folders, int level = 0)
        {
            List<TreeFolderItem> result = new();
            if (folders.Count == 0) return result;
            int lvl = level;
            _logger.LogInformation($"Recursive | Level {lvl} | Processing {folders.Count} children");

            foreach (var folder in folders)
            {
                var foldersList = (await _folderRepository.GetChildrenAsync(folder.Id)).ToList();
                var cock = new List<TreeFolderItem>();
                foldersList.ForEach(f => cock.Add(new TreeFolderItem(f)));
                var list = await ToFolderTreeView_Recursive(cock, lvl++);
                List<TreeFolderItem> children = new();
                list.ForEach(f =>
                {
                    children.Add(f);
                });
                folder.ChildFolders = children;
                result.Add(folder);
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
        public async Task<TreeFolderItem> CopyFolderAndChildren(Folder sourceFolder, UpdateFolderDTO folderDTO, string username, bool deleteOriginal = false)
        {
            var idMapping = new Dictionary<Guid, Guid>(); // To keep track of the mapping between original and copied folder IDs
            var visited = new HashSet<Guid>(); // To keep track of visited folders
            var stack = new Stack<TreeFolderItem>(); // Stack for DFS traversal
            var root = new TreeFolderItem(sourceFolder); // Create a copy of the sourceFolder as the root of the copied tree
            List<TreeFolderItem> returnFolders = new();


            Guid archiveId = folderDTO.ArchiveId;
            Guid? parentId = folderDTO.ParentId;

            //DEBUG
            StringBuilder log = new StringBuilder();

            log.AppendLine($"Starting {(deleteOriginal ? "move" : "copy")} of folder {sourceFolder.Id} ({sourceFolder.Name}) from archive {sourceFolder.ArchiveId} to archive {archiveId}");

            // Create copy of source folder
            var docs = await _documentsService.GetDocumentsAsync(sourceFolder.ArchiveId, null, sourceFolder.Id, false);
            Folder sourceFolderCopy = new Folder
            {
                Name = folderDTO.Name ?? sourceFolder.Name,
                ArchiveId = archiveId,
                ParentId = parentId,
                HasDocument = docs.Any(),
                CreatedAt = sourceFolder.CreatedAt,
                CreatedByUser = sourceFolder.CreatedByUser,
                Deleted = sourceFolder.Deleted
            };

            sourceFolderCopy.LastUpdateAt = DateTime.Now;
            sourceFolderCopy.LastUpdateByUser = username ?? "na";

            var root2 = await _folderRepository.InsertFolderAsync(sourceFolderCopy);
            idMapping.Add(sourceFolder.Id, root2.Id);
            stack.Push(new TreeFolderItem(sourceFolder));
            returnFolders.Add(new TreeFolderItem(root2));

            // Copy documents
            List<IFormFile> docBytes = new();
            foreach (var doc in docs)
            {
                var file = (await _documentsService.DownloadAsync(sourceFolder.ArchiveId, doc.Id));
                var fileContents = file.FileContents;
                var stream = new MemoryStream(fileContents);
                IFormFile formfile = new FormFile(stream, 0, fileContents.Length, null, doc.Name) { Headers = new HeaderDictionary(), ContentType = file.ContentType };
                docBytes.Add(formfile);
            }
            if (docs.Any()) await _documentsService.UploadAsync(archiveId, docBytes.ToArray(), root2.Id);

            while (stack.Count > 0)
            {
                var currentFolder = stack.Pop();
                if (!visited.Contains(currentFolder.Id)) visited.Add(currentFolder.Id); else continue;

                log.AppendLine($"\tProcessing folder {currentFolder.Id} ({currentFolder.Name}) - ParentID: {currentFolder.ParentId}");

                //var currentCopy = GetCopyById(root, currentFolder.Id); // Get the copy of the current folder in the copied tree
                if (currentFolder != null)
                {
                    var childFolders = await _folderRepository.GetChildrenAsync(currentFolder.Id, true);
                    log.AppendLine($"\t\tCurrent folder has {childFolders.Count()} children");
                    //if (idMapping.ContainsKey(currentFolder.Id)) _logger.LogInformation($"\tCurrent folder has id {currentFolder.Id} mapped to {idMapping[currentFolder.Id]}");
                    //else { _logger.LogWarning($"\t**Current folder has no ID mapping!!"); }

                    foreach (var childFolder in childFolders)
                    {
                        if (!visited.Contains(childFolder.Id) && stack.ToList().Where(item => item.Id == childFolder.Id).ToList().Count == 0 && idMapping.ContainsKey(currentFolder.Id))
                        {
                            log.AppendLine($"\t\t\tProcessing child {childFolder.Id} ({childFolder.Name}) - ParentID: {childFolder.ParentId}");

                            var childDocs = await _documentsService.GetDocumentsAsync(childFolder.ArchiveId, null, childFolder.Id, false);

                            //var childCopy = new TreeFolderItem(childFolder); // Create a copy of the child folder using TreeFolderItem
                            var childCopy = new Folder
                            {
                                ArchiveId = archiveId,
                                ParentId = childFolder.ParentId != null ? idMapping[(Guid)currentFolder.Id] : null,
                                HasDocument = childDocs.Any(),
                                CreatedAt = childFolder.CreatedAt,
                                CreatedByUser = childFolder.CreatedByUser,
                                Deleted = childFolder.Deleted,
                                Name = childFolder.Name,
                            };

                            childCopy.LastUpdateAt = DateTime.Now;
                            childCopy.LastUpdateByUser = username ?? "na";

                            var grandchildren = await _folderRepository.GetChildrenAsync(childFolder.Id, true);
                            foreach (var grandchild in grandchildren)
                            {
                                stack.Push(new TreeFolderItem(grandchild));
                            }

                            childCopy = await _folderRepository.InsertFolderAsync(childCopy);


                            // Copy documents
                            List<IFormFile> childDocBytes = new();
                            foreach (var doc in childDocs)
                            {
                                log.AppendLine($"\t\t\tProcessing document with name {doc.Name}");
                                var file = (await _documentsService.DownloadAsync(childFolder.ArchiveId, doc.Id));
                                var fileContents = file.FileContents;
                                var stream = new MemoryStream(fileContents);

                                IFormFile formfile = new FormFile(stream, 0, fileContents.Length, null, doc.Name) { Headers = new HeaderDictionary(), ContentType = file.ContentType};
                                childDocBytes.Add(formfile);

                            }
                            if (childDocs.Any()) await _documentsService.UploadAsync(archiveId, childDocBytes.ToArray(), childCopy.Id);


                            currentFolder.ChildFolders.Add(new TreeFolderItem(childCopy)); // Add the child copy to the current copy's child list
                            idMapping.Add(childFolder.Id, childCopy.Id);
                            var childf = new TreeFolderItem(childFolder);
                            stack.Push(childf);
                            returnFolders.Add(childf);
                        }
                    }


                    // Move-specific code
                    if (deleteOriginal)
                    {
                        log.AppendLine($"Deleting original folder {currentFolder.Id}");
                        await DeleteFolderAsync(currentFolder.Id, username);
                    }

                }
            }

            log.AppendLine($"DEBUG - Completed {(deleteOriginal ? "move" : "copy")} operation! Mapping: {Newtonsoft.Json.JsonConvert.SerializeObject(idMapping, Newtonsoft.Json.Formatting.Indented)}");
            _logger.LogInformation(log.ToString());

            return (await ToFolderTreeView_Recursive(returnFolders)).FirstOrDefault();
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
