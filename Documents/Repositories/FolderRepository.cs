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

        public async Task<Folder?> GetFolderByIdAsync(Guid archiveId, int folderId)
        {
            var parameters = new
            {
                archiveId,
                folderId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM Folders");
            queryBuilder.Append(" WHERE archiveId = @archiveId");
            queryBuilder.Append(" AND folderId = @folderId");

            return await _context
                .CreateConnection()
                .QuerySingleOrDefaultAsync<Folder?>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<Folder>> GetFoldersAsync(Guid archiveId, bool includeDeleted)
        {
            var parameters = new
            {
                archiveId,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM Folders");
            queryBuilder.Append(" WHERE archiveId = @archiveId");
            if (includeDeleted == false) queryBuilder.Append(" WHERE Deleted = 0");

            return await _context
                .CreateConnection()
                .QueryAsync<Folder>(queryBuilder.ToString(), parameters);
        }

        public async Task<Folder> InsertFolderAsync(Folder folder)
        {
            var parameters = new
            {
                folder.TenantId,
                folder.Name,
                folder.ParentId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Folders (");
            queryBuilder.Append(" TenantId");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" ,ParentId");
            queryBuilder.Append(" )OUTPUT INSERTED.Id");
            queryBuilder.Append(" ,INSERTED.Name");
            queryBuilder.Append(" ,INSERTED.TenantId");
            queryBuilder.Append(" ,INSERTED.Name");
            queryBuilder.Append(" ,INSERTED.ParentId");
            queryBuilder.Append(" ,INSERTED.Deleted");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append(" @TenantId");
            queryBuilder.Append(" ,@Name");
            queryBuilder.Append(" ,@ParentId");
            queryBuilder.Append(" )");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<Folder>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeleteFolderAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Folders ");
            queryBuilder.Append("SET Deleted = @deleted ");
            queryBuilder.Append(" WHERE Id = @id ");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<int> UpdateFolderAsync(Folder folder)
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
            queryBuilder.Append(" WHERE Id = @Id ");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }
    }
}
