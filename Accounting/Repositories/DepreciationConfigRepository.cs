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

            using var connection = _context.CreateConnection();

            DepreciationConfig depreciationConfig = await connection.QuerySingleAsync<DepreciationConfig>(queryBuilder.ToString(), parameters);
            return depreciationConfig;
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

            IEnumerable<DepreciationConfig> depreciationConfigs = await connection.QueryAsync<DepreciationConfig>(queryBuilder.ToString());
            return depreciationConfigs;
        }

        public async Task<Guid> InsertDepreciationConfigAsync(DepreciationConfig DepreciationConfig)
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
            queryBuilder.Append(" )OUTPUT INSERTED.Id VALUES(");
            queryBuilder.Append(" @Type");
            queryBuilder.Append(" ,@DepreciationPercent");
            queryBuilder.Append(" ,@LastModificationByUser");
            queryBuilder.Append(" )");

            using var connection = _context.CreateConnection();

            Guid DepreciationConfigId = await connection.QuerySingleAsync<Guid>(queryBuilder.ToString(), parameters);
            return DepreciationConfigId;
        }

        public async Task<int> SetDeleteDepereciationConfigAsync(Guid id, bool deleted)
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

            using var connection = _context.CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
            return rowsAffected;
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

            using var connection = _context.CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
            return rowsAffected;
        }

    }
}
