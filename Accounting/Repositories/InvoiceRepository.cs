using AccountingAPI.Context;
using AccountingAPI.Models;
using Dapper;
using System.Text;

namespace AccountingAPI.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {

        private readonly IDapperContext _context;

        public InvoiceRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId)
        {
                var parameters = new
                {
                    invoiceId
                };

                StringBuilder queryBuilder = new();
                queryBuilder.Append("SELECT Id");
                queryBuilder.Append(",BusinessPartnerId");
                queryBuilder.Append(",RefNumber");
                queryBuilder.Append(",Date");
                queryBuilder.Append(",Currency");
                queryBuilder.Append(",GrossAmount");
                queryBuilder.Append(",NetAmount");
                queryBuilder.Append(",Deleted");
                queryBuilder.Append(",CreatedAt");
                queryBuilder.Append(",CreatedBy");
                queryBuilder.Append(",LastModificationAt");
                queryBuilder.Append(",LastModificationBy");
                queryBuilder.Append(" FROM Invoices ");
                queryBuilder.Append(" WHERE Id = @invoiceId");

                return await _context.Connection.QuerySingleOrDefaultAsync<Invoice?>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesAsync(bool includeDeleted = false)
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT ID");
            queryBuilder.Append(",BusinessPartnerId");
            queryBuilder.Append(",RefNumber");
            queryBuilder.Append(",Date");
            queryBuilder.Append(",Currency");
            queryBuilder.Append(",GrossAmount");
            queryBuilder.Append(",NetAmount");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM Invoices");
            if (includeDeleted == false) queryBuilder.Append(" WHERE Deleted = 0");

            return await _context.Connection.QueryAsync<Invoice>(queryBuilder.ToString());   
        }

        public async Task<Invoice> InsertInvoice(Invoice invoice)
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
            queryBuilder.Append("INSERT INTO Invoices (");
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

            return await _context.Connection.QuerySingleAsync<Invoice>(queryBuilder.ToString(), parameters);
        }

        public async Task<Invoice> UpdateInvoiceAsync(Invoice invoice)
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
            queryBuilder.Append("UPDATE Invoices");
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

            return await _context.Connection.QuerySingleAsync<Invoice>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeleteInvoiceAsync(Guid id, bool deleted)
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

            return await _context.Connection.ExecuteAsync(queryBuilder.ToString(), parameters);
        }
    }
}
