using Accounting.Context;
using Accounting.Models;
using Dapper;
using System.Text;

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
            queryBuilder.Append(" ,LastModificationBy");
            queryBuilder.Append(" FROM Loans");
            queryBuilder.Append(" WHERE Id = @loanId");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<Loan>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<Loan>> GetLoansAsync(bool includeDeleted)
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
            queryBuilder.Append(" ,LastModificationBy");
            queryBuilder.Append(" FROM Loans");
            if (includeDeleted == false) queryBuilder.Append(" WHERE Deleted = 0");


            return await _context
                .CreateConnection()
                .QueryAsync<Loan>(queryBuilder.ToString());
        }

        public async Task<Loan> InsertLoanAsync(Loan loan)
        {
            var parameters = new
            {
                loan.BusinessPartnerId,
                loan.Concept,
                loan.Amount,
                loan.AmountPaid,
                loan.LastModificationBy,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Loans (");
            queryBuilder.Append(" BusinessPartnerId");
            queryBuilder.Append(" ,Concept");
            queryBuilder.Append(" ,Amount");
            queryBuilder.Append(" ,AmountPaid");
            queryBuilder.Append(" ,LastModificationBy");
            queryBuilder.Append(" )OUTPUT INSERTED.Id");
            queryBuilder.Append(" ,INSERTED.BusinessPartnerId");
            queryBuilder.Append(" ,INSERTED.Concept");
            queryBuilder.Append(" ,INSERTED.Amount");
            queryBuilder.Append(" ,INSERTED.AmountPaid");
            queryBuilder.Append(" ,INSERTED.Deleted");
            queryBuilder.Append(" ,INSERTED.CreationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationBy");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append(" @BusinessPartnerId");
            queryBuilder.Append(" ,@Concept");
            queryBuilder.Append(" ,@Amount");
            queryBuilder.Append(" ,@AmountPaid");
            queryBuilder.Append(" ,@LastModificationBy");
            queryBuilder.Append(" )");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<Loan>(queryBuilder.ToString(), parameters);
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

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<int> UpdateLoanAsync(Loan loan)
        {
            var parameters = new
            {
                loan.Id,
                loan.BusinessPartnerId,
                loan.Concept,
                loan.Deleted,
                loan.Amount,
                loan.AmountPaid,
                loan.LastModificationBy,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Loans");
            queryBuilder.Append(" SET BusinessPartnerId = @BusinessPartnerId");
            queryBuilder.Append(" ,Concept = @Concept");
            queryBuilder.Append(" ,Deleted = @Deleted");
            queryBuilder.Append(" ,Amount = @Amount");
            queryBuilder.Append(" ,AmountPaid = @AmountPaid");
            queryBuilder.Append(" ,LastModificationBy = @LastModificationBy");
            queryBuilder.Append(" ,LastModificationDate = @LastModificationDate");
            queryBuilder.Append(" WHERE Id = @Id");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }
    }
}
