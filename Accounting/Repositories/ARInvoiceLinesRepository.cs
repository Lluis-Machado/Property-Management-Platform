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

        public async Task<IEnumerable<ARInvoiceLine>> GetARInvoiceLinesAsync(Guid tenantId, bool includeDeleted = false)
        {
            var parameters = new
            {
                tenantId,
                deleted = includeDeleted?1:0
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT ARInvoiceLines.Id");
            queryBuilder.Append(",ARInvoiceLines.InvoiceId");
            queryBuilder.Append(",ARInvoiceLines.Description");
            queryBuilder.Append(",ARInvoiceLines.Tax");
            queryBuilder.Append(",ARInvoiceLines.Quantity");
            queryBuilder.Append(",ARInvoiceLines.UnitPrice");
            queryBuilder.Append(",ARInvoiceLines.TotalPrice");
            queryBuilder.Append(",ARInvoiceLines.ServiceDateFrom");
            queryBuilder.Append(",ARInvoiceLines.ServiceDateTo");
            queryBuilder.Append(",ARInvoiceLines.Deleted");
            queryBuilder.Append(",ARInvoiceLines.CreatedAt");
            queryBuilder.Append(",ARInvoiceLines.CreatedBy");
            queryBuilder.Append(",ARInvoiceLines.LastModificationAt");
            queryBuilder.Append(",ARInvoiceLines.LastModificationBy");
            queryBuilder.Append(" FROM ARInvoiceLines");
            queryBuilder.Append(" INNER JOIN ARInvoices ON ARInvoices.Id = ARInvoiceLines.InvoiceId");
            queryBuilder.Append(" INNER JOIN BusinessPartners ON BusinessPartners.Id = ARInvoices.BusinessPartnerId");
            queryBuilder.Append(" WHERE BusinessPartners.TenantId = @tenantId");
            if (!includeDeleted) queryBuilder.Append(" AND ARInvoiceLines.Deleted = @deleted");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QueryAsync<ARInvoiceLine>(queryBuilder.ToString(), parameters);
        }

        public async Task<ARInvoiceLine> GetARInvoiceLineByIdAsync(Guid tenantId, Guid invoiceLineId)
        {
            var parameters = new
            {
                tenantId,
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
            queryBuilder.Append(" INNER JOIN ARInvoices ON ARInvoices.Id = ARInvoiceLines.InvoiceId");
            queryBuilder.Append(" INNER JOIN BusinessPartners ON BusinessPartners.Id = ARInvoices.BusinessPartnerId");
            queryBuilder.Append(" WHERE BusinessPartners.TenantId = @tenantId");
            queryBuilder.Append(" AND ARInvoiceLines.Id = @invoiceLineId");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<ARInvoiceLine>(queryBuilder.ToString(), parameters);
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

        public async Task<int> SetDeletedARInvoiceLineAsync(Guid id, bool deleted, string userName)
        {
            var parameters = new
            {
                id,
                deleted,
                lastModificationBy = userName,
                lastModificationAt = DateTime.Now
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE ARInvoiceLines");
            queryBuilder.Append(" SET Deleted = @deleted");
            queryBuilder.Append(",LastModificationBy = @lastModificationBy");
            queryBuilder.Append(",LastModificationAt = @lastModificationAt");
            queryBuilder.Append(" WHERE Id = @id");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
        }
    }
}
