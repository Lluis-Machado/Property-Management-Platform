using Accounting.Context;
using Accounting.Models;
using Dapper;
using System.Text;

namespace Accounting.Repositories
{
    public class DepreciationRepository  : IDepreciationRepository
    {
        private readonly DapperContext _context;
        public DepreciationRepository(DapperContext context) 
        {
            _context = context;
        }

        public async Task<Depreciation> GetDepreciationByIdAsync(Guid DepreciationId)
        {
            var parameters = new
            {
                DepreciationId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM Depreciations");
            queryBuilder.Append(" WHERE Id = @DepreciationId");

            using var connection = _context.CreateConnection();

            Depreciation Depreciation = await connection.QuerySingleAsync<Depreciation>(queryBuilder.ToString(), parameters);
            return Depreciation;
        }

        public async Task<IEnumerable<Depreciation>> GetDepreciationsAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM Depreciations");

            using var connection = _context.CreateConnection();

            IEnumerable<Depreciation> Depreciations = await connection.QueryAsync<Depreciation>(queryBuilder.ToString());
            return Depreciations;
        }

        public async Task<Guid> InsertDepreciationAsync(Depreciation Depreciation)
        {
            var parameters = new
            {
                Depreciation.FixedAssetId,
                Depreciation.Period,
                Depreciation.Amount,
                Depreciation.LastModificationByUser,
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

            Guid DepreciationId = await connection.QuerySingleAsync<Guid>(queryBuilder.ToString(), parameters);
            return DepreciationId;
        }

        public async Task<int> UpdateDepreciationAsync(Depreciation Depreciation)
        {
            var parameters = new
            {
                Depreciation.Id,
                Depreciation.FixedAssetId,
                Depreciation.Period,
                Depreciation.Amount,
                Depreciation.Deleted,
                Depreciation.LastModificationByUser,
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
