using Accounting.Context;
using Accounting.Models;
using Dapper;
using System.Text;

namespace Accounting.Repositories
{
    public class ExpenseTypeRepository : IExpenseTypeRepository
    {
        private readonly DapperContext _context;
        public ExpenseTypeRepository(DapperContext context) 
        {
            _context = context;
        }

        public async Task<ExpenseType> GetExpenseTypeByIdAsync(Guid expenseTypeId)
        {
            var parameters = new
            {
                expenseTypeId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM ExpenseTypes");
            queryBuilder.Append(" WHERE Id = @expenseTypeId");

            using var connection = _context.CreateConnection();

            ExpenseType expenseType = await connection.QuerySingleAsync<ExpenseType>(queryBuilder.ToString(), parameters);
            return expenseType;
        }

        public async Task<IEnumerable<ExpenseType>> GetExpenseTypesAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM ExpenseTypes");

            using var connection = _context.CreateConnection();

            IEnumerable<ExpenseType> expenseTypes = await connection.QueryAsync<ExpenseType>(queryBuilder.ToString());
            return expenseTypes;
        }

        public async Task<Guid> InsertExpenseTypeAsync(ExpenseType expenseType)
        {
            var parameters = new
            {
                expenseType.Code,
                expenseType.Description,
                expenseType.LastModificationByUser,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO ExpenseTypes (");
            queryBuilder.Append(" Code");
            queryBuilder.Append(" ,Description");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" )OUTPUT INSERTED.Id VALUES(");
            queryBuilder.Append(" @Code");
            queryBuilder.Append(" ,@Description");
            queryBuilder.Append(" ,@LastModificationByUser");
            queryBuilder.Append(" )");

            using var connection = _context.CreateConnection();

            Guid expenseTypeId = await connection.QuerySingleAsync<Guid>(queryBuilder.ToString(), parameters);
            return expenseTypeId;
        }

        public async Task<int> UpdateExpenseTypeAsync(ExpenseType expenseType)
        {
            var parameters = new
            {
                expenseType.Id,
                expenseType.Code,
                expenseType.Description,
                expenseType.Deleted,
                expenseType.LastModificationByUser,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE ExpenseTypes ");
            queryBuilder.Append("SET Code = @Code ");
            queryBuilder.Append(" ,Description = @Description ");
            queryBuilder.Append(" ,Deleted = @Deleted ");
            queryBuilder.Append(" ,LastModificationByUser = @LastModificationByUser ");
            queryBuilder.Append(" ,LastModificationDate = @LastModificationDate ");
            queryBuilder.Append(" WHERE Id = @Id ");

            using var connection = _context.CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
            return rowsAffected;
        }
    }
}
