using Accounting.Context;
using Accounting.Models;
using Dapper;
using System.Text;

namespace Accounting.Repositories
{
    public class AmortizationRepository  : IAmortizationRepository
    {
        private readonly DapperContext _context;
        public AmortizationRepository(DapperContext context) 
        {
            _context = context;
        }

        public async Task<Amortization> GetAmortizationByIdAsync(Guid amortizationId)
        {
            var parameters = new
            {
                amortizationId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM Amortizations");
            queryBuilder.Append(" WHERE Id = @amortizationId");

            using var connection = _context.CreateConnection();

            Amortization amortization = await connection.QuerySingleAsync<Amortization>(queryBuilder.ToString(), parameters);
            return amortization;
        }

        public async Task<IEnumerable<Amortization>> GetAmortizationsAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM Amortizations");

            using var connection = _context.CreateConnection();

            IEnumerable<Amortization> amortizations = await connection.QueryAsync<Amortization>(queryBuilder.ToString());
            return amortizations;
        }

        public async Task<Guid> InsertAmortizationAsync(Amortization amortization)
        {
            var parameters = new
            {
                amortization.FixedAssetId,
                amortization.Period,
                amortization.Amount,
                amortization.LastModificationByUser,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Amortizations (");
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

            Guid amortizationId = await connection.QuerySingleAsync<Guid>(queryBuilder.ToString(), parameters);
            return amortizationId;
        }

        public async Task<int> UpdateAmortizationAsync(Amortization amortization)
        {
            var parameters = new
            {
                amortization.Id,
                amortization.FixedAssetId,
                amortization.Period,
                amortization.Amount,
                amortization.Deleted,
                amortization.LastModificationByUser,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Amortizations ");
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
