using Accounting.Models;

namespace Accounting.Repositories
{
    public interface IExpenseTypeRepository
    {
        Task<Guid> InsertExpenseTypeAsync(ExpenseType expenseType);
        Task<IEnumerable<ExpenseType>> GetExpenseTypesAsync();
        Task<ExpenseType> GetExpenseTypeByIdAsync(Guid expenseTypeId);
        Task<int> UpdateExpenseTypeAsync(ExpenseType expenseType);
    }
}
