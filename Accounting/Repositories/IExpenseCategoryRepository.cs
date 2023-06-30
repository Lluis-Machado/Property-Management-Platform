using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface IExpenseCategoryRepository
    {
        Task<ExpenseCategory> InsertExpenseCategoryAsync(ExpenseCategory expenseCategory);
        Task<IEnumerable<ExpenseCategory>> GetExpenseCategoriesAsync(bool includeDeleted = false);
        Task<ExpenseCategory?> GetExpenseCategoryByIdAsync(Guid expenseCategoryId);
        Task<ExpenseCategory> UpdateExpenseCategoryAsync(ExpenseCategory expenseCategory);
        Task<int> SetDeletedExpenseCategoryAsync(Guid id, bool deleted, string userName);
    }
}
