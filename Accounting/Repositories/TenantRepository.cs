using Dapper;
using System.Text;
using Accounting.Context;
using Accounting.Models;

namespace Accounting.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly DapperContext _context;
        public TenantRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Tenant> GetTenantByIdAsync(Guid tenantId)
        {
            var parameters = new
            {
                tenantId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM Tenants");
            queryBuilder.Append(" WHERE Id = @tenantId");

            using var connection = _context.CreateConnection();

            Tenant account = await connection.QuerySingleAsync<Tenant>(queryBuilder.ToString(), parameters);
            return account;
        }

        public async Task<IEnumerable<Tenant>> GetTenantsAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT * FROM Tenants");

            using var connection = _context.CreateConnection();

            IEnumerable<Tenant> tenants = await connection.QueryAsync<Tenant>(queryBuilder.ToString());
            return tenants;
        }

        public async Task<Guid> InsertTenantAsync(Tenant tenant)
        {
            var parameters = new
            {
                tenant.Name,
                tenant.LastModificationByUser,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Tenants (");
            queryBuilder.Append(" Name");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" )OUTPUT INSERTED.Id VALUES(");
            queryBuilder.Append(" @Name");
            queryBuilder.Append(" ,@LastModificationByUser");
            queryBuilder.Append(" )");

            using var connection = _context.CreateConnection();

            Guid tenantId = await connection.QuerySingleAsync<Guid>(queryBuilder.ToString(), parameters);
            return tenantId;
        }

        public async Task<int> SetDeleteTenantAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Tenants ");
            queryBuilder.Append("SET Deleted = @deleted ");
            queryBuilder.Append(" WHERE Id = @id ");

            using var connection = _context.CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
            return rowsAffected;
        }

        public async Task<int> UpdateTenantAsync(Tenant tenant)
        {
            var parameters = new
            {
                tenant.Id,
                tenant.Name,
                tenant.Deleted,
                tenant.LastModificationByUser,
                LastModificationDate = DateTime.Now,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Tenants ");
            queryBuilder.Append("SET Name = @Name ");
            queryBuilder.Append(" ,Deleted = @Deleted ");
            queryBuilder.Append(" ,LastModificationDate = @LastModificationDate ");
            queryBuilder.Append(" ,LastModificationByUser = @LastModificationByUser ");
            queryBuilder.Append(" WHERE Id = @Id ");

            using var connection = _context.CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
            return rowsAffected;
        }
    }
}
