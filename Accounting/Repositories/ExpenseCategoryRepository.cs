using AccountingAPI.Context;
using AccountingAPI.Models;
using Dapper;
using System.Text;

namespace AccountingAPI.Repositories
{
    public class ExpenseCategoryRepository : IExpenseCategoryRepository
    {
        private readonly IDapperContext _context;
        public ExpenseCategoryRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<ExpenseCategory?> GetExpenseCategoryByIdAsync(Guid expenseTypeId)
        {
            var parameters = new
            {
                expenseTypeId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",ExpenseTypeCode");
            queryBuilder.Append(",Name");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM ExpenseCategories");
            queryBuilder.Append(" WHERE Id = @expenseTypeId");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleOrDefaultAsync<ExpenseCategory?>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<ExpenseCategory>> GetExpenseCategoriesAsync(bool includeDeleted = false)
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",ExpenseTypeCode");
            queryBuilder.Append(",Name");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM ExpenseCategories");
            if (includeDeleted == false) queryBuilder.Append(" WHERE Deleted = 0");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QueryAsync<ExpenseCategory>(queryBuilder.ToString());
        }

        public async Task<ExpenseCategory> InsertExpenseCategoryAsync(ExpenseCategory expenseType)
        {
            var parameters = new
            {
                expenseType.ExpenseTypeCode,
                expenseType.Name,
                expenseType.CreatedBy,
                expenseType.LastModificationBy,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO ExpenseCategories (");
            queryBuilder.Append("ExpenseTypeCode");
            queryBuilder.Append(",Name");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(")OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.Id");
            queryBuilder.Append(",INSERTED.ExpenseTypeCode");
            queryBuilder.Append(",INSERTED.Name");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append("@ExpenseTypeCode");
            queryBuilder.Append(",@Name");
            queryBuilder.Append(",@CreatedBy");
            queryBuilder.Append(",@LastModificationBy");
            queryBuilder.Append(")");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<ExpenseCategory>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeletedExpenseCategoryAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE ExpenseCategories");
            queryBuilder.Append(" SET Deleted = @deleted");
            queryBuilder.Append(" WHERE Id = @id");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<ExpenseCategory> UpdateExpenseCategoryAsync(ExpenseCategory expenseType)
        {
            var parameters = new
            {
                expenseType.Id,
                expenseType.ExpenseTypeCode,
                expenseType.Name,
                expenseType.Deleted,
                expenseType.LastModificationBy,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE ExpenseCategories");
            queryBuilder.Append(" SET ExpenseTypeCode = @ExpenseTypeCode");
            queryBuilder.Append(",Name = @Name");
            queryBuilder.Append(",Deleted = @Deleted");
            queryBuilder.Append(",LastModificationBy = @LastModificationBy");
            queryBuilder.Append(",LastModificationAt = @LastModificationAt");
            queryBuilder.Append(")OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.Id");
            queryBuilder.Append(",INSERTED.ExpenseTypeCode");
            queryBuilder.Append(",INSERTED.Name");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(" WHERE Id = @Id");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<ExpenseCategory>(queryBuilder.ToString(), parameters);
        }
    }
}
