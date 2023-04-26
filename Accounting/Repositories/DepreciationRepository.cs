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
            queryBuilder.Append("SELECT * FROM Depreciations");
            queryBuilder.Append(" WHERE Id = @depreciationId");

            using var connection = _context.CreateConnection();

            Depreciation depreciation = await connection.QuerySingleAsync<Depreciation>(queryBuilder.ToString(), parameters);
            return depreciation;
        }

        public async Task<IEnumerable<Depreciation>> GetDepreciationsAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM Depreciations");

            using var connection = _context.CreateConnection();

            IEnumerable<Depreciation> depreciations = await connection.QueryAsync<Depreciation>(queryBuilder.ToString());
            return depreciations;
        }

        public async Task<Guid> InsertDepreciationAsync(Depreciation depreciation)
        {
            var parameters = new
            {
                depreciation.FixedAssetId,
                depreciation.Period,
                depreciation.Amount,
                depreciation.LastModificationByUser,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Depreciations (");
            queryBuilder.Append(" FixedAssetId");
            queryBuilder.Append(" ,Period");
            queryBuilder.Append(" ,Amount");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" )OUTPUT INSERTED.Id VALUES(");
            queryBuilder.Append(" @FixedAssetId");
            queryBuilder.Append(" ,@Period");
            queryBuilder.Append(" ,@Amount");
            queryBuilder.Append(" ,@LastModificationByUser");
            queryBuilder.Append(" )");

            using var connection = _context.CreateConnection();

            Guid depreciationId = await connection.QuerySingleAsync<Guid>(queryBuilder.ToString(), parameters);
            return depreciationId;
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

            using var connection = _context.CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
            return rowsAffected;
        }

        public async Task<int> UpdateDepreciationAsync(Depreciation depreciation)
        {
            var parameters = new
            {
                depreciation.Id,
                depreciation.FixedAssetId,
                depreciation.Period,
                depreciation.Amount,
                depreciation.Deleted,
                depreciation.LastModificationByUser,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Depreciations ");
            queryBuilder.Append("SET FixedAssetId = @FixedAssetId ");
            queryBuilder.Append(" ,Period = @Period ");
            queryBuilder.Append(" ,Amount = @Amount ");
            queryBuilder.Append(" ,Deleted = @Deleted ");
            queryBuilder.Append(" ,LastModificationDate = @LastModificationDate ");
            queryBuilder.Append(" ,LastModificationByUser = @LastModificationByUser ");
            queryBuilder.Append(" WHERE Id = @Id ");

            using var connection = _context.CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
            return rowsAffected;
        }
    }
}
