using AccountingAPI.Context;
using AccountingAPI.Models;
using Dapper;
using System.Text;

namespace AccountingAPI.Repositories
{
    public class APInvoiceLineRepository : IAPInvoiceLineRepository
    {
        private readonly IDapperContext _context;
        public APInvoiceLineRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<APInvoiceLine>> GetAPInvoiceLinesAsync(bool includeDeleted)
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",InvoiceId");
            queryBuilder.Append(",Description");
            queryBuilder.Append(",Tax");
            queryBuilder.Append(",Quantity");
            queryBuilder.Append(",UnitPrice");
            queryBuilder.Append(",TotalPrice");
            queryBuilder.Append(",ExpenseCategoryId");
            queryBuilder.Append(",ServiceDateFrom");
            queryBuilder.Append(",ServiceDateTo");
            queryBuilder.Append(",FixedAssetId");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM InvoiceLines");
            if (includeDeleted == false) queryBuilder.Append(" WHERE Deleted = 0");


            return await _context.Connection.QueryAsync<APInvoiceLine>(queryBuilder.ToString());
        }

        public async Task<APInvoiceLine> GetAPInvoiceLineByIdAsync(Guid invoiceLineId)
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
            queryBuilder.Append(",ExpenseCategoryId");
            queryBuilder.Append(",ServiceDateFrom");
            queryBuilder.Append(",ServiceDateTo");
            queryBuilder.Append(",FixedAssetId");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM InvoiceLines");
            queryBuilder.Append(" WHERE Id = @invoiceLineId");

            return await _context.Connection.QuerySingleAsync<APInvoiceLine>(queryBuilder.ToString(), parameters);
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
            queryBuilder.Append("INSERT INTO InvoiceLines (");
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

            return await _context.Connection.QuerySingleAsync<APInvoiceLine>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeletedAPInvoiceLineAsync(Guid id, bool deleted)
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

            return await _context.Connection.ExecuteAsync(queryBuilder.ToString(), parameters);
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
            queryBuilder.Append("UPDATE InvoiceLines");
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

            return await _context.Connection.QuerySingleAsync<APInvoiceLine>(queryBuilder.ToString(), parameters);
        }
    }
}
