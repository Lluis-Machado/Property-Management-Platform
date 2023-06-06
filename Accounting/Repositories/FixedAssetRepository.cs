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
            queryBuilder.Append(" ,CapitalizationDate");
            queryBuilder.Append(" ,AcquisitionAndProductionCosts");
            queryBuilder.Append(" ,DepreciatedAmount");
            queryBuilder.Append(" ,DepreciationConfigId");
            queryBuilder.Append(" ,DepreciationAmountPercent");
            queryBuilder.Append(" ,DepreciationMaxYears");
            queryBuilder.Append(" ,EstimatedUsefulLife");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationBy");
            queryBuilder.Append(" FROM FixedAssets");
            queryBuilder.Append(" WHERE Id = @fixedAssetId");

            return await _context
                .CreateConnection()
                .QuerySingleOrDefaultAsync<FixedAsset?>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<FixedAsset>> GetFixedAssetsAsync(bool includeDeleted)
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,InvoiceId");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" ,CapitalizationDate");
            queryBuilder.Append(" ,AcquisitionAndProductionCosts");
            queryBuilder.Append(" ,DepreciatedAmount");
            queryBuilder.Append(" ,DepreciationConfigId");
            queryBuilder.Append(" ,DepreciationAmountPercent");
            queryBuilder.Append(" ,DepreciationMaxYears");
            queryBuilder.Append(" ,EstimatedUsefulLife");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationBy");
            queryBuilder.Append(" FROM FixedAssets");
            if (includeDeleted == false) queryBuilder.Append(" WHERE Deleted = 0");


            return await _context
                .CreateConnection()
                .QueryAsync<FixedAsset>(queryBuilder.ToString());
        }

        public async Task<FixedAsset> InsertFixedAssetAsync(FixedAsset fixedAsset)
        {
            var parameters = new
            {
                fixedAsset.InvoiceId,
                fixedAsset.Description,
                fixedAsset.CapitalizationDate,
                fixedAsset.AcquisitionAndProductionCosts,
                fixedAsset.DepreciationAmountPercent,
                fixedAsset.LastModificationBy,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO FixedAssets (");
            queryBuilder.Append(" InvoiceId");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" ,CapitalizationDate");
            queryBuilder.Append(" ,AcquisitionAndProductionCosts");
            queryBuilder.Append(" ,DepreciationAmountPercent");
            queryBuilder.Append(" ,LastModificationBy");
            queryBuilder.Append(" )OUTPUT INSERTED.Id");
            queryBuilder.Append(" ,INSERTED.InvoiceId");
            queryBuilder.Append(" ,INSERTED.Name");
            queryBuilder.Append(" ,INSERTED.CapitalizationDate");
            queryBuilder.Append(" ,INSERTED.AcquisitionAndProductionCosts");
            queryBuilder.Append(" ,INSERTED.DepreciatedAmount");
            queryBuilder.Append(" ,INSERTED.DepreciationAmountPercent");
            queryBuilder.Append(" ,INSERTED.Deleted");
            queryBuilder.Append(" ,INSERTED.CreationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationBy");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append(" @InvoiceId");
            queryBuilder.Append(" ,@Name");
            queryBuilder.Append(" ,@CapitalizationDate");
            queryBuilder.Append(" ,@AcquisitionAndProductionCosts");
            queryBuilder.Append(" ,@DepreciationAmountPercent");
            queryBuilder.Append(" ,@LastModificationBy");
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
                fixedAsset.Description,
                fixedAsset.CapitalizationDate,
                fixedAsset.AcquisitionAndProductionCosts,
                fixedAsset.Deleted,
                fixedAsset.LastModificationBy,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE FixedAssets");
            queryBuilder.Append(" SET InvoiceId = @InvoiceId");
            queryBuilder.Append(" ,Description = @Description");
            queryBuilder.Append(" ,CapitalizationDate = @CapitalizationDate");
            queryBuilder.Append(" ,AcquisitionAndProductionCosts = @AcquisitionAndProductionCosts");
            queryBuilder.Append(" ,Deleted = @Deleted");
            queryBuilder.Append(" ,LastModificationBy = @LastModificationBy");
            queryBuilder.Append(" ,LastModificationDate = @LastModificationDate");
            queryBuilder.Append(" WHERE Id = @Id");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }
    }
}
