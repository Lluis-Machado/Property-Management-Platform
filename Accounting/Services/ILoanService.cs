using AccountingAPI.Models;
using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface ILoanService
    {
        Task<LoanDTO> CreateLoanAsync(CreateLoanDTO createLoanDTO, string userName);
        Task<IEnumerable<LoanDTO>> GetLoansAsync(bool includeDeleted = false);
        Task<LoanDTO?> GetLoanByIdAsync(Guid LoanId);
        Task<bool> CheckIfLoanExistsAsync(Guid LoanId);
        Task<LoanDTO> UpdateLoanAsync(CreateLoanDTO createLoanDTO, string userName);
        Task<int> SetDeletedLoanAsync(Guid loanId, bool deleted);
    }
}
