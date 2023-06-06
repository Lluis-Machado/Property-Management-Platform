using Accounting.Models;

namespace Accounting.Repositories
{
    public interface IExpenseCategoryRepository
    {
        Task<ExpenseCategory> InsertExpenseCategoryAsync(ExpenseCategory expenseType);
        Task<IEnumerable<ExpenseCategory>> GetExpenseCategoriesAsync(bool includeDeleted);
        Task<ExpenseCategory?> GetExpenseCategoriesByIdAsync(Guid expenseTypeId);
        Task<int> UpdateExpenseCategoryAsync(ExpenseCategory expenseType);
        Task<int> SetDeleteExpenseCategoryAsync(Guid id, bool deleted);
    }
}
