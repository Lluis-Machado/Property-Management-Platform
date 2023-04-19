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
            var parameters = new
            {
                invoiceId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM Invoices");
            queryBuilder.Append(" WHERE Id = @invoiceId");

            using var connection = _context.CreateConnection();

            Invoice invoice = await connection.QuerySingleAsync<Invoice>(queryBuilder.ToString(), parameters);
            return invoice;
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM Invoices");

            using var connection = _context.CreateConnection();

            IEnumerable<Invoice> invoices = await connection.QueryAsync<Invoice>(queryBuilder.ToString());
            return invoices;
        }

        public async Task<Guid> InsertInvoiceAsync(Invoice invoice)
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

            using var connection = _context.CreateConnection();

            Guid invoiceId = await connection.QuerySingleAsync<Guid>(queryBuilder.ToString(), parameters);
            return invoiceId;
        }

        public async Task<int> UpdateInvoiceAsync(Invoice invoice)
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
                invoice.Deleted,
                invoice.LastModificationByUser,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Invoices ");
            queryBuilder.Append("SET BusinessPartnerID = @BusinessPartnerID ");
            queryBuilder.Append(" ,RefNumber = @RefNumber ");
            queryBuilder.Append(" ,Date = @Date ");
            queryBuilder.Append(" ,Currency = @Currency ");
            queryBuilder.Append(" ,GrossAmount = @GrossAmount ");
            queryBuilder.Append(" ,NetAmount = @NetAmount ");
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
