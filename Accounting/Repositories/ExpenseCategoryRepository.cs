using Accounting.Context;
using Accounting.Models;
using Dapper;
using System.Text;

namespace Accounting.Repositories
{
    public class ExpenseCategoryRepository : IExpenseCategoryRepository
    {
        private readonly DapperContext _context;
        public ExpenseCategoryRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<ExpenseCategory?> GetExpenseCategoriesByIdAsync(Guid expenseTypeId)
        {
            var parameters = new
            {
                expenseTypeId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,ExpenseTypeCode");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationBy");
            queryBuilder.Append(" FROM ExpenseCategorys");
            queryBuilder.Append(" WHERE Id = @expenseTypeId");

            return await _context
                .CreateConnection()
                .QuerySingleOrDefaultAsync<ExpenseCategory?>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<ExpenseCategory>> GetExpenseCategoriesAsync(bool includeDeleted)
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,ExpenseTypeCode");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationBy");
            queryBuilder.Append(" FROM ExpenseCategorys");
            if (includeDeleted == false) queryBuilder.Append(" WHERE Deleted = 0");


            return await _context
                .CreateConnection()
                .QueryAsync<ExpenseCategory>(queryBuilder.ToString());
        }

        public async Task<ExpenseCategory> InsertExpenseCategoryAsync(ExpenseCategory expenseType)
        {
            var parameters = new
            {
                expenseType.ExpenseTypeCode,
                expenseType.Name,
                expenseType.LastModificationBy,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO ExpenseCategorys (");
            queryBuilder.Append(" ExpenseTypeCode");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" ,LastModificationBy");
            queryBuilder.Append(" )OUTPUT INSERTED.Id");
            queryBuilder.Append(" ,INSERTED.Id");
            queryBuilder.Append(" ,INSERTED.ExpenseTypeCode");
            queryBuilder.Append(" ,INSERTED.Name");
            queryBuilder.Append(" ,INSERTED.Deleted");
            queryBuilder.Append(" ,INSERTED.CreationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationBy");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append(" @ExpenseTypeCode");
            queryBuilder.Append(" ,@Name");
            queryBuilder.Append(" ,@LastModificationBy");
            queryBuilder.Append(" )");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<ExpenseCategory>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeleteExpenseCategoryAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE ExpenseCategorys");
            queryBuilder.Append(" SET Deleted = @deleted");
            queryBuilder.Append(" WHERE Id = @id");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<int> UpdateExpenseCategoryAsync(ExpenseCategory expenseType)
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
            queryBuilder.Append("UPDATE ExpenseCategorys");
            queryBuilder.Append(" SET ExpenseTypeCode = @ExpenseTypeCode");
            queryBuilder.Append(" ,Name = @Name");
            queryBuilder.Append(" ,Deleted = @Deleted");
            queryBuilder.Append(" ,LastModificationBy = @LastModificationBy");
            queryBuilder.Append(" ,LastModificationDate = @LastModificationDate");
            queryBuilder.Append(" WHERE Id = @Id");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }
    }
}
