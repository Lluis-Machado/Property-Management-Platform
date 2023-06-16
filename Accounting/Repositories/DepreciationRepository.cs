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
            queryBuilder.Append(",PerdioId");
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

            return await _context.Connection.QuerySingleAsync<Depreciation>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<Depreciation>> GetDepreciationsAsync(bool includeDeleted = false)
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",FixedAssetId");
            queryBuilder.Append(",PeriodId");
            queryBuilder.Append(",DepreciationAmount");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM Depreciations");
            if (!includeDeleted) queryBuilder.Append(" WHERE Deleted = 0");

            return await _context.Connection.QueryAsync<Depreciation>(queryBuilder.ToString());
        }

        public async Task<Depreciation> GetDepreciationByIdAsync(Guid depreciationId)
        {
            var parameters = new
            {
                depreciationId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",FixedAssetId");
            queryBuilder.Append(",PeriodId");
            queryBuilder.Append(",DepreciationAmount");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM Depreciations");
            queryBuilder.Append(" WHERE Id = @depreciationId");

            return await _context.Connection.QuerySingleAsync<Depreciation>(queryBuilder.ToString(), parameters);
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

            return await _context.Connection.ExecuteAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<Depreciation> UpdateDepreciationAsync(Depreciation depreciation)
        {
            var parameters = new
            {
                depreciation.Id,
                depreciation.FixedAssetId,
                depreciation.PeriodId,
                depreciation.DepreciationAmount,
                depreciation.LastModificationBy,
                depreciation.LastModificationAt,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Depreciations");
            queryBuilder.Append(" SET FixedAssetId = @FixedAssetId");
            queryBuilder.Append(",PeriodId = @PeriodId");
            queryBuilder.Append(",DepreciationAmount = @DepreciationAmount");
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

            return await _context.Connection.QuerySingleAsync<Depreciation>(queryBuilder.ToString(), parameters);
        }
    }
}
