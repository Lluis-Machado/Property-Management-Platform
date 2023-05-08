using Accounting.Context;
using Accounting.Models;
using Dapper;
using System.Text;

namespace Accounting.Repositories
{
    public class DepreciationConfigRepository : IDepreciationConfigRepository
    {
        private readonly DapperContext _context;
        public DepreciationConfigRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<DepreciationConfig> GetDepreciationConfigByIdAsync(Guid depreciationConfigId)
        {
            var parameters = new
            {
                depreciationConfigId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,Type");
            queryBuilder.Append(" ,DepreciationPercent");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" FROM DepreciationConfigs");
            queryBuilder.Append(" WHERE Id = @depreciationConfigId");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<DepreciationConfig>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<DepreciationConfig>> GetDepreciationConfigsAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,Type");
            queryBuilder.Append(" ,DepreciationPercent");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" FROM DepreciationConfigs");

            using var connection = _context.CreateConnection();

            return await _context
                 .CreateConnection()
                 .QueryAsync<DepreciationConfig>(queryBuilder.ToString());
        }

        public async Task<DepreciationConfig> InsertDepreciationConfigAsync(DepreciationConfig DepreciationConfig)
        {
            var parameters = new
            {
                DepreciationConfig.Type,
                DepreciationConfig.DepreciationPercent,
                DepreciationConfig.LastModificationByUser,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO DepreciationConfigs (");
            queryBuilder.Append(" Type");
            queryBuilder.Append(" ,DepreciationPercent");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" )OUTPUT INSERTED.Id");
            queryBuilder.Append(" ,INSERTED.Type");
            queryBuilder.Append(" ,INSERTED.DepreciationPercent");
            queryBuilder.Append(" ,INSERTED.Deleted");
            queryBuilder.Append(" ,INSERTED.CreationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationByUser");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append(" @Type");
            queryBuilder.Append(" ,@DepreciationPercent");
            queryBuilder.Append(" ,@LastModificationByUser");
            queryBuilder.Append(" )");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<DepreciationConfig>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeleteDepreciationConfigAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE DepreciationConfigs");
            queryBuilder.Append(" SET Deleted = @deleted ");
            queryBuilder.Append(" WHERE Id = @id");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<int> UpdateDepreciationConfigAsync(DepreciationConfig depreciationConfig)
        {
            var parameters = new
            {
                depreciationConfig.Id,
                depreciationConfig.Type,
                depreciationConfig.DepreciationPercent,
                depreciationConfig.Deleted,
                depreciationConfig.LastModificationByUser,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE DepreciationConfigs");
            queryBuilder.Append(" SET Type = @Type");
            queryBuilder.Append(" ,DepreciationPercent = @DepreciationPercent");
            queryBuilder.Append(" ,Deleted = @Deleted");
            queryBuilder.Append(" ,LastModificationByUser = @LastModificationByUser");
            queryBuilder.Append(" ,LastModificationDate = @LastModificationDate");
            queryBuilder.Append(" WHERE Id = @Id");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }

    }
}
