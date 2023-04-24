using Accounting.Context;
using Accounting.Models;
using Dapper;
using System.Text;

namespace Accounting.Repositories
{
    public class InvoiceLineRepository : IInvoiceLineRepository
    {
        private readonly DapperContext _context;
        public InvoiceLineRepository(DapperContext context) 
        {
            _context = context;
        }

        public async Task<IEnumerable<InvoiceLine>> GetInvoiceLinesAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM InvoiceLines");

            using var connection = _context.CreateConnection();

            IEnumerable<InvoiceLine> invoiceLines = await connection.QueryAsync<InvoiceLine>(queryBuilder.ToString());
            return invoiceLines;
        }

        public async Task<InvoiceLine> GetInvoiceLineByIdAsync(Guid invoiceLineId)
        {
            var parameters = new
            {
                invoiceLineId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM InvoiceLines");
            queryBuilder.Append(" WHERE Id = @invoiceLineId");

            using var connection = _context.CreateConnection();

            InvoiceLine invoiceLine = await connection.QuerySingleAsync<InvoiceLine>(queryBuilder.ToString(), parameters);
            return invoiceLine;
        }

        public async Task<Guid> InsertInvoiceLineAsync(InvoiceLine invoiceLine)
        {
            var parameters = new
            {
                invoiceLine.LineNumber,
                invoiceLine.ArticleRefNumber,
                invoiceLine.ArticleName,
                invoiceLine.Tax,
                invoiceLine.Quantity,
                invoiceLine.UnitPrice,
                invoiceLine.TotalPrice,
                invoiceLine.DateRefFrom,
                invoiceLine.DateRefTo,
                invoiceLine.ExpenseTypeId,
                invoiceLine.LastModificationByUser,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO InvoiceLines (");
            queryBuilder.Append(" LineNumber");
            queryBuilder.Append(" ,ArticleRefNumber");
            queryBuilder.Append(" ,ArticleName");
            queryBuilder.Append(" ,Tax");
            queryBuilder.Append(" ,Quantity");
            queryBuilder.Append(" ,UnitPrice");
            queryBuilder.Append(" ,TotalPrice");
            queryBuilder.Append(" ,DateRefFrom");
            queryBuilder.Append(" ,DateRefTo");
            queryBuilder.Append(" ,ExpenseTypeId");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" )OUTPUT INSERTED.Id VALUES(");
            queryBuilder.Append(" @LineNumber");
            queryBuilder.Append(" ,@ArticleRefNumber");
            queryBuilder.Append(" ,@ArticleName");
            queryBuilder.Append(" ,@Tax");
            queryBuilder.Append(" ,@Quantity");
            queryBuilder.Append(" ,@UnitPrice");
            queryBuilder.Append(" ,@TotalPrice");
            queryBuilder.Append(" ,@DateRefFrom");
            queryBuilder.Append(" ,@DateRefTo");
            queryBuilder.Append(" ,@ExpenseTypeId");
            queryBuilder.Append(" ,@LastModificationByUser");
            queryBuilder.Append(" )");

            using var connection = _context.CreateConnection();

            Guid loanId = await connection.QuerySingleAsync<Guid>(queryBuilder.ToString(), parameters);
            return loanId;
        }

        public async Task<int> UpdateInvoiceLineAsync(InvoiceLine invoiceLine)
        {
            var parameters = new
            {
                invoiceLine.Id,
                invoiceLine.LineNumber,
                invoiceLine.ArticleRefNumber,
                invoiceLine.ArticleName,
                invoiceLine.Tax,
                invoiceLine.Quantity,
                invoiceLine.UnitPrice,
                invoiceLine.TotalPrice,
                invoiceLine.DateRefFrom,
                invoiceLine.DateRefTo,
                invoiceLine.ExpenseTypeId,
                invoiceLine.Deleted,
                invoiceLine.LastModificationByUser,
                LastModificationDate = DateTime.Now,
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE InvoiceLines ");
            queryBuilder.Append("SET LineNumber = @LineNumber ");
            queryBuilder.Append(" ,ArticleRefNumber = @ArticleRefNumber ");
            queryBuilder.Append(" ,ArticleName = @ArticleName ");
            queryBuilder.Append(" ,Tax = @Tax ");
            queryBuilder.Append(" ,Quantity = @Quantity ");
            queryBuilder.Append(" ,UnitPrice = @UnitPrice ");
            queryBuilder.Append(" ,TotalPrice = @TotalPrice ");
            queryBuilder.Append(" ,DateRefFrom = @DateRefFrom ");
            queryBuilder.Append(" ,DateRefTo = @DateRefTo ");
            queryBuilder.Append(" ,ExpenseTypeId = @ExpenseTypeId ");
            queryBuilder.Append(" ,Deleted = @Deleted ");
            queryBuilder.Append(" ,LastModificationByUser = @LastModificationByUser ");
            queryBuilder.Append(" ,LastModificationDate = @LastModificationDate ");
            queryBuilder.Append(" WHERE Id = @Id ");

            using var connection = _context.CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
            return rowsAffected;
        }
    }
}
