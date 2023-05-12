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

        public async Task<FixedAsset?> GetFixedAssetByIdAsync(Guid fixedAssetId)
        {
            var parameters = new
            {
                fixedAssetId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,InvoiceId");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" ,ActivationDate");
            queryBuilder.Append(" ,ActivationAmount");
            queryBuilder.Append(" ,DepreciationConfigId");
            queryBuilder.Append(" ,DepreciationAmountPercent");
            queryBuilder.Append(" ,DepreciationMaxYears");
            queryBuilder.Append(" ,EstimatedUsefulLife");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" FROM FixedAssets");
            queryBuilder.Append(" WHERE Id = @fixedAssetId");

            return await _context
                .CreateConnection()
                .QuerySingleOrDefaultAsync<FixedAsset?>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<FixedAsset>> GetFixedAssetsAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,InvoiceId");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" ,ActivationDate");
            queryBuilder.Append(" ,ActivationAmount");
            queryBuilder.Append(" ,DepreciationConfigId");
            queryBuilder.Append(" ,DepreciationAmountPercent");
            queryBuilder.Append(" ,DepreciationMaxYears");
            queryBuilder.Append(" ,EstimatedUsefulLife");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" FROM FixedAssets");

            return await _context
                .CreateConnection()
                .QueryAsync<FixedAsset>(queryBuilder.ToString());
        }

        public async Task<FixedAsset> InsertFixedAssetAsync(FixedAsset fixedAsset)
        {
            var parameters = new
            {
                fixedAsset.InvoiceId,
                fixedAsset.Name,
                fixedAsset.ActivationDate,
                fixedAsset.ActivationAmount,
                fixedAsset.DepreciationConfigId,
                fixedAsset.DepreciationAmountPercent,
                fixedAsset.DepreciationMaxYears,
                fixedAsset.EstimatedUsefulLife,
                fixedAsset.LastModificationByUser,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO FixedAssets (");
            queryBuilder.Append(" InvoiceId");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" ,ActivationDate");
            queryBuilder.Append(" ,ActivationAmount");
            queryBuilder.Append(" ,DepreciationConfigId");
            queryBuilder.Append(" ,DepreciationAmountPercent");
            queryBuilder.Append(" ,DepreciationMaxYears");
            queryBuilder.Append(" ,EstimatedUsefulLife");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" )OUTPUT INSERTED.Id");
            queryBuilder.Append(" ,INSERTED.InvoiceId");
            queryBuilder.Append(" ,INSERTED.Name");
            queryBuilder.Append(" ,INSERTED.ActivationDate");
            queryBuilder.Append(" ,INSERTED.ActivationAmount");
            queryBuilder.Append(" ,INSERTED.DepreciationConfigId");
            queryBuilder.Append(" ,INSERTED.DepreciationAmountPercent");
            queryBuilder.Append(" ,INSERTED.DepreciationMaxYears");
            queryBuilder.Append(" ,INSERTED.EstimatedUsefulLife");
            queryBuilder.Append(" ,INSERTED.Deleted");
            queryBuilder.Append(" ,INSERTED.CreationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationByUser");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append(" @InvoiceId");
            queryBuilder.Append(" ,@Name");
            queryBuilder.Append(" ,@ActivationDate");
            queryBuilder.Append(" ,@ActivationAmount");
            queryBuilder.Append(" ,@DepreciationConfigId");
            queryBuilder.Append(" ,@DepreciationAmountPercent");
            queryBuilder.Append(" ,@DepreciationMaxYears");
            queryBuilder.Append(" ,@EstimatedUsefulLife");
            queryBuilder.Append(" ,@LastModificationByUser");
            queryBuilder.Append(" )");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<FixedAsset>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeleteFixedAssetAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE FixedAssets");
            queryBuilder.Append(" SET Deleted = @deleted");
            queryBuilder.Append(" WHERE Id = @id");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
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
                fixedAsset.DepreciationConfigId,
                fixedAsset.DepreciationMaxYears,
                fixedAsset.EstimatedUsefulLife,
                fixedAsset.Deleted,
                fixedAsset.LastModificationByUser,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE FixedAssets");
            queryBuilder.Append(" SET InvoiceId = @InvoiceId");
            queryBuilder.Append(" ,Name = @Name");
            queryBuilder.Append(" ,ActivationDate = @ActivationDate");
            queryBuilder.Append(" ,ActivationAmount = @ActivationAmount");
            queryBuilder.Append(" ,DepreciationConfigId = @DepreciationConfigId");
            queryBuilder.Append(" ,DepreciationMaxYears = @DepreciationMaxYears");
            queryBuilder.Append(" ,EstimatedUsefulLife = @EstimatedUsefulLife");
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
