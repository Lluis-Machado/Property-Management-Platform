using Accounting.Context;
using Accounting.Models;
using Dapper;
using System.Text;

namespace Accounting.Repositories
{
    public class DepreciationRepository : IDepreciationRepository
    {
        private readonly DapperContext _context;
        public DepreciationRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Depreciation> GetDepreciationByIdAsync(Guid depreciationId)
        {
            var parameters = new
            {
                depreciationId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,FixedAssetId");
            queryBuilder.Append(" ,PeriodStart");
            queryBuilder.Append(" ,PeriodEnd");
            queryBuilder.Append(" ,Amount");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" FROM Depreciations");
            queryBuilder.Append(" WHERE Id = @depreciationId");

            return await _context.
                CreateConnection().
                QuerySingleAsync<Depreciation>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<Depreciation>> GetDepreciationByFAandPeriodAsync(Guid fixedAssetId, bool includeDeleted, DateTime? periodStart, DateTime? periodEnd)
        {
            var parameters = new
            {
                fixedAssetId,
                pS = periodStart != null ? ((DateTime)periodStart).ToString("yyyyMMdd") : null,
                pE = periodEnd != null ? ((DateTime)periodEnd).ToString("yyyyMMdd") : null,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,FixedAssetId");
            queryBuilder.Append(" ,PeriodStart");
            queryBuilder.Append(" ,PeriodEnd");
            queryBuilder.Append(" ,Amount");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" FROM Depreciations");
            queryBuilder.Append(" WHERE FixedAssetId = @fixedAssetId");
            if (includeDeleted == false) queryBuilder.Append("AND Deleted = 0");
            if (periodStart != null) queryBuilder.Append(" AND PeriodStart >= @pS");
            if (periodEnd != null) queryBuilder.Append(" AND PeriodEnd <= @pE");

            return await _context.
                CreateConnection().
                QueryAsync<Depreciation>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<Depreciation>> GetDepreciationsAsync(bool includeDeleted)
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,FixedAssetId");
            queryBuilder.Append(" ,PeriodStart");
            queryBuilder.Append(" ,PeriodEnd");
            queryBuilder.Append(" ,Amount");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" FROM Depreciations");
            if (includeDeleted == false) queryBuilder.Append(" WHERE Deleted = 0");


            return await _context
                .CreateConnection()
                .QueryAsync<Depreciation>(queryBuilder.ToString());
        }

        public async Task<Depreciation> InsertDepreciationAsync(Depreciation depreciation)
        {
            var parameters = new
            {
                depreciation.FixedAssetId,
                depreciation.PeriodStart,
                depreciation.PeriodEnd,
                depreciation.Amount,
                depreciation.LastModificationByUser,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Depreciations (");
            queryBuilder.Append(" FixedAssetId");
            queryBuilder.Append(" ,PeriodStart");
            queryBuilder.Append(" ,PeriodEnd");
            queryBuilder.Append(" ,Amount");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" )OUTPUT INSERTED.Id");
            queryBuilder.Append(" ,INSERTED.FixedAssetId");
            queryBuilder.Append(" ,INSERTED.PeriodStart");
            queryBuilder.Append(" ,INSERTED.PeriodEnd");
            queryBuilder.Append(" ,INSERTED.Amount");
            queryBuilder.Append(" ,INSERTED.Deleted");
            queryBuilder.Append(" ,INSERTED.CreationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationByUser");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append(" @FixedAssetId");
            queryBuilder.Append(" ,@PeriodStart");
            queryBuilder.Append(" ,@PeriodEnd");
            queryBuilder.Append(" ,@Amount");
            queryBuilder.Append(" ,@LastModificationByUser");
            queryBuilder.Append(" )");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<Depreciation>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeleteDepreciationAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Depreciations ");
            queryBuilder.Append("SET Deleted = @deleted ");
            queryBuilder.Append(" WHERE Id = @id ");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<int> UpdateDepreciationAsync(Depreciation depreciation)
        {
            var parameters = new
            {
                depreciation.Id,
                depreciation.FixedAssetId,
                depreciation.PeriodStart,
                depreciation.PeriodEnd,
                depreciation.Amount,
                depreciation.Deleted,
                depreciation.LastModificationByUser,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Depreciations ");
            queryBuilder.Append("SET FixedAssetId = @FixedAssetId ");
            queryBuilder.Append(" ,PeriodStart = @PeriodStart ");
            queryBuilder.Append(" ,PeriodEnd = @PeriodEnd ");
            queryBuilder.Append(" ,Amount = @Amount ");
            queryBuilder.Append(" ,Deleted = @Deleted ");
            queryBuilder.Append(" ,LastModificationDate = @LastModificationDate ");
            queryBuilder.Append(" ,LastModificationByUser = @LastModificationByUser ");
            queryBuilder.Append(" WHERE Id = @Id ");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<int> UpdateTotalDepreciationForFixedAsset(Guid fixedAssetId)
        {
            var parameters = new
            {
                fixedAssetId
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE FixedAssets ");
            queryBuilder.Append("SET DepreciatedAmount = ( ");
            queryBuilder.Append(" SELECT SUM(Depreciations.Amount) FROM Depreciations ");
            queryBuilder.Append("  WHERE Depreciations.FixedAssetId = @fixedAssetId");
            queryBuilder.Append("  AND Depreciations.Deleted = 0)");
            queryBuilder.Append(" WHERE FixedAssets.Id = @fixedAssetId");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }

    }
}
