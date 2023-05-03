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
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,LineNumber");
            queryBuilder.Append(" ,ArticleRefNumber");
            queryBuilder.Append(" ,ArticleName");
            queryBuilder.Append(" ,Tax");
            queryBuilder.Append(" ,Quantity");
            queryBuilder.Append(" ,UnitPrice");
            queryBuilder.Append(" ,TotalPrice");
            queryBuilder.Append(" ,DateRefFrom");
            queryBuilder.Append(" ,DateRefTo");
            queryBuilder.Append(" ,ExpenseTypeId");
            queryBuilder.Append(" ,InvoiceId");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" FROM InvoiceLines");

            return await _context
                .CreateConnection()
                .QueryAsync<InvoiceLine>(queryBuilder.ToString());
        }

        public async Task<InvoiceLine> GetInvoiceLineByIdAsync(Guid invoiceLineId)
        {
            var parameters = new
            {
                invoiceLineId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,LineNumber");
            queryBuilder.Append(" ,ArticleRefNumber");
            queryBuilder.Append(" ,ArticleName");
            queryBuilder.Append(" ,Tax");
            queryBuilder.Append(" ,Quantity");
            queryBuilder.Append(" ,UnitPrice");
            queryBuilder.Append(" ,TotalPrice");
            queryBuilder.Append(" ,DateRefFrom");
            queryBuilder.Append(" ,DateRefTo");
            queryBuilder.Append(" ,ExpenseTypeId");
            queryBuilder.Append(" ,InvoiceId");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" FROM InvoiceLines");
            queryBuilder.Append(" WHERE Id = @invoiceLineId");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<InvoiceLine>(queryBuilder.ToString(), parameters);
        }

        public async Task<InvoiceLine> InsertInvoiceLineAsync(InvoiceLine invoiceLine)
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
            queryBuilder.Append(" )OUTPUT INSERTED.Id");
            queryBuilder.Append(" ,INSERTED.LineNumber");
            queryBuilder.Append(" ,INSERTED.ArticleRefNumber");
            queryBuilder.Append(" ,INSERTED.ArticleName");
            queryBuilder.Append(" ,INSERTED.Tax");
            queryBuilder.Append(" ,INSERTED.Quantity");
            queryBuilder.Append(" ,INSERTED.UnitPrice");
            queryBuilder.Append(" ,INSERTED.TotalPrice");
            queryBuilder.Append(" ,INSERTED.DateRefFrom");
            queryBuilder.Append(" ,INSERTED.DateRefTo");
            queryBuilder.Append(" ,INSERTED.ExpenseTypeId");
            queryBuilder.Append(" ,INSERTED.InvoiceId");
            queryBuilder.Append(" ,INSERTED.Deleted");
            queryBuilder.Append(" ,INSERTED.CreationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationByUser");
            queryBuilder.Append(" VALUES(");
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

            return await _context
                .CreateConnection()
                .QuerySingleAsync<InvoiceLine>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeleteInvoiceLineAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE InvoiceLines");
            queryBuilder.Append(" SET Deleted = @deleted");
            queryBuilder.Append(" WHERE Id = @id");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
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
            queryBuilder.Append("UPDATE InvoiceLines");
            queryBuilder.Append(" SET LineNumber = @LineNumber");
            queryBuilder.Append(" ,ArticleRefNumber = @ArticleRefNumber");
            queryBuilder.Append(" ,ArticleName = @ArticleName");
            queryBuilder.Append(" ,Tax = @Tax");
            queryBuilder.Append(" ,Quantity = @Quantity");
            queryBuilder.Append(" ,UnitPrice = @UnitPrice");
            queryBuilder.Append(" ,TotalPrice = @TotalPrice");
            queryBuilder.Append(" ,DateRefFrom = @DateRefFrom");
            queryBuilder.Append(" ,DateRefTo = @DateRefTo");
            queryBuilder.Append(" ,ExpenseTypeId = @ExpenseTypeId");
            queryBuilder.Append(" ,Deleted = @Deleted");
            queryBuilder.Append(" ,LastModificationByUser = @LastModificationByUser");
            queryBuilder.Append(" ,LastModificationDate = @LastModificationDate");
            queryBuilder.Append(" WHERE Id = @Id");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }
    }
}
