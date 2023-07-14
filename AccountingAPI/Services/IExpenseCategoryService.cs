using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IExpenseCategoryService
    {
        Task<ExpenseCategoryDTO> CreateExpenseCategoryAsync(CreateExpenseCategoryDTO createExpenseCategoryDTO, string userName);
        Task<IEnumerable<ExpenseCategoryDTO>> GetExpenseCategoriesAsync(bool includeDeleted = false);
        Task<ExpenseCategoryDTO> GetExpenseCategoryByIdAsync(Guid expenseCategoryId);
        Task<ExpenseCategoryDTO> UpdateExpenseCategoryAsync(Guid expenseCategoryId, UpdateExpenseCategoryDTO updateExpenseCategoryDTO, string userName);
        Task SetDeletedExpenseCategoryAsync(Guid expenseCategoryId, bool deleted, string userName);
    }
}
