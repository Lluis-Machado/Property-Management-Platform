using Accounting.Context;
using Accounting.Models;
using Dapper;
using System.Text;

namespace Accounting.Repositories
{
    public class FixedAssetRepository : IFixedAssetRepository
    {
        private readonly DapperContext _context;
        public FixedAssetRepository(DapperContext context) 
        {
            _context = context;
        }

        public async Task<FixedAsset> GetFixedAssetByIdAsync(Guid fixedAssetId)
        {
            var parameters = new
            {
                fixedAssetId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM FixedAssets");
            queryBuilder.Append(" WHERE Id = @fixedAssetId");

            using var connection = _context.CreateConnection();

            FixedAsset fixedAsset = await connection.QuerySingleAsync<FixedAsset>(queryBuilder.ToString(), parameters);
            return fixedAsset;
        }

        public async Task<IEnumerable<FixedAsset>> GetFixedAssetsAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM FixedAssets");

            using var connection = _context.CreateConnection();

            IEnumerable<FixedAsset> fixedAssets = await connection.QueryAsync<FixedAsset>(queryBuilder.ToString());
            return fixedAssets;
        }

        public async Task<Guid> InsertFixedAssetAsync(FixedAsset fixedAsset)
        {
            var parameters = new
            {
                fixedAsset.InvoiceId,
                fixedAsset.Name,
                fixedAsset.ActivationDate,
                fixedAsset.ActivationAmount,
                fixedAsset.AmortizationConfigId,
                fixedAsset.LastModificationByUser,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO FixedAssets (");
            queryBuilder.Append(" InvoiceId");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" ,ActivationDate");
            queryBuilder.Append(" ,ActivationAmount");
            queryBuilder.Append(" ,AmortizationConfigId");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" )OUTPUT INSERTED.Id VALUES(");
            queryBuilder.Append(" @InvoiceId");
            queryBuilder.Append(" ,@Name");
            queryBuilder.Append(" ,@ActivationDate");
            queryBuilder.Append(" ,@ActivationAmount");
            queryBuilder.Append(" ,@AmortizationConfigId");
            queryBuilder.Append(" ,@LastModificationByUser");
            queryBuilder.Append(" )");

            using var connection = _context.CreateConnection();

            Guid fixedAssetId = await connection.QuerySingleAsync<Guid>(queryBuilder.ToString(), parameters);
            return fixedAssetId;
        }

        public async Task<int> UpdateFixedAssetAsync(FixedAsset fixedAsset)
        {
            var parameters = new
            {
                fixedAsset.Id,
                fixedAsset.InvoiceId,
                fixedAsset.Name,
                fixedAsset.ActivationDate,
                fixedAsset.ActivationAmount,
                fixedAsset.AmortizationConfigId,
                fixedAsset.Deleted,
                fixedAsset.LastModificationByUser,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE FixedAssets ");
            queryBuilder.Append("SET InvoiceId = @InvoiceId ");
            queryBuilder.Append(" ,Name = @Name ");
            queryBuilder.Append(" ,ActivationDate = @ActivationDate ");
            queryBuilder.Append(" ,ActivationAmount = @ActivationAmount ");
            queryBuilder.Append(" ,AmortizationConfigId = @AmortizationConfigId ");
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
