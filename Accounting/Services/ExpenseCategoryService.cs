using AccountingAPI.DTOs;
using AccountingAPI.Models;
using AccountingAPI.Repositories;
using AutoMapper;

namespace AccountingAPI.Services
{
    public class ExpenseCategoryService : IExpenseCategoryService
    {
        private readonly IExpenseCategoryRepository _expenseCategoryRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<ExpenseCategoryService> _logger;

        public ExpenseCategoryService(IExpenseCategoryRepository expenseCategoryRepo, ILogger<ExpenseCategoryService> logger, IMapper mapper)
        {
            _expenseCategoryRepo = expenseCategoryRepo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ExpenseCategoryDTO> CreateExpenseCategoryAsync(CreateExpenseCategoryDTO createExpenseCategoryDTO, string userName)
        {
            ExpenseCategory expenseCategory = _mapper.Map<ExpenseCategory>(createExpenseCategoryDTO);
            expenseCategory.CreatedBy = userName;
            expenseCategory.LastModificationBy = userName;

            expenseCategory = await _expenseCategoryRepo.InsertExpenseCategoryAsync(expenseCategory);
            return _mapper.Map<ExpenseCategoryDTO>(expenseCategory);
        }
        public async Task<IEnumerable<ExpenseCategoryDTO>> GetExpenseCategoriesAsync(bool includeDeleted = false)
        {
            IEnumerable<ExpenseCategory> expenseCategories = await _expenseCategoryRepo.GetExpenseCategoriesAsync(includeDeleted);
            return _mapper.Map<IEnumerable<ExpenseCategoryDTO>>(expenseCategories);
        }

        public async Task<ExpenseCategoryDTO?> GetExpenseCategoryByIdAsync(Guid expenseCategoryId)
        {
            ExpenseCategory? expenseCategory = await _expenseCategoryRepo.GetExpenseCategoryByIdAsync(expenseCategoryId);
            if (expenseCategory == null) return null;
            return _mapper.Map<ExpenseCategoryDTO>(expenseCategory);
        }

        public async Task<bool> CheckIfExpenseCategoryExistsAsync(Guid expenseCategoryId)
        {
            return await _expenseCategoryRepo.GetExpenseCategoryByIdAsync(expenseCategoryId) != null;
        }

        public async Task<ExpenseCategoryDTO> UpdateExpenseCategoryAsync(CreateExpenseCategoryDTO createExpenseCategoryDTO, string userName, Guid expenseCategoryId)
        {
            ExpenseCategory expenseCategory = _mapper.Map<ExpenseCategory>(createExpenseCategoryDTO);
            expenseCategory.Id = expenseCategoryId;
            expenseCategory.LastModificationAt = DateTime.Now;
            expenseCategory.LastModificationBy = userName;

            expenseCategory = await _expenseCategoryRepo.UpdateExpenseCategoryAsync(expenseCategory);
            return _mapper.Map<ExpenseCategoryDTO>(expenseCategory);
        }

        public async Task<int> SetDeletedExpenseCategoryAsync(Guid expenseCategoryId, bool deleted)
        {
            return await _expenseCategoryRepo.SetDeletedExpenseCategoryAsync(expenseCategoryId, deleted);
        }
    }
}