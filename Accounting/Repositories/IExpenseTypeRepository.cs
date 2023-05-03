using Accounting.Models;

namespace Accounting.Repositories
{
    public interface IExpenseTypeRepository
    {
        Task<ExpenseType> InsertExpenseTypeAsync(ExpenseType expenseType);
        Task<IEnumerable<ExpenseType>> GetExpenseTypesAsync();
        Task<ExpenseType> GetExpenseTypeByIdAsync(Guid expenseTypeId);
        Task<int> UpdateExpenseTypeAsync(ExpenseType expenseType);
        Task<int> SetDeleteExpenseTypeAsync(Guid id, bool deleted);
    }
}
