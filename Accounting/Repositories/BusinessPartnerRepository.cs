using AccountingAPI.Context;
using AccountingAPI.Models;
using Dapper;
using System.Text;


namespace AccountingAPI.Repositories
{
    public class BusinessPartnerRepository : IBusinessPartnerRepository
    {
        private readonly IDapperContext _context;

        public BusinessPartnerRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<BusinessPartner> InsertBusinessPartnerAsync(BusinessPartner businessPartner)
        {
            var parameters = new
            {
                businessPartner.Name,
                businessPartner.VATNumber,
                businessPartner.TenantId,
                businessPartner.LastModificationBy,
                businessPartner.CreatedBy,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO BusinessPartners (");
            queryBuilder.Append("Name");
            queryBuilder.Append(",VATNumber");
            queryBuilder.Append(",TenantId");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(")OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.Name");
            queryBuilder.Append(",INSERTED.VATNumber");
            queryBuilder.Append(",INSERTED.TenantId");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append("@Name");
            queryBuilder.Append(",@VATNumber");
            queryBuilder.Append(",@TenantId");
            queryBuilder.Append(",@LastModificationBy");
            queryBuilder.Append(",@CreatedBy");
            queryBuilder.Append(")");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<BusinessPartner>(queryBuilder.ToString(), parameters);
        }

        public async Task<BusinessPartner?> GetBusinessPartnerByIdAsync(Guid businessPartnerId)
        {
            var parameters = new
            {
                businessPartnerId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM BusinessPartners");
            queryBuilder.Append(" WHERE Id = @businessPartnerId");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleOrDefaultAsync<BusinessPartner?>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<BusinessPartner>> GetBusinessPartnersAsync(bool includeDeleted = false)
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM BusinessPartners");
            if (includeDeleted == false) queryBuilder.Append(" WHERE Deleted = 0");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QueryAsync<BusinessPartner>(queryBuilder.ToString());
        }

        public async Task<int> SetDeletedBusinessPartnerAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE BusinessPartners");
            queryBuilder.Append(" SET Deleted = @deleted ");
            queryBuilder.Append(" WHERE Id = @id ");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<BusinessPartner> UpdateBusinessPartnerAsync(BusinessPartner businessPartner)
        {
            var parameters = new
            {
                businessPartner.Id,
                businessPartner.Name,
                businessPartner.VATNumber,
                businessPartner.LastModificationBy,
                businessPartner.LastModificationAt
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE BusinessPartners");
            queryBuilder.Append(" SET Name = @Name");
            queryBuilder.Append(",VATNumber = @VATNumber");
            queryBuilder.Append(",LastModificationBy = @LastModificationBy");
            queryBuilder.Append(",LastModificationAt = @LastModificationAt");
            queryBuilder.Append(" OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.Name");
            queryBuilder.Append(",INSERTED.VATNumber");
            queryBuilder.Append(",INSERTED.TenantId");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(" WHERE Id = @Id ");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<BusinessPartner>(queryBuilder.ToString(), parameters);
        }
    }
}
