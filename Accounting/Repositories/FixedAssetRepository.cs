using AccountingAPI.Context;
using AccountingAPI.Models;
using Dapper;
using System.Text;

namespace AccountingAPI.Repositories
{
    public class FixedAssetRepository : IFixedAssetRepository
    {
        private readonly IDapperContext _context;
        public FixedAssetRepository(IDapperContext context)
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
            queryBuilder.Append(",InvoiceLineId");
            queryBuilder.Append(",Description");
            queryBuilder.Append(",CapitalizationDate");
            queryBuilder.Append(",AcquisitionAndProductionCosts");
            queryBuilder.Append(",DepreciationPercentagePerYear");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM FixedAssets");
            queryBuilder.Append(" WHERE Id = @fixedAssetId");

            return await _context.Connection.QuerySingleOrDefaultAsync<FixedAsset?>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<FixedAsset>> GetFixedAssetsAsync(bool includeDeleted)
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",InvoiceLineId");
            queryBuilder.Append(",Description");
            queryBuilder.Append(",CapitalizationDate");
            queryBuilder.Append(",AcquisitionAndProductionCosts");
            queryBuilder.Append(",DepreciationPercentagePerYear");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM FixedAssets");
            if (includeDeleted == false) queryBuilder.Append(" WHERE Deleted = 0");


            return await _context.Connection.QueryAsync<FixedAsset>(queryBuilder.ToString());
        }

        public async Task<FixedAsset> InsertFixedAssetAsync(FixedAsset fixedAsset)
        {
            var parameters = new
            {
                fixedAsset.InvoiceLineId,
                fixedAsset.Description,
                fixedAsset.CapitalizationDate,
                fixedAsset.AcquisitionAndProductionCosts,
                fixedAsset.DepreciationPercentagePerYear,
                fixedAsset.LastModificationBy,
                fixedAsset.CreatedBy,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO FixedAssets (");
            queryBuilder.Append("InvoiceLineId");
            queryBuilder.Append(",Description");
            queryBuilder.Append(",CapitalizationDate");
            queryBuilder.Append(",AcquisitionAndProductionCosts");
            queryBuilder.Append(",DepreciationPercentagePerYear");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(")OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.InvoiceLineId");
            queryBuilder.Append(",INSERTED.Description");
            queryBuilder.Append(",INSERTED.CapitalizationDate");
            queryBuilder.Append(",INSERTED.AcquisitionAndProductionCosts");
            queryBuilder.Append(",INSERTED.DepreciationPercentagePerYear");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append("@InvoiceLineId");
            queryBuilder.Append(",@Description");
            queryBuilder.Append(",@CapitalizationDate");
            queryBuilder.Append(",@AcquisitionAndProductionCosts");
            queryBuilder.Append(",@DepreciationPercentagePerYear");
            queryBuilder.Append(",@CreatedBy");
            queryBuilder.Append(",@LastModificationBy");
            queryBuilder.Append(")");

            return await _context.Connection.QuerySingleAsync<FixedAsset>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeletedFixedAssetAsync(Guid id, bool deleted)
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

            return await _context.Connection.ExecuteAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<FixedAsset> UpdateFixedAssetAsync(FixedAsset fixedAsset)
        {
            var parameters = new
            {
                fixedAsset.Id,
                fixedAsset.Description,
                fixedAsset.CapitalizationDate,
                fixedAsset.AcquisitionAndProductionCosts,
                fixedAsset.DepreciationPercentagePerYear,
                fixedAsset.Deleted,
                fixedAsset.LastModificationBy,
                fixedAsset.LastModificationAt,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE FixedAssets");
            queryBuilder.Append(" SET Description = @Description");
            queryBuilder.Append(",CapitalizationDate = @CapitalizationDate");
            queryBuilder.Append(",AcquisitionAndProductionCosts = @AcquisitionAndProductionCosts");
            queryBuilder.Append(",DepreciationPercentagePerYear = @DepreciationPercentagePerYear");
            queryBuilder.Append(",Deleted = @Deleted");
            queryBuilder.Append(",LastModificationBy = @LastModificationBy");
            queryBuilder.Append(",LastModificationAt = @LastModificationAt");
            queryBuilder.Append(" OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.InvoiceLineId");
            queryBuilder.Append(",INSERTED.Description");
            queryBuilder.Append(",INSERTED.CapitalizationDate");
            queryBuilder.Append(",INSERTED.AcquisitionAndProductionCosts");
            queryBuilder.Append(",INSERTED.DepreciationPercentagePerYear");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(" WHERE Id = @Id");

            return await _context.Connection.QuerySingleAsync<FixedAsset>(queryBuilder.ToString(), parameters);
        }
    }
}
