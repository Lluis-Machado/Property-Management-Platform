using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface ILoanRepository
    {
        Task<Loan> InsertLoanAsync(Loan loan);
        Task<IEnumerable<Loan>> GetLoansAsync(Guid tenantId, bool includeDeleted = false);
        Task<Loan> GetLoanByIdAsync(Guid tenantId, Guid loanId);
        Task<Loan> UpdateLoanAsync(Loan loan);
        Task<int> SetDeletedLoanAsync(Guid id, bool deleted);
    }
}
