using AccountingAPI.Context;
using AccountingAPI.Models;
using Dapper;
using System.Text;

namespace AccountingAPI.Repositories
{
    public class ARInvoiceLinesRepository : IARInvoiceLineRepository
    {
        private readonly IDapperContext _context;
        public ARInvoiceLinesRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ARInvoiceLine>> GetARInvoiceLinesAsync(bool includeDeleted = false)
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",InvoiceId");
            queryBuilder.Append(",Description");
            queryBuilder.Append(",Tax");
            queryBuilder.Append(",Quantity");
            queryBuilder.Append(",UnitPrice");
            queryBuilder.Append(",TotalPrice");
            queryBuilder.Append(",ServiceDateFrom");
            queryBuilder.Append(",ServiceDateTo");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM ARInvoiceLines");
            if (includeDeleted == false) queryBuilder.Append(" WHERE Deleted = 0");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QueryAsync<ARInvoiceLine>(queryBuilder.ToString());
        }

        public async Task<ARInvoiceLine> GetARInvoiceLineByIdAsync(Guid invoiceLineId)
        {
            var parameters = new
            {
                invoiceLineId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",InvoiceId");
            queryBuilder.Append(",Description");
            queryBuilder.Append(",Tax");
            queryBuilder.Append(",Quantity");
            queryBuilder.Append(",UnitPrice");
            queryBuilder.Append(",TotalPrice");
            queryBuilder.Append(",ServiceDateFrom");
            queryBuilder.Append(",ServiceDateTo");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM ARInvoiceLines");
            queryBuilder.Append(" WHERE Id = @invoiceLineId");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<ARInvoiceLine>(queryBuilder.ToString(), parameters);
        }

        public async Task<ARInvoiceLine> InsertARInvoiceLineAsync(ARInvoiceLine invoiceLine)
        {
            var parameters = new
            {
                invoiceLine.InvoiceId,
                invoiceLine.Description,
                invoiceLine.Tax,
                invoiceLine.Quantity,
                invoiceLine.UnitPrice,
                invoiceLine.TotalPrice,
                invoiceLine.ServiceDateFrom,
                invoiceLine.ServiceDateTo,
                invoiceLine.CreatedBy,
                invoiceLine.LastModificationBy,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO ARInvoiceLines (");
            queryBuilder.Append("InvoiceId");
            queryBuilder.Append(",Description");
            queryBuilder.Append(",Tax");
            queryBuilder.Append(",Quantity");
            queryBuilder.Append(",UnitPrice");
            queryBuilder.Append(",TotalPrice");
            queryBuilder.Append(",ServiceDateFrom");
            queryBuilder.Append(",ServiceDateTo");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(")OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.InvoiceId");
            queryBuilder.Append(",INSERTED.Description");
            queryBuilder.Append(",INSERTED.Tax");
            queryBuilder.Append(",INSERTED.Quantity");
            queryBuilder.Append(",INSERTED.UnitPrice");
            queryBuilder.Append(",INSERTED.TotalPrice");
            queryBuilder.Append(",INSERTED.ServiceDateFrom");
            queryBuilder.Append(",INSERTED.ServiceDateTo");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append("@InvoiceId");
            queryBuilder.Append(",@Description");
            queryBuilder.Append(",@Tax");
            queryBuilder.Append(",@Quantity");
            queryBuilder.Append(",@UnitPrice");
            queryBuilder.Append(",@TotalPrice");
            queryBuilder.Append(",@ServiceDateFrom");
            queryBuilder.Append(",@ServiceDateTo");
            queryBuilder.Append(",@CreatedBy");
            queryBuilder.Append(",@LastModificationBy");
            queryBuilder.Append(")");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<ARInvoiceLine>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeletedARInvoiceLineAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE ARInvoiceLines");
            queryBuilder.Append(" SET Deleted = @deleted");
            queryBuilder.Append(" WHERE Id = @id");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<ARInvoiceLine> UpdateARInvoiceLineAsync(ARInvoiceLine invoiceLine)
        {
            var parameters = new
            {
                invoiceLine.Id,
                invoiceLine.Description,
                invoiceLine.Tax,
                invoiceLine.Quantity,
                invoiceLine.UnitPrice,
                invoiceLine.TotalPrice,
                invoiceLine.ServiceDateFrom,
                invoiceLine.ServiceDateTo,
                invoiceLine.Deleted,
                invoiceLine.LastModificationAt,
                invoiceLine.LastModificationBy
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE ARInvoiceLines");
            queryBuilder.Append(" SET Description = @Description");
            queryBuilder.Append(",Tax = @Tax");
            queryBuilder.Append(",Quantity = @Quantity");
            queryBuilder.Append(",UnitPrice = @UnitPrice");
            queryBuilder.Append(",TotalPrice = @TotalPrice");
            queryBuilder.Append(",ServiceDateFrom = @ServiceDateFrom");
            queryBuilder.Append(",ServiceDateTo = @ServiceDateTo");
            queryBuilder.Append(",Deleted = @Deleted");
            queryBuilder.Append(",LastModificationBy = @LastModificationBy");
            queryBuilder.Append(",LastModificationAt = @LastModificationAt");
            queryBuilder.Append(" OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.InvoiceId");
            queryBuilder.Append(",INSERTED.Description");
            queryBuilder.Append(",INSERTED.Tax");
            queryBuilder.Append(",INSERTED.Quantity");
            queryBuilder.Append(",INSERTED.UnitPrice");
            queryBuilder.Append(",INSERTED.TotalPrice");
            queryBuilder.Append(",INSERTED.ServiceDateFrom");
            queryBuilder.Append(",INSERTED.ServiceDateTo");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(" WHERE Id = @Id");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<ARInvoiceLine>(queryBuilder.ToString(), parameters);
        }
    }
}
