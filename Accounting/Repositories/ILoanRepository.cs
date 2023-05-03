using Accounting.Models;

namespace Accounting.Repositories
{
    public interface ILoanRepository
    {
        Task<Loan> InsertLoanAsync(Loan loan);
        Task<IEnumerable<Loan>> GetLoansAsync();
        Task<Loan> GetLoanByIdAsync(Guid loanId);
        Task<int> UpdateLoanAsync(Loan loan);
        Task<int> SetDeleteLoanAsync(Guid id, bool deleted);
    }
}
