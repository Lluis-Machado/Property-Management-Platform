using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IExpenseCategoryService
    {
        Task<ExpenseCategoryDTO> CreateExpenseCategoryAsync(CreateExpenseCategoryDTO createExpenseCategoryDTO, string userName);
        Task<IEnumerable<ExpenseCategoryDTO>> GetExpenseCategoriesAsync(bool includeDeleted = false);
        Task<bool> CheckIfExpenseCategoryExistsAsync(Guid expenseCategoryId);
        Task<ExpenseCategoryDTO?> GetExpenseCategoryByIdAsync(Guid expenseCategoryId);
        Task<ExpenseCategoryDTO> UpdateExpenseCategoryAsync(CreateExpenseCategoryDTO createExpenseCategoryDTO, string userName, Guid expenseCategoryId);
        Task<int> SetDeletedExpenseCategoryAsync(Guid expenseCategoryId, bool deleted);
    }
}
