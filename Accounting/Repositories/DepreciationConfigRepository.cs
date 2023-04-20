using Accounting.Context;
using Accounting.Models;
using Dapper;
using System.Text;

namespace Accounting.Repositories
{
    public class DepreciationConfigRepository : IDepreciationCongifRepository
    {
        private readonly DapperContext _context;
        public DepreciationConfigRepository(DapperContext context) 
        {
            _context = context;
        }

        public async Task<DepreciationConfig> GetDepreciationConfigByIdAsync(Guid DepreciationConfigId)
        {
            var parameters = new
            {
                DepreciationConfigId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM DepreciationConfigs");
            queryBuilder.Append(" WHERE Id = @DepreciationConfigId");

            using var connection = _context.CreateConnection();

            DepreciationConfig DepreciationConfig = await connection.QuerySingleAsync<DepreciationConfig>(queryBuilder.ToString(), parameters);
            return DepreciationConfig;
        }

        public async Task<IEnumerable<DepreciationConfig>> GetDepreciationConfigsAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM DepreciationConfigs");

            using var connection = _context.CreateConnection();

            IEnumerable<DepreciationConfig> DepreciationConfigs = await connection.QueryAsync<DepreciationConfig>(queryBuilder.ToString());
            return DepreciationConfigs;
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

        public async Task<int> UpdateDepreciationConfigAsync(DepreciationConfig DepreciationConfig)
        {
            var parameters = new
            {
                DepreciationConfig.Id,
                DepreciationConfig.Type,
                DepreciationConfig.DepreciationPercent,
                DepreciationConfig.Deleted,
                DepreciationConfig.LastModificationByUser,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE DepreciationConfigs ");
            queryBuilder.Append("SET Type = @Type ");
            queryBuilder.Append(" ,DepreciationPercent = @DepreciationPercent ");
            queryBuilder.Append(" ,Deleted = @Deleted ");
            queryBuilder.Append(" ,LastModificationByUser = @LastModificationByUser ");
            queryBuilder.Append(" ,LastModificationDate = @LastModificationDate ");
            queryBuilder.Append(" WHERE Id = @Id ");

            using var connection = _context.CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
            return rowsAffected;
        }
    }
}
