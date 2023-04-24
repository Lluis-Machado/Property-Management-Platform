using Dapper;
using System.Text;
using Accounting.Context;
using Accounting.Models;

namespace Accounting.Repositories
{
    public class BusinessPartnerRepository : IBusinessPartnerRepository
    {
        private readonly DapperContext _context;

        public BusinessPartnerRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<BusinessPartner> GetBusinessPartnerByIdAsync(Guid businessPartnerId)
        {
            var parameters = new
            {
                businessPartnerId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM BusinessPartners");
            queryBuilder.Append(" WHERE Id = @businessPartnerId");

            using var connection = _context.CreateConnection();

            BusinessPartner businessPartner = await connection.QuerySingleAsync<BusinessPartner>(queryBuilder.ToString(), parameters);
            return businessPartner;
        }

        public async Task<IEnumerable<BusinessPartner>> GetBusinessPartnersAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM BusinessPartners");

            using var connection = _context.CreateConnection();

            IEnumerable<BusinessPartner> businessPartners = await connection.QueryAsync<BusinessPartner>(queryBuilder.ToString());
            return businessPartners;
        }

        public async Task<Guid> InsertBusinessPartnerAsync(BusinessPartner businessPartner)
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
            queryBuilder.Append(" )OUTPUT INSERTED.Id VALUES(");
            queryBuilder.Append(" @Name");
            queryBuilder.Append(" ,@VATNumber");
            queryBuilder.Append(" ,@AccountID");
            queryBuilder.Append(" ,@Type");
            queryBuilder.Append(" ,@LastModificationByUser");
            queryBuilder.Append(" )");

            using var connection = _context.CreateConnection();

            Guid businessPartnerId = await connection.QuerySingleAsync<Guid>(queryBuilder.ToString(), parameters);
            return businessPartnerId;
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
            queryBuilder.Append(" ,Type = @Type ");
            queryBuilder.Append(" ,LastModificationByUser = @LastModificationByUser ");
            queryBuilder.Append(" ,LastModificationDate = @LastModificationDate ");
            queryBuilder.Append(" WHERE Id = @Id ");

            using var connection = _context.CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
            return rowsAffected;
        }
    }
}
