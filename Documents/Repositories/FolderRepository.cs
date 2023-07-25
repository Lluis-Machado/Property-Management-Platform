using Dapper;
using DocumentsAPI.Contexts;
using DocumentsAPI.Models;
using DocumentsAPI.Repositories;
using System.Text;

namespace DocumentsAPI.Repositories
{
    public class FolderRepository : IFolderRepository

    {
        private readonly DapperContext _context;
        private readonly ILogger<FolderRepository> _logger;

        public FolderRepository(DapperContext context, ILogger<FolderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> CheckFolderExists(Guid folderId)
        {
            var parameters = new
            {
                folderId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id FROM Folders");
            queryBuilder.Append(" WHERE Id = @folderId");

            var result = await _context
                .CreateConnection()
                .QuerySingleOrDefaultAsync<Folder>(queryBuilder.ToString(), parameters);

            return result != default;
        }

        public async Task<Folder> GetFolderByIdAsync(Guid? archiveId, Guid folderId)
        {
            var parameters = new
            {
                archiveId,
                folderId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT ");
            queryBuilder.Append(" Id");
            queryBuilder.Append(",ArchiveId");
            queryBuilder.Append(",Name");
            queryBuilder.Append(",ParentId");
            queryBuilder.Append(",HasDocument");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedByUser");
            queryBuilder.Append(",LastUpdateAt");
            queryBuilder.Append(",LastUpdateByUser");
            queryBuilder.Append(" FROM Folders");
            queryBuilder.Append(" WHERE Id = @folderId");
            if (archiveId != null) queryBuilder.Append(" AND ArchiveId = @archiveId");

            return await _context
                .CreateConnection()
                .QuerySingleOrDefaultAsync<Folder>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<Folder>> GetFoldersAsync(Guid? archiveId, bool includeDeleted = false)
        {
            var parameters = new
            {
                archiveId,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT ");
            queryBuilder.Append(" Id");
            queryBuilder.Append(",ArchiveId");
            queryBuilder.Append(",Name");
            queryBuilder.Append(",ParentId");
            queryBuilder.Append(",HasDocument");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedByUser");
            queryBuilder.Append(",LastUpdateAt");
            queryBuilder.Append(",LastUpdateByUser");
            queryBuilder.Append(" FROM Folders");
            queryBuilder.Append(" WHERE 1 = 1");
            if (archiveId != null) queryBuilder.Append(" AND ArchiveId = @archiveId");
            if (includeDeleted == false) queryBuilder.Append(" AND Deleted = 0");

            return await _context
                .CreateConnection()
                .QueryAsync<Folder>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<Folder>> GetChildrenAsync(Guid parentId, bool includeDeleted = false)
        {

            var parameters = new
            {
                parentId,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT ");
            queryBuilder.Append(" Id");
            queryBuilder.Append(",ArchiveId");
            queryBuilder.Append(",Name");
            queryBuilder.Append(",ParentId");
            queryBuilder.Append(",HasDocument");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedByUser");
            queryBuilder.Append(",LastUpdateAt");
            queryBuilder.Append(",LastUpdateByUser");
            queryBuilder.Append(" FROM Folders");
            queryBuilder.Append(" WHERE ParentId = @parentId");
            if (includeDeleted == false) queryBuilder.Append(" AND Deleted = 0");

            return await _context
                .CreateConnection()
                .QueryAsync<Folder>(queryBuilder.ToString(), parameters);
        }

        public async Task<Folder> InsertFolderAsync(Folder folder)
        {
            if (folder.CreatedByUser == null)
            {
                folder.CreatedByUser = "na";
                folder.LastUpdateByUser = "na";
            }
            if (folder.CreatedAt != null)
            {
                folder.CreatedAt = DateTime.Now;
                folder.LastUpdateAt = DateTime.Now;
            }

            var parameters = new
            {
                folder.ArchiveId,
                folder.Name,
                folder.ParentId,
                folder.CreatedAt,
                folder.LastUpdateAt,
                folder.CreatedByUser,
                folder.LastUpdateByUser,
                folder.Deleted,
                folder.HasDocument
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Folders (");
            queryBuilder.Append(" ArchiveId");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" ,ParentId");
            queryBuilder.Append(" ,CreatedAt");
            queryBuilder.Append(" ,LastUpdateAt");
            queryBuilder.Append(" ,CreatedByUser");
            queryBuilder.Append(" ,LastUpdateByUser");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,HasDocument");
            queryBuilder.Append(" )OUTPUT INSERTED.*");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append(" @ArchiveId");
            queryBuilder.Append(" ,@Name");
            queryBuilder.Append(" ,@ParentId");
            queryBuilder.Append(" ,@CreatedAt");
            queryBuilder.Append(" ,@LastUpdateAt");
            queryBuilder.Append(" ,@CreatedByUser");
            queryBuilder.Append(" ,@LastUpdateByUser");
            queryBuilder.Append(" ,@Deleted");
            queryBuilder.Append(" ,@HasDocument");
            queryBuilder.Append(" )");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<Folder>(queryBuilder.ToString(), parameters);
        }

        public async Task<Folder> SetDeleteFolderAsync(Guid id, bool deleted, string userName)
        {
            var parameters = new
            {
                id,
                deleted,
                userName
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Folders ");
            queryBuilder.Append(" SET Deleted = @deleted ");
            queryBuilder.Append(", LastUpdateByUser = @userName ");
            queryBuilder.Append(" OUTPUT INSERTED.*");
            queryBuilder.Append(" WHERE Id = @id ");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<Folder>(queryBuilder.ToString(), parameters);
        }

        public async Task<Folder> UpdateFolderAsync(Folder folder)
        {
            var parameters = new
            {
                folder.Name,
                folder.Deleted,
                folder.ParentId,
                folder.ArchiveId,
                folder.Id,
                folder.HasDocument,
                LastUpdateAt = DateTime.Now
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Folders ");
            queryBuilder.Append("SET Name = @Name ");
            queryBuilder.Append(" ,Deleted = @Deleted ");
            queryBuilder.Append(" ,ParentId = @ParentId ");
            queryBuilder.Append(" ,ArchiveId = @ArchiveId ");
            queryBuilder.Append(" ,HasDocument = @HasDocument ");
            queryBuilder.Append(" ,LastUpdateAt = @LastUpdateAt ");
            queryBuilder.Append(" OUTPUT INSERTED.* ");
            queryBuilder.Append(" WHERE Id = @Id ");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<Folder>(queryBuilder.ToString(), parameters);
        }

        public async Task<bool> UpdateFolderHasDocumentsAsync(Guid folderId, bool status = true)
        {
            var parameters = new
            {
                folderId,
                status
            };

            _logger.LogInformation($"DEBUG - Beginning updating HasDocument for folder {folderId}, setting to {status}");

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Folders ");
            queryBuilder.Append("SET HasDocument = @status ");
            queryBuilder.Append("OUTPUT INSERTED.*");
            queryBuilder.Append(" WHERE Id = @folderId ");

            var result = await _context
                .CreateConnection()
                .QuerySingleAsync<Folder>(queryBuilder.ToString(), parameters);

            _logger.LogInformation($"DEBUG - Folder status after update: \n{Newtonsoft.Json.JsonConvert.SerializeObject(result)}");

            return result.HasDocument;
        }

        public async Task<IEnumerable<Folder>> UpdateChildrenArchiveAsync(Guid parentId, Guid oldArchiveId, Guid newArchiveId)
        {
            var parameters = new
            {
                parentId,
                oldArchiveId,
                newArchiveId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Folders ");
            queryBuilder.Append("SET ArchiveId = @newArchiveId ");
            queryBuilder.Append(" OUTPUT INSERTED.* ");
            queryBuilder.Append(" WHERE ParentId = @parentId ");
            queryBuilder.Append(" AND ArchiveId = @oldArchiveId ");

            return await _context
                .CreateConnection()
                .QueryAsync<Folder>(queryBuilder.ToString(), parameters);
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

        public async Task DeleteFoldersByArchiveAsync(Guid archiveId, string username = "na")
        {
            var parameters = new
            {
                archiveId,
                username
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Folders ");
            queryBuilder.Append(" SET Deleted = 1 ");
            queryBuilder.Append(", LastUpdateByUser = @userName ");
            queryBuilder.Append(", LastUpdateAt = CURRENT_TIMESTAMP ");
            queryBuilder.Append(" OUTPUT INSERTED.*");
            queryBuilder.Append(" WHERE ArchiveId = @archiveId ");

            await _context
                .CreateConnection()
                .QueryAsync(queryBuilder.ToString(), parameters);
        }

        public async Task UndeleteFoldersByArchiveAsync(Guid archiveId, string username = "na")
        {
            var parameters = new
            {
                archiveId,
                username
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Folders ");
            queryBuilder.Append(" SET Deleted = 0 ");
            queryBuilder.Append(", LastUpdateByUser = @userName ");
            queryBuilder.Append(", LastUpdateAt = CURRENT_TIMESTAMP ");
            queryBuilder.Append(" OUTPUT INSERTED.*");
            queryBuilder.Append(" WHERE ArchiveId = @archiveId ");

            await _context
                .CreateConnection()
                .QueryAsync(queryBuilder.ToString(), parameters);
        }
    }
}
