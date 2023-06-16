using DocumentsAPI.Contexts;
using DocumentsAPI.Models;
using System.Text;
using DocumentsAPI.Repositories;
using Dapper;

namespace Documents.Repositories
{
    public class FolderRepository : IFolderRepository

    {
        private readonly DapperContext _context;

        public FolderRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Folder> GetFolderByIdAsync(Guid archiveId, int folderId)
        {
            var parameters = new
            {
                archiveId,
                folderId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM Folders");
            queryBuilder.Append(" WHERE ArchiveId = @archiveId");
            queryBuilder.Append(" AND FolderId = @folderId");

            return await _context
                .CreateConnection()
                .QuerySingleOrDefaultAsync<Folder>(queryBuilder.ToString(), parameters);
        }

        public async Task<Folder> GetFolderById(Guid folderId)
        {
            var parameters = new
            {
                folderId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM Folders");
            queryBuilder.Append(" WHERE ArchiveId = @archiveId");
            queryBuilder.Append(" AND FolderId = @folderId");

            return await _context
                .CreateConnection()
                .QuerySingleOrDefaultAsync<Folder>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<Folder>> GetFoldersAsync(Guid archiveId, bool includeDeleted = false)
        {
            var parameters = new
            {
                archiveId,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM Folders");
            queryBuilder.Append(" WHERE ArchiveId = @archiveId");
            if (includeDeleted == false) queryBuilder.Append(" AND Deleted = 0");

            return await _context
                .CreateConnection()
                .QueryAsync<Folder>(queryBuilder.ToString(), parameters);
        }

        public async Task<Folder> InsertFolderAsync(Folder folder)
        {
            var parameters = new
            {
                folder.ArchiveId,
                folder.Name,
                folder.ParentId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Folders (");
            queryBuilder.Append(" ArchiveId");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" ,ParentId");
            queryBuilder.Append(" )OUTPUT INSERTED.*");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append(" @ArchiveId");
            queryBuilder.Append(" ,@Name");
            queryBuilder.Append(" ,@ParentId");
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
            queryBuilder.Append("SET Deleted = @deleted ");
            queryBuilder.Append("SET LastUpdateByUser = @userName ");
            queryBuilder.Append(" OUTPUT INSERTED.* ");
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
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE BusinessPartners ");
            queryBuilder.Append("SET Name = @Name ");
            queryBuilder.Append(" ,Deleted = @Deleted ");
            queryBuilder.Append(" ,ParentId = @ParentId ");
            queryBuilder.Append(" OUTPUT INSERTED.* ");
            queryBuilder.Append(" WHERE Id = @Id ");

            return await _context
                .CreateConnection()
                .QuerySingleAsync(queryBuilder.ToString(), parameters);
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
