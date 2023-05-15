﻿using Accounting.Context;
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

        public async Task<ExpenseType?> GetExpenseTypeByIdAsync(Guid expenseTypeId)
        {
            var parameters = new
            {
                expenseTypeId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,Code");
            queryBuilder.Append(" ,Description");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" FROM ExpenseTypes");
            queryBuilder.Append(" WHERE Id = @expenseTypeId");

            return await _context
                .CreateConnection()
                .QuerySingleOrDefaultAsync<ExpenseType?>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<ExpenseType>> GetExpenseTypesAsync(bool includeDeleted)
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,Code");
            queryBuilder.Append(" ,Description");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" FROM ExpenseTypes");
            if (includeDeleted == false) queryBuilder.Append("WHERE Deleted = 0");


            return await _context
                .CreateConnection()
                .QueryAsync<ExpenseType>(queryBuilder.ToString());
        }

        public async Task<ExpenseType> InsertExpenseTypeAsync(ExpenseType expenseType)
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
            queryBuilder.Append(" )OUTPUT INSERTED.Id");
            queryBuilder.Append(" ,INSERTED.Id");
            queryBuilder.Append(" ,INSERTED.Code");
            queryBuilder.Append(" ,INSERTED.Description");
            queryBuilder.Append(" ,INSERTED.Deleted");
            queryBuilder.Append(" ,INSERTED.CreationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationByUser");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append(" @Code");
            queryBuilder.Append(" ,@Description");
            queryBuilder.Append(" ,@LastModificationByUser");
            queryBuilder.Append(" )");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<ExpenseType>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeleteExpenseTypeAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE ExpenseTypes");
            queryBuilder.Append(" SET Deleted = @deleted");
            queryBuilder.Append(" WHERE Id = @id");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
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
            queryBuilder.Append("UPDATE ExpenseTypes");
            queryBuilder.Append(" SET Code = @Code");
            queryBuilder.Append(" ,Description = @Description");
            queryBuilder.Append(" ,Deleted = @Deleted");
            queryBuilder.Append(" ,LastModificationByUser = @LastModificationByUser");
            queryBuilder.Append(" ,LastModificationDate = @LastModificationDate");
            queryBuilder.Append(" WHERE Id = @Id");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }
    }
}
