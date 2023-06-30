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

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<FixedAsset>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<FixedAsset>> GetFixedAssetsAsync(Guid tenantId, bool includeDeleted = false)
        {
            var parameters = new
            {
                tenantId,
                deleted = includeDeleted ? 1 : 0
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT FixedAssets.Id");
            queryBuilder.Append(",FixedAssets.InvoiceLineId");
            queryBuilder.Append(",FixedAssets.Description");
            queryBuilder.Append(",FixedAssets.CapitalizationDate");
            queryBuilder.Append(",FixedAssets.AcquisitionAndProductionCosts");
            queryBuilder.Append(",FixedAssets.DepreciationPercentagePerYear");
            queryBuilder.Append(",FixedAssets.Deleted");
            queryBuilder.Append(",FixedAssets.CreatedAt");
            queryBuilder.Append(",FixedAssets.CreatedBy");
            queryBuilder.Append(",FixedAssets.LastModificationAt");
            queryBuilder.Append(",FixedAssets.LastModificationBy");
            queryBuilder.Append(" FROM FixedAssets");
            queryBuilder.Append(" INNER JOIN APInvoiceLines ON APInvoiceLines.Id = FixedAssets.InvoiceLineId");
            queryBuilder.Append(" INNER JOIN APInvoices ON APInvoices.Id = APInvoiceLines.InvoiceId");
            queryBuilder.Append(" INNER JOIN BusinessPartners ON BusinessPartners.Id = APInvoices.BusinessPartnerId");
            queryBuilder.Append(" WHERE BusinessPartners.TenantId = @tenantId");
            if (!includeDeleted) queryBuilder.Append(" AND FixedAssets.Deleted = @deleted");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QueryAsync<FixedAsset>(queryBuilder.ToString(), parameters);
        }

        public async Task<FixedAsset?> GetFixedAssetByIdAsync(Guid tenantId, Guid fixedAssetId)
        {
            var parameters = new
            {
                tenantId,
                fixedAssetId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT FixedAssets.Id");
            queryBuilder.Append(",FixedAssets.InvoiceLineId");
            queryBuilder.Append(",FixedAssets.Description");
            queryBuilder.Append(",FixedAssets.CapitalizationDate");
            queryBuilder.Append(",FixedAssets.AcquisitionAndProductionCosts");
            queryBuilder.Append(",FixedAssets.DepreciationPercentagePerYear");
            queryBuilder.Append(",FixedAssets.Deleted");
            queryBuilder.Append(",FixedAssets.CreatedAt");
            queryBuilder.Append(",FixedAssets.CreatedBy");
            queryBuilder.Append(",FixedAssets.LastModificationAt");
            queryBuilder.Append(",FixedAssets.LastModificationBy");
            queryBuilder.Append(" FROM FixedAssets");
            queryBuilder.Append(" INNER JOIN APInvoiceLines ON APInvoiceLines.Id = FixedAssets.InvoiceLineId");
            queryBuilder.Append(" INNER JOIN APInvoices ON APInvoices.Id = APInvoiceLines.InvoiceId");
            queryBuilder.Append(" INNER JOIN BusinessPartners ON BusinessPartners.Id = APInvoices.BusinessPartnerId");
            queryBuilder.Append(" WHERE FixedAssets.tenantId = @tenantId");
            queryBuilder.Append(" AND FixedAssets.Id = @fixedAssetId");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleOrDefaultAsync<FixedAsset?>(queryBuilder.ToString(), parameters);
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

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<FixedAsset>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeletedFixedAssetAsync(Guid tenantId, Guid id, bool deleted, string userName)
        {
            var parameters = new
            {
                id,
                deleted,
                lastModificationAt = DateTime.Now,
                lastModificationBy = userName,
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE FixedAssets");
            queryBuilder.Append(" SET Deleted = @deleted");
            queryBuilder.Append(" ,LastModificationAt = @lastModificationAt");
            queryBuilder.Append(" ,LastModificationBy = @lastModificationBy");
            queryBuilder.Append(" WHERE Id = @id");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
        }


    }
}
