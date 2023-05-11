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

        public async Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId)
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
                queryBuilder.Append("SELECT Id");
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

                Invoice? invoice = await connection.QuerySingleOrDefaultAsync<Invoice?>(queryBuilder.ToString(), parameters, tran);
                if(invoice == null) 
                {
                    tran.Rollback();
                    connection.Close();
                    return null;
                }

                StringBuilder queryBuilder2 = new();
                queryBuilder2.Append("SELECT Id");
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
                queryBuilder2.Append(" ,InvoiceId ");
                queryBuilder2.Append(" FROM InvoiceLines");
                queryBuilder2.Append(" WHERE InvoiceId = @invoiceId");

                IEnumerable<InvoiceLine> aux = await connection.QueryAsync<InvoiceLine>(queryBuilder2.ToString(), parameters, tran);
                invoice.InvoiceLines = aux.ToArray();
                if (invoice.InvoiceLines == null || invoice.InvoiceLines.Length == 0)
                {
                    tran.Rollback();
                    connection.Close();
                    return null;
                }

                tran.Commit();
                connection.Close();
                return invoice;
            }
            catch 
            {
                tran.Rollback();
                connection.Close();
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
                connection.Close();
                return invoices;
            }
            catch
            {
                tran.Rollback();
                connection.Close();
                throw;
            }
        }

        public async Task<Invoice> InsertInvoiceAsync(Invoice invoice)
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
                queryBuilder.Append(" BusinessPartnerId");
                queryBuilder.Append(" ,RefNumber");
                queryBuilder.Append(" ,Date");
                queryBuilder.Append(" ,Currency");
                queryBuilder.Append(" ,GrossAmount");
                queryBuilder.Append(" ,NetAmount");
                queryBuilder.Append(" ,LastModificationByUser");
                queryBuilder.Append(" )OUTPUT INSERTED.Id");
                queryBuilder.Append(" ,INSERTED.BusinessPartnerId");
                queryBuilder.Append(" ,INSERTED.RefNumber");
                queryBuilder.Append(" ,INSERTED.Date");
                queryBuilder.Append(" ,INSERTED.Currency");
                queryBuilder.Append(" ,INSERTED.GrossAmount");
                queryBuilder.Append(" ,INSERTED.NetAmount");
                queryBuilder.Append(" ,INSERTED.Deleted");
                queryBuilder.Append(" ,INSERTED.CreationDate");
                queryBuilder.Append(" ,INSERTED.LastModificationDate");
                queryBuilder.Append(" ,INSERTED.LastModificationByUser");
                queryBuilder.Append(" VALUES(");
                queryBuilder.Append(" @BusinessPartnerId");
                queryBuilder.Append(" ,@RefNumber");
                queryBuilder.Append(" ,@Date");
                queryBuilder.Append(" ,@Currency");
                queryBuilder.Append(" ,@GrossAmount");
                queryBuilder.Append(" ,@NetAmount");
                queryBuilder.Append(" ,@LastModificationByUser");
                queryBuilder.Append(" )");

                Invoice invoiceResponse = await connection.QuerySingleAsync<Invoice>(queryBuilder.ToString(), parameters, tran);

                List<InvoiceLine> lines = new();
                foreach (var invoiceLine in invoice.InvoiceLines)
                {
                    invoiceLine.InvoiceId = invoiceResponse.Id;
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
                    lineQueryBuilder.Append(" InvoiceId");
                    lineQueryBuilder.Append(" ,LineNumber");
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
                    lineQueryBuilder.Append(" )OUTPUT INSERTED.Id");
                    lineQueryBuilder.Append(" ,INSERTED.LineNumber");
                    lineQueryBuilder.Append(" ,INSERTED.ArticleRefNumber");
                    lineQueryBuilder.Append(" ,INSERTED.ArticleName");
                    lineQueryBuilder.Append(" ,INSERTED.Tax");
                    lineQueryBuilder.Append(" ,INSERTED.Quantity");
                    lineQueryBuilder.Append(" ,INSERTED.UnitPrice");
                    lineQueryBuilder.Append(" ,INSERTED.TotalPrice");
                    lineQueryBuilder.Append(" ,INSERTED.DateRefFrom");
                    lineQueryBuilder.Append(" ,INSERTED.DateRefTo");
                    lineQueryBuilder.Append(" ,INSERTED.ExpenseTypeId");
                    lineQueryBuilder.Append(" ,INSERTED.InvoiceId");
                    lineQueryBuilder.Append(" ,INSERTED.Deleted");
                    lineQueryBuilder.Append(" ,INSERTED.CreationDate");
                    lineQueryBuilder.Append(" ,INSERTED.LastModificationDate");
                    lineQueryBuilder.Append(" ,INSERTED.LastModificationByUser");
                    lineQueryBuilder.Append(" VALUES(");
                    lineQueryBuilder.Append(" @InvoiceId");
                    lineQueryBuilder.Append(" ,@LineNumber");
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

                    lines.Add(await connection.QuerySingleAsync<InvoiceLine>(lineQueryBuilder.ToString(), lineParameters, tran));
                }

                invoiceResponse.InvoiceLines = lines.ToArray();

                tran.Commit();

                return invoiceResponse;
            }
            catch
            {
                tran.Rollback();
                connection.Close();
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
                connection.Close();

                return rowsAffected;
            }
            catch
            {
                tran.Rollback();
                connection.Close();
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
                queryBuilder.Append(" SET BusinessPartnerId = @BusinessPartnerId");
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
                connection.Close();

                return rowsAffected;
            }
            catch
            {
                tran.Rollback();
                connection.Close();
                throw;
            }
        }
    }
}
