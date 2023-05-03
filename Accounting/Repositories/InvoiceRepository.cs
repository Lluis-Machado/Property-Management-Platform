using Accounting.Context;
using Accounting.Models;
using Dapper;
using System.Text;

namespace Accounting.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {

        private readonly DapperContext _context;

        public InvoiceRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Invoice> GetInvoiceByIdAsync(Guid invoiceId)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                var parameters = new
                {
                    invoiceId
                };
                StringBuilder queryBuilder = new();
                queryBuilder.Append("SELECT ID");
                queryBuilder.Append(" ,BusinessPartnerId");
                queryBuilder.Append(" ,RefNumber");
                queryBuilder.Append(" ,Date");
                queryBuilder.Append(" ,Currency");
                queryBuilder.Append(" ,GrossAmount");
                queryBuilder.Append(" ,NetAmount");
                queryBuilder.Append(" ,Deleted");
                queryBuilder.Append(" ,CreationDate");
                queryBuilder.Append(" ,LastModificationDate");
                queryBuilder.Append(" ,LastModificationByUser");
                queryBuilder.Append(" FROM Invoices ");
                queryBuilder.Append(" WHERE Id = @invoiceId");

                Invoice invoice = await connection.QuerySingleAsync<Invoice>(queryBuilder.ToString(), parameters, tran);

                StringBuilder queryBuilder2 = new();
                queryBuilder.Append("SELECT ID");
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
                queryBuilder.Append(" ,Deleted");
                queryBuilder.Append(" ,CreationDate");
                queryBuilder.Append(" ,LastModificationDate");
                queryBuilder.Append(" ,LastModificationByUser");
                queryBuilder.Append(" ,InvoiceId ");
                queryBuilder.Append(" FROM InvoiceLines");
                queryBuilder2.Append(" WHERE InvoiceId = @invoiceId");

                invoice.InvoiceLines = await connection.QuerySingleAsync<InvoiceLine[]>(queryBuilder2.ToString(), parameters, tran);

                tran.Commit();
                return invoice;
            }
            catch 
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesAsync()
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                StringBuilder queryBuilder = new();
                queryBuilder.Append("SELECT ID");
                queryBuilder.Append(" ,BusinessPartnerId");
                queryBuilder.Append(" ,RefNumber");
                queryBuilder.Append(" ,Date");
                queryBuilder.Append(" ,Currency");
                queryBuilder.Append(" ,GrossAmount");
                queryBuilder.Append(" ,NetAmount");
                queryBuilder.Append(" ,Deleted");
                queryBuilder.Append(" ,CreationDate");
                queryBuilder.Append(" ,LastModificationDate");
                queryBuilder.Append(" ,LastModificationByUser");
                queryBuilder.Append(" FROM Invoices");

                IEnumerable<Invoice> invoices = await connection.QueryAsync<Invoice>(queryBuilder.ToString(), transaction: tran);

                foreach (var invoice in invoices)
                {
                    StringBuilder queryBuilder2 = new();
                    queryBuilder2.Append("SELECT ID");
                    queryBuilder2.Append(" ,LineNumber");
                    queryBuilder2.Append(" ,ArticleRefNumber");
                    queryBuilder2.Append(" ,ArticleName");
                    queryBuilder2.Append(" ,Tax");
                    queryBuilder2.Append(" ,Quantity");
                    queryBuilder2.Append(" ,UnitPrice");
                    queryBuilder2.Append(" ,TotalPrice");
                    queryBuilder2.Append(" ,DateRefFrom");
                    queryBuilder2.Append(" ,DateRefTo");
                    queryBuilder2.Append(" ,ExpenseTypeId");
                    queryBuilder2.Append(" ,Deleted");
                    queryBuilder2.Append(" ,CreationDate");
                    queryBuilder2.Append(" ,LastModificationDate");
                    queryBuilder2.Append(" ,LastModificationByUser");
                    queryBuilder2.Append(" ,InvoiceId");
                    queryBuilder2.Append(" FROM InvoiceLines");
                    queryBuilder2.Append(" WHERE InvoiceId = @Id");

                    invoice.InvoiceLines = connection
                        .Query<InvoiceLine>(queryBuilder2.ToString(), new { invoice.Id }, tran)
                        .ToArray();
                }

                tran.Commit();

                return invoices;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<Guid> InsertInvoiceAsync(Invoice invoice)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            Array.ForEach(invoice.InvoiceLines, line => line.TotalPrice = line.UnitPrice * line.Quantity);

            invoice.GrossAmount = invoice.InvoiceLines.Sum(invoiceLine => invoiceLine.TotalPrice);
            invoice.NetAmount = invoice.InvoiceLines.Sum(invoiceLine => invoiceLine.TotalPrice / (1 + invoiceLine.Tax));

            try
            {
                var parameters = new
                {
                    invoice.BusinessPartnerId,
                    invoice.RefNumber,
                    invoice.Date,
                    invoice.Currency,
                    invoice.GrossAmount,
                    invoice.NetAmount,
                    invoice.LastModificationByUser,
                };
                StringBuilder queryBuilder = new();
                queryBuilder.Append("INSERT INTO Invoices (");
                queryBuilder.Append(" BusinessPartnerID");
                queryBuilder.Append(" ,RefNumber");
                queryBuilder.Append(" ,Date");
                queryBuilder.Append(" ,Currency");
                queryBuilder.Append(" ,GrossAmount");
                queryBuilder.Append(" ,NetAmount");
                queryBuilder.Append(" ,LastModificationByUser");
                queryBuilder.Append(" )OUTPUT INSERTED.Id VALUES(");
                queryBuilder.Append(" @BusinessPartnerID");
                queryBuilder.Append(" ,@RefNumber");
                queryBuilder.Append(" ,@Date");
                queryBuilder.Append(" ,@Currency");
                queryBuilder.Append(" ,@GrossAmount");
                queryBuilder.Append(" ,@NetAmount");
                queryBuilder.Append(" ,@LastModificationByUser");
                queryBuilder.Append(" )");


                Guid invoiceId = await connection.QuerySingleAsync<Guid>(queryBuilder.ToString(), parameters, tran);

                foreach (var invoiceLine in invoice.InvoiceLines)
                {
                    invoiceLine.InvoiceId = invoiceId;
                    var lineParameters = new
                    {
                        invoiceLine.InvoiceId,
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
                    StringBuilder lineQueryBuilder = new();
                    lineQueryBuilder.Append("INSERT INTO InvoiceLines (");
                    lineQueryBuilder.Append(" LineNumber");
                    lineQueryBuilder.Append(" ,InvoiceId");
                    lineQueryBuilder.Append(" ,ArticleRefNumber");
                    lineQueryBuilder.Append(" ,ArticleName");
                    lineQueryBuilder.Append(" ,Tax");
                    lineQueryBuilder.Append(" ,Quantity");
                    lineQueryBuilder.Append(" ,UnitPrice");
                    lineQueryBuilder.Append(" ,TotalPrice");
                    lineQueryBuilder.Append(" ,DateRefFrom");
                    lineQueryBuilder.Append(" ,DateRefTo");
                    lineQueryBuilder.Append(" ,ExpenseTypeId");
                    lineQueryBuilder.Append(" ,LastModificationByUser");
                    lineQueryBuilder.Append(" )OUTPUT INSERTED.Id VALUES(");
                    lineQueryBuilder.Append(" @LineNumber");
                    lineQueryBuilder.Append(" ,@InvoiceId");
                    lineQueryBuilder.Append(" ,@ArticleRefNumber");
                    lineQueryBuilder.Append(" ,@ArticleName");
                    lineQueryBuilder.Append(" ,@Tax");
                    lineQueryBuilder.Append(" ,@Quantity");
                    lineQueryBuilder.Append(" ,@UnitPrice");
                    lineQueryBuilder.Append(" ,@TotalPrice");
                    lineQueryBuilder.Append(" ,@DateRefFrom");
                    lineQueryBuilder.Append(" ,@DateRefTo");
                    lineQueryBuilder.Append(" ,@ExpenseTypeId");
                    lineQueryBuilder.Append(" ,@LastModificationByUser");
                    lineQueryBuilder.Append(" )");

                    await connection.QuerySingleAsync<Guid>(lineQueryBuilder.ToString(), lineParameters, tran);
                }

                tran.Commit();

                return invoiceId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<int> SetDeleteInvoiceAsync(Guid id, bool deleted)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                var parameters = new
                {
                    id,
                    deleted
                };

                StringBuilder queryBuilder = new();
                queryBuilder.Append("UPDATE Invoices");
                queryBuilder.Append(" SET Deleted = @deleted");
                queryBuilder.Append(" WHERE Id = @id");

                int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters, tran);

                StringBuilder queryBuilder2 = new();
                queryBuilder2.Append("UPDATE InvoiceLines");
                queryBuilder2.Append(" SET Deleted = @deleted");
                queryBuilder2.Append(" WHERE InvoiceId = @id");
                int rowsAffected2 = await connection.ExecuteAsync(queryBuilder2.ToString(), parameters, tran);

                tran.Commit();

                return rowsAffected;
            }
            catch
            {
                tran.Rollback();
                throw;
            }

        }

        public async Task<int> UpdateInvoiceAsync(Invoice invoice)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                StringBuilder queryBuilder1 = new();
                queryBuilder1.Append("DELETE InvoiceLines");
                queryBuilder1.Append(" WHERE InvoiceId = @Id");

                connection.Execute(queryBuilder1.ToString(), new { invoice.Id }, tran);

                Array.ForEach(invoice.InvoiceLines, line => line.TotalPrice = line.UnitPrice * line.Quantity);

                invoice.GrossAmount = invoice.InvoiceLines.Sum(invoiceLine => invoiceLine.TotalPrice);
                invoice.NetAmount = invoice.InvoiceLines.Sum(invoiceLine => invoiceLine.TotalPrice / (1 + invoiceLine.Tax));

                var parameters = new
                {
                    invoice.Id,
                    invoice.BusinessPartnerId,
                    invoice.RefNumber,
                    invoice.Date,
                    invoice.Currency,
                    invoice.GrossAmount,
                    invoice.NetAmount,
                    invoice.Deleted,
                    invoice.LastModificationByUser,
                    LastModificationDate = DateTime.Now,
                };

                StringBuilder queryBuilder = new();
                queryBuilder.Append("UPDATE Invoices");
                queryBuilder.Append(" SET BusinessPartnerID = @BusinessPartnerID");
                queryBuilder.Append(" ,RefNumber = @RefNumber");
                queryBuilder.Append(" ,Date = @Date");
                queryBuilder.Append(" ,Currency = @Currency");
                queryBuilder.Append(" ,GrossAmount = @GrossAmount");
                queryBuilder.Append(" ,NetAmount = @NetAmount");
                queryBuilder.Append(" ,Deleted = @Deleted");
                queryBuilder.Append(" ,LastModificationByUser = @LastModificationByUser");
                queryBuilder.Append(" ,LastModificationDate = @LastModificationDate");
                queryBuilder.Append(" WHERE Id = @Id");

                int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters, tran);

                foreach (var invoiceLine in invoice.InvoiceLines)
                {
                    invoiceLine.InvoiceId = invoice.Id;
                    var lineParameters = new
                    {
                        invoiceLine.InvoiceId,
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
                    StringBuilder lineQueryBuilder = new();
                    lineQueryBuilder.Append("INSERT INTO InvoiceLines (");
                    lineQueryBuilder.Append(" LineNumber");
                    lineQueryBuilder.Append(" ,InvoiceId");
                    lineQueryBuilder.Append(" ,ArticleRefNumber");
                    lineQueryBuilder.Append(" ,ArticleName");
                    lineQueryBuilder.Append(" ,Tax");
                    lineQueryBuilder.Append(" ,Quantity");
                    lineQueryBuilder.Append(" ,UnitPrice");
                    lineQueryBuilder.Append(" ,TotalPrice");
                    lineQueryBuilder.Append(" ,DateRefFrom");
                    lineQueryBuilder.Append(" ,DateRefTo");
                    lineQueryBuilder.Append(" ,ExpenseTypeId");
                    lineQueryBuilder.Append(" ,LastModificationByUser");
                    lineQueryBuilder.Append(" )OUTPUT INSERTED.Id VALUES(");
                    lineQueryBuilder.Append(" @LineNumber");
                    lineQueryBuilder.Append(" ,@InvoiceId");
                    lineQueryBuilder.Append(" ,@ArticleRefNumber");
                    lineQueryBuilder.Append(" ,@ArticleName");
                    lineQueryBuilder.Append(" ,@Tax");
                    lineQueryBuilder.Append(" ,@Quantity");
                    lineQueryBuilder.Append(" ,@UnitPrice");
                    lineQueryBuilder.Append(" ,@TotalPrice");
                    lineQueryBuilder.Append(" ,@DateRefFrom");
                    lineQueryBuilder.Append(" ,@DateRefTo");
                    lineQueryBuilder.Append(" ,@ExpenseTypeId");
                    lineQueryBuilder.Append(" ,@LastModificationByUser");
                    lineQueryBuilder.Append(" )");

                    await connection.QuerySingleAsync<Guid>(lineQueryBuilder.ToString(), lineParameters, tran);
                }

                tran.Commit();

                return rowsAffected;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }
    }
}
