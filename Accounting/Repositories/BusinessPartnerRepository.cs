using Accounting.Context;
using Accounting.Models;
using Dapper;
using System.Text;


namespace Accounting.Repositories
{
    public class BusinessPartnerRepository : IBusinessPartnerRepository
    {
        private readonly DapperContext _context;

        public BusinessPartnerRepository(DapperContext context)
        {
            _context = context;
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

            return await _context
                .CreateConnection()
                .QuerySingleOrDefaultAsync<BusinessPartner?>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<BusinessPartner>> GetBusinessPartnersAsync(bool includeDeleted)
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM BusinessPartners");
            if (includeDeleted == false) queryBuilder.Append("WHERE Deleted = 0");

            return await _context
                .CreateConnection()
                .QueryAsync<BusinessPartner>(queryBuilder.ToString());
        }

        public async Task<BusinessPartner> InsertBusinessPartnerAsync(BusinessPartner businessPartner)
        {
            var parameters = new
            {
                businessPartner.Name,
                businessPartner.VATNumber,
                businessPartner.AccountID,
                businessPartner.Type,
                businessPartner.LastModificationByUser,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO BusinessPartners (");
            queryBuilder.Append(" Name");
            queryBuilder.Append(" ,VATNumber");
            queryBuilder.Append(" ,AccountID");
            queryBuilder.Append(" ,Type");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" )OUTPUT INSERTED.Id");
            queryBuilder.Append(" ,INSERTED.Name");
            queryBuilder.Append(" ,INSERTED.VATNumber");
            queryBuilder.Append(" ,INSERTED.AccountId");
            queryBuilder.Append(" ,INSERTED.Type");
            queryBuilder.Append(" ,INSERTED.Deleted");
            queryBuilder.Append(" ,INSERTED.CreationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationByUser");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append(" @Name");
            queryBuilder.Append(" ,@VATNumber");
            queryBuilder.Append(" ,@AccountID");
            queryBuilder.Append(" ,@Type");
            queryBuilder.Append(" ,@LastModificationByUser");
            queryBuilder.Append(" )");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<BusinessPartner>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeleteBusinessPartnerAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE BusinessPartners ");
            queryBuilder.Append("SET Deleted = @deleted ");
            queryBuilder.Append(" WHERE Id = @id ");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<int> UpdateBusinessPartnerAsync(BusinessPartner businessPartner)
        {
            var parameters = new
            {
                businessPartner.Id,
                businessPartner.Name,
                businessPartner.Deleted,
                businessPartner.VATNumber,
                businessPartner.AccountID,
                businessPartner.Type,
                businessPartner.LastModificationByUser,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE BusinessPartners ");
            queryBuilder.Append("SET Name = @Name ");
            queryBuilder.Append(" ,Deleted = @Deleted ");
            queryBuilder.Append(" ,VATNumber = @VATNumber ");
            queryBuilder.Append(" ,AccountID = @AccountID ");
            queryBuilder.Append(" ,Name = @Name ");
            queryBuilder.Append(" ,LastModificationByUser = @LastModificationByUser ");
            queryBuilder.Append(" ,LastModificationDate = @LastModificationDate ");
            queryBuilder.Append(" WHERE Id = @Id ");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }
    }
}
