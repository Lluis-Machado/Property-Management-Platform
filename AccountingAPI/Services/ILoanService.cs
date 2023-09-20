using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface ILoanService
    {
        Task<LoanDTO> CreateLoanAsync(Guid tenantId, Guid BusinessPartnerId, CreateLoanDTO createLoanDTO, string userName);
        Task<IEnumerable<LoanDTO>> GetLoansAsync(Guid tenantId, int? page, int? pageSize, bool includeDeleted = false);
        Task<LoanDTO> GetLoanByIdAsync(Guid tenantId, Guid LoanId);
        Task<LoanDTO> UpdateLoanAsync(Guid tenantId, Guid loanId, UpdateLoanDTO updateLoanDTO, string userName);
        Task SetDeletedLoanAsync(Guid tenantId, Guid loanId, bool deleted, string userName);
    }
}
