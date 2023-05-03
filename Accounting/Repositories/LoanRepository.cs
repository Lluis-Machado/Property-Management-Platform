using Dapper;
using System.Text;
using Accounting.Context;
using Accounting.Models;

namespace Accounting.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly DapperContext _context;

        public LoanRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Loan> GetLoanByIdAsync(Guid loanId)
        {
            var parameters = new
            {
                loanId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,BusinessPartnerId");
            queryBuilder.Append(" ,Concept");
            queryBuilder.Append(" ,Amount");
            queryBuilder.Append(" ,AmountPaid");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" FROM Loans");
            queryBuilder.Append(" WHERE Id = @loanId");

            using var connection = _context.CreateConnection();

            Loan loan = await connection.QuerySingleAsync<Loan>(queryBuilder.ToString(), parameters);
            return loan;
        }

        public async Task<IEnumerable<Loan>> GetLoansAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,BusinessPartnerId");
            queryBuilder.Append(" ,Concept");
            queryBuilder.Append(" ,Amount");
            queryBuilder.Append(" ,AmountPaid");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" FROM Loans");

            using var connection = _context.CreateConnection();

            IEnumerable<Loan> loans = await connection.QueryAsync<Loan>(queryBuilder.ToString());
            return loans;
        }

        public async Task<Guid> InsertLoanAsync(Loan loan)
        {
            var parameters = new
            {
                loan.BusinessPartnerID,
                loan.Concept,
                loan.Amount,
                loan.AmountPaid,
                loan.LastModificationByUser,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Loans (");
            queryBuilder.Append(" BusinessPartnerID");
            queryBuilder.Append(" ,Concept");
            queryBuilder.Append(" ,Amount");
            queryBuilder.Append(" ,AmountPaid");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" )OUTPUT INSERTED.Id VALUES(");
            queryBuilder.Append(" @BusinessPartnerID");
            queryBuilder.Append(" ,@Concept");
            queryBuilder.Append(" ,@Amount");
            queryBuilder.Append(" ,@AmountPaid");
            queryBuilder.Append(" ,@LastModificationByUser");
            queryBuilder.Append(" )");

            using var connection = _context.CreateConnection();

            Guid loanId = await connection.QuerySingleAsync<Guid>(queryBuilder.ToString(), parameters);
            return loanId;
        }

        public async Task<int> SetDeleteLoanAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Loans");
            queryBuilder.Append(" SET Deleted = @deleted");
            queryBuilder.Append(" WHERE Id = @id");

            using var connection = _context.CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
            return rowsAffected;
        }

        public async Task<int> UpdateLoanAsync(Loan loan)
        {
            var parameters = new
            {
                loan.Id,
                loan.BusinessPartnerID,
                loan.Concept,
                loan.Deleted,
                loan.Amount,
                loan.AmountPaid,
                loan.LastModificationByUser,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Loans");
            queryBuilder.Append(" SET BusinessPartnerID = @BusinessPartnerID");
            queryBuilder.Append(" ,Concept = @Concept");
            queryBuilder.Append(" ,Deleted = @Deleted");
            queryBuilder.Append(" ,Amount = @Amount");
            queryBuilder.Append(" ,AmountPaid = @AmountPaid");
            queryBuilder.Append(" ,LastModificationByUser = @LastModificationByUser");
            queryBuilder.Append(" ,LastModificationDate = @LastModificationDate");
            queryBuilder.Append(" WHERE Id = @Id");

            using var connection = _context.CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
            return rowsAffected;
        }
    }
}
