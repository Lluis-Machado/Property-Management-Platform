using AccountingAPI.Context;
using AccountingAPI.Models;
using Dapper;
using System.Text;

namespace AccountingAPI.Repositories
{
    public class ARInvoiceRepository : IARInvoiceRepository
    {

        private readonly IDapperContext _context;

        public ARInvoiceRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<ARInvoice> InsertARInvoice(ARInvoice invoice)
        {
            var parameters = new
            {
                invoice.BusinessPartnerId,
                invoice.RefNumber,
                invoice.Date,
                invoice.Currency,
                invoice.GrossAmount,
                invoice.NetAmount,
                invoice.CreatedBy,
                invoice.LastModificationBy,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO ARInvoices (");
            queryBuilder.Append("BusinessPartnerId");
            queryBuilder.Append(",RefNumber");
            queryBuilder.Append(",Date");
            queryBuilder.Append(",Currency");
            queryBuilder.Append(",GrossAmount");
            queryBuilder.Append(",NetAmount");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(")OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.BusinessPartnerId");
            queryBuilder.Append(",INSERTED.RefNumber");
            queryBuilder.Append(",INSERTED.Date");
            queryBuilder.Append(",INSERTED.Currency");
            queryBuilder.Append(",INSERTED.GrossAmount");
            queryBuilder.Append(",INSERTED.NetAmount");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append("@BusinessPartnerId");
            queryBuilder.Append(",@RefNumber");
            queryBuilder.Append(",@Date");
            queryBuilder.Append(",@Currency");
            queryBuilder.Append(",@GrossAmount");
            queryBuilder.Append(",@NetAmount");
            queryBuilder.Append(",@CreatedBy");
            queryBuilder.Append(",@LastModificationBy");
            queryBuilder.Append(")");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<ARInvoice>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<ARInvoice>> GetARInvoicesAsync(Guid tenantId, bool includeDeleted = false)
        {
            var parameters = new
            {
                tenantId,
                deleted = includeDeleted? 1:0
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT ARInvoices.Id");
            queryBuilder.Append(",ARInvoices.BusinessPartnerId");
            queryBuilder.Append(",ARInvoices.RefNumber");
            queryBuilder.Append(",ARInvoices.Date");
            queryBuilder.Append(",ARInvoices.Currency");
            queryBuilder.Append(",ARInvoices.GrossAmount");
            queryBuilder.Append(",ARInvoices.NetAmount");
            queryBuilder.Append(",ARInvoices.Deleted");
            queryBuilder.Append(",ARInvoices.CreatedAt");
            queryBuilder.Append(",ARInvoices.CreatedBy");
            queryBuilder.Append(",ARInvoices.LastModificationAt");
            queryBuilder.Append(",ARInvoices.LastModificationBy");
            queryBuilder.Append(" FROM ARInvoices");
            queryBuilder.Append(" INNER JOIN BusinessPartners ON BusinessPartners.Id = ARInvoices.BusinessPartnerId");
            queryBuilder.Append(" WHERE BusinessPartners.TenantId = @tenantId");
            queryBuilder.Append(" AND Deleted = @deleted");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QueryAsync<ARInvoice>(queryBuilder.ToString(), parameters);
        }

        public async Task<ARInvoice?> GetARInvoiceByIdAsync(Guid tenantId, Guid invoiceId)
        {
            var parameters = new
            {
                tenantId,
                invoiceId
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT ARInvoices.Id");
            queryBuilder.Append(",ARInvoices.BusinessPartnerId");
            queryBuilder.Append(",ARInvoices.RefNumber");
            queryBuilder.Append(",ARInvoices.Date");
            queryBuilder.Append(",ARInvoices.Currency");
            queryBuilder.Append(",ARInvoices.GrossAmount");
            queryBuilder.Append(",ARInvoices.NetAmount");
            queryBuilder.Append(",ARInvoices.Deleted");
            queryBuilder.Append(",ARInvoices.CreatedAt");
            queryBuilder.Append(",ARInvoices.CreatedBy");
            queryBuilder.Append(",ARInvoices.LastModificationAt");
            queryBuilder.Append(",ARInvoices.LastModificationBy");
            queryBuilder.Append(" FROM ARInvoices ");
            queryBuilder.Append(" INNER JOIN BusinessPartners ON BusinessPartners.Id = ARInvoices.BusinessPartnerId");
            queryBuilder.Append(" WHERE BusinessPartners.TenantId = @tenantId");
            queryBuilder.Append(" AND Id = @invoiceId");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleOrDefaultAsync<ARInvoice?>(queryBuilder.ToString(), parameters);
        }

        public async Task<ARInvoice> UpdateARInvoiceAsync(ARInvoice invoice)
        {
            var parameters = new
            {
                invoice.Id,
                invoice.BusinessPartnerId,
                invoice.RefNumber,
                invoice.Date,
                invoice.Currency,
                invoice.GrossAmount,
                invoice.NetAmount,
                invoice.LastModificationAt,
                invoice.LastModificationBy,
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE ARInvoices");
            queryBuilder.Append(" SET BusinessPartnerId = @BusinessPartnerId");
            queryBuilder.Append(",RefNumber = @RefNumber");
            queryBuilder.Append(",Date = @Date");
            queryBuilder.Append(",Currency = @Currency");
            queryBuilder.Append(",GrossAmount = @GrossAmount");
            queryBuilder.Append(",NetAmount = @NetAmount");
            queryBuilder.Append(",LastModificationAt = @LastModificationAt");
            queryBuilder.Append(",LastModificationBy = @LastModificationBy");
            queryBuilder.Append(" OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.BusinessPartnerId");
            queryBuilder.Append(",INSERTED.RefNumber");
            queryBuilder.Append(",INSERTED.Date");
            queryBuilder.Append(",INSERTED.Currency");
            queryBuilder.Append(",INSERTED.GrossAmount");
            queryBuilder.Append(",INSERTED.NetAmount");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(" WHERE Id = @Id");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<ARInvoice>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeletedARInvoiceAsync(Guid id, bool deleted, string userName)
        {
            var parameters = new
            {
                id,
                deleted,
                lastModificationAt = DateTime.Now,
                lastModificationBy = userName,

            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE ARInvoices");
            queryBuilder.Append(" SET Deleted = @deleted");
            queryBuilder.Append(" SET LastModificationAt = @lastModificationAt");
            queryBuilder.Append(" SET LastModificationBy = @lastModificationBy");
            queryBuilder.Append(" WHERE Id = @id");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
        }
    }
}
