using AccountingAPI.Context;
using AccountingAPI.Models;
using Dapper;
using System.Text;

namespace AccountingAPI.Repositories
{
    public class DepreciationRepository : IDepreciationRepository
    {
        private readonly IDapperContext _context;
        public DepreciationRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<Depreciation> InsertDepreciationAsync(Depreciation depreciation)
        {
            var parameters = new
            {
                depreciation.FixedAssetId,
                depreciation.PeriodId,
                depreciation.DepreciationAmount,
                depreciation.CreatedBy,
                depreciation.LastModificationBy,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Depreciations (");
            queryBuilder.Append(" FixedAssetId");
            queryBuilder.Append(",PeriodId");
            queryBuilder.Append(",DepreciationAmount");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(")OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.FixedAssetId");
            queryBuilder.Append(",INSERTED.PeriodId");
            queryBuilder.Append(",INSERTED.DepreciationAmount");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append("@FixedAssetId");
            queryBuilder.Append(",@PeriodId");
            queryBuilder.Append(",@DepreciationAmount");
            queryBuilder.Append(",@CreatedBy");
            queryBuilder.Append(",@LastModificationBy");
            queryBuilder.Append(")");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<Depreciation>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<Depreciation>> GetDepreciationsAsync(Guid tenantId, bool includeDeleted = false)
        {
            var parameters = new
            {
                tenantId,
                deleted = includeDeleted? 1:0
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Depreciations.Id");
            queryBuilder.Append(",Depreciations.FixedAssetId");
            queryBuilder.Append(",Depreciations.PeriodId");
            queryBuilder.Append(",Depreciations.DepreciationAmount");
            queryBuilder.Append(",Depreciations.Deleted");
            queryBuilder.Append(",Depreciations.CreatedAt");
            queryBuilder.Append(",Depreciations.CreatedBy");
            queryBuilder.Append(",Depreciations.LastModificationAt");
            queryBuilder.Append(",Depreciations.LastModificationBy");
            queryBuilder.Append(" FROM Depreciations");
            queryBuilder.Append(" INNER JOIN FixedAssets ON FixedAssets.Id = Depreciations.FixedAssetId");
            queryBuilder.Append(" INNER JOIN APInvoiceLines ON APInvoiceLines.Id = FixedAssets.InvoiceLineId");
            queryBuilder.Append(" INNER JOIN APInvoices ON APInvoices.Id = APInvoiceLines.InvoiceId");
            queryBuilder.Append(" INNER JOIN BusinessPartners ON BusinessPartners.Id = APInvoices.BusinessPartnerId");
            queryBuilder.Append(" WHERE BusinessPartners.TenantId = @tenantId");
            if (!includeDeleted) queryBuilder.Append(" AND Depreciations.Deleted = @deleted");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QueryAsync<Depreciation>(queryBuilder.ToString(), parameters);
        }

        public async Task<Depreciation> GetDepreciationByIdAsync(Guid tenantId, Guid depreciationId)
        {
            var parameters = new
            {
                tenantId,
                depreciationId
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Depreciations.Id");
            queryBuilder.Append(",Depreciations.FixedAssetId");
            queryBuilder.Append(",Depreciations.PeriodId");
            queryBuilder.Append(",Depreciations.DepreciationAmount");
            queryBuilder.Append(",Depreciations.Deleted");
            queryBuilder.Append(",Depreciations.CreatedAt");
            queryBuilder.Append(",Depreciations.CreatedBy");
            queryBuilder.Append(",Depreciations.LastModificationAt");
            queryBuilder.Append(",Depreciations.LastModificationBy");
            queryBuilder.Append(" FROM Depreciations");
            queryBuilder.Append(" INNER JOIN FixedAssets ON FixedAssets.Id = Depreciations.FixedAssetId");
            queryBuilder.Append(" INNER JOIN APInvoiceLines ON APInvoiceLines.Id = FixedAssets.InvoiceLineId");
            queryBuilder.Append(" INNER JOIN APInvoices ON APInvoices.Id = APInvoiceLines.InvoiceId");
            queryBuilder.Append(" INNER JOIN BusinessPartners ON BusinessPartners.Id = APInvoices.BusinessPartnerId");
            queryBuilder.Append(" WHERE Depreciations.Id = @depreciationId");
            queryBuilder.Append(" AND BusinessPartners.TenantId = @tenantId");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<Depreciation>(queryBuilder.ToString(), parameters);
        }

        public async Task<Depreciation> UpdateDepreciationAsync(Depreciation depreciation)
        {
            var parameters = new
            {
                depreciation.Id,
                depreciation.DepreciationAmount,
                depreciation.LastModificationBy,
                depreciation.LastModificationAt,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Depreciations");
            queryBuilder.Append(" SET DepreciationAmount = @DepreciationAmount");
            queryBuilder.Append(",LastModificationAt = @LastModificationAt");
            queryBuilder.Append(",LastModificationBy = @LastModificationBy");
            queryBuilder.Append(" OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.FixedAssetId");
            queryBuilder.Append(",INSERTED.PeriodId");
            queryBuilder.Append(",INSERTED.DepreciationAmount");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(" WHERE Id = @Id");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<Depreciation>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeletedDepreciationAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Depreciations");
            queryBuilder.Append(" SET Deleted = @deleted");
            queryBuilder.Append(" WHERE Id = @id");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
        }
    }
}
