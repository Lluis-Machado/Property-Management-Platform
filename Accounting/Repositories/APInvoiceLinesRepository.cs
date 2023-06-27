using AccountingAPI.Context;
using AccountingAPI.Models;
using Dapper;
using System.Text;

namespace AccountingAPI.Repositories
{
    public class APInvoiceLinesRepository : IAPInvoiceLineRepository
    {
        private readonly IDapperContext _context;
        public APInvoiceLinesRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<APInvoiceLine> InsertAPInvoiceLineAsync(APInvoiceLine invoiceLine)
        {
            var parameters = new
            {
                invoiceLine.InvoiceId,
                invoiceLine.Description,
                invoiceLine.Tax,
                invoiceLine.Quantity,
                invoiceLine.UnitPrice,
                invoiceLine.TotalPrice,
                invoiceLine.ExpenseCategoryId,
                invoiceLine.ServiceDateFrom,
                invoiceLine.ServiceDateTo,
                invoiceLine.FixedAssetId,
                invoiceLine.CreatedBy,
                invoiceLine.LastModificationBy,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO APInvoiceLines (");
            queryBuilder.Append("InvoiceId");
            queryBuilder.Append(",Description");
            queryBuilder.Append(",Tax");
            queryBuilder.Append(",Quantity");
            queryBuilder.Append(",UnitPrice");
            queryBuilder.Append(",TotalPrice");
            queryBuilder.Append(",ExpenseCategoryId");
            queryBuilder.Append(",ServiceDateFrom");
            queryBuilder.Append(",ServiceDateTo");
            queryBuilder.Append(",FixedAssetId");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(")OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.InvoiceId");
            queryBuilder.Append(",INSERTED.Description");
            queryBuilder.Append(",INSERTED.Tax");
            queryBuilder.Append(",INSERTED.Quantity");
            queryBuilder.Append(",INSERTED.UnitPrice");
            queryBuilder.Append(",INSERTED.TotalPrice");
            queryBuilder.Append(",INSERTED.ExpenseCategoryId");
            queryBuilder.Append(",INSERTED.ServiceDateFrom");
            queryBuilder.Append(",INSERTED.ServiceDateTo");
            queryBuilder.Append(",INSERTED.FixedAssetId");
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
            queryBuilder.Append(",@ExpenseCategoryId");
            queryBuilder.Append(",@ServiceDateFrom");
            queryBuilder.Append(",@ServiceDateTo");
            queryBuilder.Append(",@FixedAssetId");
            queryBuilder.Append(",@CreatedBy");
            queryBuilder.Append(",@LastModificationBy");
            queryBuilder.Append(")");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<APInvoiceLine>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<APInvoiceLine>> GetAPInvoiceLinesAsync(Guid tenantId, bool includeDeleted = false)
        {
            var parameters = new
            {
                tenantId,
                deleted = includeDeleted?1:0
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT APInvoiceLines.Id");
            queryBuilder.Append(",APInvoiceLines.InvoiceId");
            queryBuilder.Append(",APInvoiceLines.Description");
            queryBuilder.Append(",APInvoiceLines.Tax");
            queryBuilder.Append(",APInvoiceLines.Quantity");
            queryBuilder.Append(",APInvoiceLines.UnitPrice");
            queryBuilder.Append(",APInvoiceLines.TotalPrice");
            queryBuilder.Append(",APInvoiceLines.ExpenseCategoryId");
            queryBuilder.Append(",APInvoiceLines.ServiceDateFrom");
            queryBuilder.Append(",APInvoiceLines.ServiceDateTo");
            queryBuilder.Append(",APInvoiceLines.FixedAssetId");
            queryBuilder.Append(",APInvoiceLines.Deleted");
            queryBuilder.Append(",APInvoiceLines.CreatedAt");
            queryBuilder.Append(",APInvoiceLines.CreatedBy");
            queryBuilder.Append(",APInvoiceLines.LastModificationAt");
            queryBuilder.Append(",APInvoiceLines.LastModificationBy");
            queryBuilder.Append(" FROM APInvoiceLines");
            queryBuilder.Append(" INNER JOIN APInvoices ON APInvoices.Id = APInvoiceLines.InvoiceId");
            queryBuilder.Append(" INNER JOIN BusinessPartners ON BusinessPartners.Id = APInvoices.BusinessPartnerId");
            queryBuilder.Append(" WHERE BusinessPartners.TenantId = @tenantId");
            if (!includeDeleted) queryBuilder.Append(" AND APInvoiceLines.Deleted = @deleted");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QueryAsync<APInvoiceLine>(queryBuilder.ToString(), parameters);
        }

        public async Task<APInvoiceLine> GetAPInvoiceLineByIdAsync(Guid tenantId, Guid invoiceLineId)
        {
            var parameters = new
            {
                tenantId,
                invoiceLineId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT APInvoiceLines.Id");
            queryBuilder.Append(",APInvoiceLines.InvoiceId");
            queryBuilder.Append(",APInvoiceLines.Description");
            queryBuilder.Append(",APInvoiceLines.Tax");
            queryBuilder.Append(",APInvoiceLines.Quantity");
            queryBuilder.Append(",APInvoiceLines.UnitPrice");
            queryBuilder.Append(",APInvoiceLines.TotalPrice");
            queryBuilder.Append(",APInvoiceLines.ExpenseCategoryId");
            queryBuilder.Append(",APInvoiceLines.ServiceDateFrom");
            queryBuilder.Append(",APInvoiceLines.ServiceDateTo");
            queryBuilder.Append(",APInvoiceLines.FixedAssetId");
            queryBuilder.Append(",APInvoiceLines.Deleted");
            queryBuilder.Append(",APInvoiceLines.CreatedAt");
            queryBuilder.Append(",APInvoiceLines.CreatedBy");
            queryBuilder.Append(",APInvoiceLines.LastModificationAt");
            queryBuilder.Append(",APInvoiceLines.LastModificationBy");
            queryBuilder.Append(" FROM APInvoiceLines");
            queryBuilder.Append(" INNER JOIN APInvoices ON APInvoices.Id = APInvoiceLines.InvoiceId");
            queryBuilder.Append(" INNER JOIN BusinessPartners ON BusinessPartners.Id = APInvoices.BusinessPartnerId");
            queryBuilder.Append(" WHERE BusinessPartners.TenantId = @tenantId");
            queryBuilder.Append(" AND Id = @invoiceLineId");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<APInvoiceLine>(queryBuilder.ToString(), parameters);
        }

        public async Task<APInvoiceLine> UpdateAPInvoiceLineAsync(APInvoiceLine invoiceLine)
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
                invoiceLine.ExpenseCategoryId,
                invoiceLine.FixedAssetId,
                invoiceLine.Deleted,
                invoiceLine.LastModificationAt,
                invoiceLine.LastModificationBy
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE APInvoiceLines");
            queryBuilder.Append(" SET Description = @Description");
            queryBuilder.Append(",Tax = @Tax");
            queryBuilder.Append(",Quantity = @Quantity");
            queryBuilder.Append(",UnitPrice = @UnitPrice");
            queryBuilder.Append(",TotalPrice = @TotalPrice");
            queryBuilder.Append(",ServiceDateFrom = @ServiceDateFrom");
            queryBuilder.Append(",ServiceDateTo = @ServiceDateTo");
            queryBuilder.Append(",ExpenseCategoryId = @ExpenseCategoryId");
            queryBuilder.Append(",FixedAssetId = @FixedAssetId");
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
            queryBuilder.Append(",INSERTED.ExpenseCategoryId");
            queryBuilder.Append(",INSERTED.FixedAssetId");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(" WHERE Id = @Id");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<APInvoiceLine>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeletedAPInvoiceLineAsync(Guid id, bool deleted, string userName)
        {
            var parameters = new
            {
                id,
                deleted,
                lastModificationBy = userName,
                lastModificationAt = DateTime.Now
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE APInvoiceLines");
            queryBuilder.Append(" SET Deleted = @deleted");
            queryBuilder.Append(",LastModificationBy = @lastModificationBy");
            queryBuilder.Append(",LastModificationAt = @lastModificationAt");
            queryBuilder.Append(" WHERE Id = @id");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
        }
    }
}
