using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface ILoanService
    {
        Task<LoanDTO> CreateLoanAsync(CreateLoanDTO createLoanDTO, string userName);
        Task<IEnumerable<LoanDTO>> GetLoansAsync(Guid tenantId, bool includeDeleted = false);
        Task<LoanDTO> GetLoanByIdAsync(Guid tenantId, Guid LoanId);
        Task<LoanDTO> UpdateLoanAsync(Guid tenantId, Guid loanId, UpdateLoanDTO updateLoanDTO, string userName);
        Task<int> SetDeletedLoanAsync(Guid tenantId, Guid loanId, bool deleted);
    }
}
