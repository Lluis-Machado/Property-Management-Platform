using Accounting.Models;

namespace Accounting.Repositories
{
    public interface ILoanRepository
    {
        Task<Guid> InsertLoanAsync(Loan loan);
        Task<IEnumerable<Loan>> GetLoansAsync();
        Task<Loan> GetLoanByIdAsync(Guid loanId);
        Task<int> UpdateLoanAsync(Loan loan);
    }
}
