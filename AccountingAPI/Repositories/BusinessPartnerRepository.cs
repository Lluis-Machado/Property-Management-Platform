using AccountingAPI.Context;
using AccountingAPI.Models;
using Dapper;
using System.Text;
using System.Text.RegularExpressions;

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

        public async Task<IEnumerable<BusinessPartner>> GetBusinessPartnersAsync(Guid? tenantId, bool includeDeleted = false)
        {
            var parameters = new
            {
                tenantId,
                deleted = includeDeleted ? 1 : 0
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",Name");
            queryBuilder.Append(",VATNumber");
            queryBuilder.Append(",TenantId");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM BusinessPartners");
            queryBuilder.Append(" WHERE 1 = 1");
            if (tenantId is not null) queryBuilder.Append(" AND tenantId = @tenantId");
            if (!includeDeleted) queryBuilder.Append(" AND Deleted = @deleted");
            queryBuilder.Append(" ORDER BY Name ASC");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QueryAsync<BusinessPartner>(queryBuilder.ToString(), parameters);
        }


        // This should only ever return one business partner. However, it returns a list because
        // this can help identify eventual duplicates in the database.
        public async Task<IEnumerable<BusinessPartner>> GetBusinessPartnerByCIFAsync(string CIF, bool includeDeleted = false)
        {

            // CIF cleanup before search: Remove hyphens (-) and country identification letters
            string cleanCIF = CIF.Replace("-", "");
            Regex reg = new Regex(@"^(es|de|it)", RegexOptions.IgnoreCase);
            cleanCIF = reg.Replace(cleanCIF, "");

            var parameters = new
            {
                cleanCIF = $"%{cleanCIF}%",
                deleted = includeDeleted ? 1 : 0
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",Name");
            queryBuilder.Append(",VATNumber");
            queryBuilder.Append(",TenantId");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM BusinessPartners");
            queryBuilder.Append(" WHERE VATNumber LIKE @cleanCIF");
            if (!includeDeleted) queryBuilder.Append(" AND Deleted = @deleted");
            queryBuilder.Append(" ORDER BY Name ASC");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QueryAsync<BusinessPartner>(queryBuilder.ToString(), parameters);
        }

        public async Task<BusinessPartner?> GetBusinessPartnerByIdAsync(Guid tenantId, Guid businessPartnerId)
        {
            var parameters = new
            {
                tenantId,
                businessPartnerId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",Name");
            queryBuilder.Append(",VATNumber");
            queryBuilder.Append(",TenantId");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM BusinessPartners");
            queryBuilder.Append(" WHERE tenantId = @tenantId");
            queryBuilder.Append(" AND Id = @businessPartnerId");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleOrDefaultAsync<BusinessPartner?>(queryBuilder.ToString(), parameters);
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

        public async Task<int> SetDeletedBusinessPartnerAsync(Guid id, bool deleted, string userName)
        {
            var parameters = new
            {
                id,
                deleted,
                lastModificationBy = userName,
                lastModificationAt = DateTime.Now,
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE BusinessPartners");
            queryBuilder.Append(" SET Deleted = @deleted ");
            queryBuilder.Append(",LastModificationBy = @lastModificationBy");
            queryBuilder.Append(",LastModificationAt = @lastModificationAt");
            queryBuilder.Append(" WHERE Id = @id ");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
        }


    }
}
