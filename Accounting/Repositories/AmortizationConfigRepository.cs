using Accounting.Context;
using Accounting.Models;
using Dapper;
using System.Text;

namespace Accounting.Repositories
{
    public class AmortizationConfigRepository : IAmortizationCongifRepository
    {
        private readonly DapperContext _context;
        public AmortizationConfigRepository(DapperContext context) 
        {
            _context = context;
        }

        public async Task<AmortizationConfig> GetAmortizationConfigByIdAsync(Guid amortizationConfigId)
        {
            var parameters = new
            {
                amortizationConfigId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM AmortizationConfigs");
            queryBuilder.Append(" WHERE Id = @amortizationConfigId");

            using var connection = _context.CreateConnection();

            AmortizationConfig amortizationConfig = await connection.QuerySingleAsync<AmortizationConfig>(queryBuilder.ToString(), parameters);
            return amortizationConfig;
        }

        public async Task<IEnumerable<AmortizationConfig>> GetAmortizationConfigsAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM AmortizationConfigs");

            using var connection = _context.CreateConnection();

            IEnumerable<AmortizationConfig> amortizationConfigs = await connection.QueryAsync<AmortizationConfig>(queryBuilder.ToString());
            return amortizationConfigs;
        }

        public async Task<Guid> InsertAmortizationConfigAsync(AmortizationConfig amortizationConfig)
        {
            var parameters = new
            {
                amortizationConfig.Type,
                amortizationConfig.AmortizationPercent,
                amortizationConfig.LastModificationByUser,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO AmortizationConfigs (");
            queryBuilder.Append(" Type");
            queryBuilder.Append(" ,AmortizationPercent");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" )OUTPUT INSERTED.Id VALUES(");
            queryBuilder.Append(" @Type");
            queryBuilder.Append(" ,@AmortizationPercent");
            queryBuilder.Append(" ,@LastModificationByUser");
            queryBuilder.Append(" )");

            using var connection = _context.CreateConnection();

            Guid amortizationConfigId = await connection.QuerySingleAsync<Guid>(queryBuilder.ToString(), parameters);
            return amortizationConfigId;
        }

        public async Task<int> UpdateAmortizationConfigAsync(AmortizationConfig amortizationConfig)
        {
            var parameters = new
            {
                amortizationConfig.Id,
                amortizationConfig.Type,
                amortizationConfig.AmortizationPercent,
                amortizationConfig.Deleted,
                amortizationConfig.LastModificationByUser,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE AmortizationConfigs ");
            queryBuilder.Append("SET Type = @Type ");
            queryBuilder.Append(" ,AmortizationPercent = @AmortizationPercent ");
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
