using AccountingAPI.DTOs;
using AccountingAPI.Exceptions;
using AccountingAPI.Models;
using AccountingAPI.Repositories;
using AutoMapper;
using FluentValidation;

namespace AccountingAPI.Services
{
    public class ExpenseCategoryService : IExpenseCategoryService
    {
        private readonly IExpenseCategoryRepository _expenseCategoryRepo;
        private readonly IValidator<CreateExpenseCategoryDTO> _createExpenseCategoryDTOValidator;
        private readonly IValidator<UpdateExpenseCategoryDTO> _updateExpenseCategoryDTOValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<ExpenseCategoryService> _logger;

        public ExpenseCategoryService(IExpenseCategoryRepository expenseCategoryRepo, IValidator<CreateExpenseCategoryDTO> createExpenseCategoryDTOValidator, IValidator<UpdateExpenseCategoryDTO> updateExpenseCategoryDTOValidator, ILogger<ExpenseCategoryService> logger, IMapper mapper)
        {
            _expenseCategoryRepo = expenseCategoryRepo;
            _createExpenseCategoryDTOValidator = createExpenseCategoryDTOValidator;
            _updateExpenseCategoryDTOValidator = updateExpenseCategoryDTOValidator;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ExpenseCategoryDTO> CreateExpenseCategoryAsync(CreateExpenseCategoryDTO createExpenseCategoryDTO, string userName)
        {
            // validation
            await _createExpenseCategoryDTOValidator.ValidateAndThrowAsync(createExpenseCategoryDTO);

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

        public async Task<ExpenseCategoryDTO> GetExpenseCategoryByIdAsync(Guid expenseCategoryId)
        {
            ExpenseCategory? expenseCategory = await _expenseCategoryRepo.GetExpenseCategoryByIdAsync(expenseCategoryId);

            if (expenseCategory is null) throw new NotFoundException("Expense Category");

            return _mapper.Map<ExpenseCategoryDTO>(expenseCategory);
        }

        public async Task<ExpenseCategoryDTO> UpdateExpenseCategoryAsync(Guid expenseCategoryId, UpdateExpenseCategoryDTO updateExpenseCategoryDTO, string userName)
        {
            // validation
            await _updateExpenseCategoryDTOValidator.ValidateAndThrowAsync(updateExpenseCategoryDTO);

            // check if exists
            await GetExpenseCategoryByIdAsync(expenseCategoryId);

            ExpenseCategory expenseCategory = _mapper.Map<ExpenseCategory>(updateExpenseCategoryDTO);
            expenseCategory.Id = expenseCategoryId;
            expenseCategory.LastModificationAt = DateTime.Now;
            expenseCategory.LastModificationBy = userName;

            expenseCategory = await _expenseCategoryRepo.UpdateExpenseCategoryAsync(expenseCategory);
            return _mapper.Map<ExpenseCategoryDTO>(expenseCategory);
        }

        public async Task SetDeletedExpenseCategoryAsync(Guid expenseCategoryId, bool deleted, string userName)
        {
            // check if exists
            ExpenseCategoryDTO expenseCategoryDTO = await GetExpenseCategoryByIdAsync(expenseCategoryId);

            // check if already deleted/undeleted
            if (expenseCategoryDTO.Deleted == deleted)
            {
                string action = deleted ? "deleted" : "undeleted";
                throw new ConflictException($"Expense category already {action}");
            }

            await _expenseCategoryRepo.SetDeletedExpenseCategoryAsync(expenseCategoryId, deleted, userName);
        }
    }
}