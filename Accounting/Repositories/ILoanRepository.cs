using AccountingAPI.Models;
using AccountingAPI.DTOs;

namespace AccountingAPI.Repositories
{
    public interface ILoanRepository
    {
        Task<Loan> InsertLoanAsync(Loan loan);
        Task<IEnumerable<Loan>> GetLoansAsync(bool includeDeleted);
        Task<Loan> GetLoanByIdAsync(Guid loanId);
        Task<Loan> UpdateLoanAsync(Loan loan);
        Task<int> SetDeletedLoanAsync(Guid id, bool deleted);
    }
}
