﻿using Dapper;
using DocumentsAPI.Contexts;
using DocumentsAPI.Models;
using DocumentsAPI.Repositories;
using System.Text;

namespace Documents.Repositories
{
    public class FolderRepository : IFolderRepository

    {
        private readonly DapperContext _context;

        public FolderRepository(DapperContext context)
        {
            _context = context;
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

        public async Task<Folder> GetFolderByIdAsync(Guid archiveId, Guid folderId)
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
            queryBuilder.Append(" WHERE ArchiveId = @archiveId");
            queryBuilder.Append(" AND Id = @folderId");

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
            queryBuilder.Append(" WHERE ArchiveId = @archiveId");
            queryBuilder.Append(" AND Id = @folderId");

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
                folder.ParentId,
                folder.CreatedByUser,
                folder.LastUpdateByUser
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Folders (");
            queryBuilder.Append(" ArchiveId");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" ,ParentId");
            queryBuilder.Append(" ,CreatedByUser");
            queryBuilder.Append(" ,LastUpdateByUser");
            queryBuilder.Append(" )OUTPUT INSERTED.*");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append(" @ArchiveId");
            queryBuilder.Append(" ,@Name");
            queryBuilder.Append(" ,@ParentId");
            queryBuilder.Append(" ,@CreatedByUser");
            queryBuilder.Append(" ,@LastUpdateByUser");
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
            queryBuilder.Append(", LastUpdateByUser = @userName ");
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
                folder.Id
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Folders ");
            queryBuilder.Append("SET Name = @Name ");
            queryBuilder.Append(" ,Deleted = @Deleted ");
            queryBuilder.Append(" ,ParentId = @ParentId ");
            queryBuilder.Append(" WHERE Id = @Id ");

            return await _context
                .CreateConnection()
                .QuerySingleAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<bool> UpdateFolderHasDocumentsAsync(Guid folderId, bool status = true)
        {
            var parameters = new
            {
                folderId,
                status
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Folders ");
            queryBuilder.Append("SET HasDocument = @status ");
            queryBuilder.Append(" WHERE Id = @folderId ");
            await _context
                .CreateConnection()
                .QuerySingleAsync(queryBuilder.ToString(), parameters);
            return status;
        }

        public List<TreeFolderItem> ToFolderTreeView(List<Folder> folders)
        {
            throw new NotImplementedException();
        }
    }
}
