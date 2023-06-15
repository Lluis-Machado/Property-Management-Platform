using AccountingAPI.Context;
using AccountingAPI.Models;
using AccountingAPI.DTOs;
using Dapper;
using System.Text;

namespace AccountingAPI.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly IDapperContext _context;
        public TenantRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<Tenant> InsertTenantAsync(Tenant tenant)
        {
            var parameters = new
            {
                tenant.Name,
                tenant.CreatedBy,
                tenant.LastModificationBy,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("INSERT INTO Tenants (");
            queryBuilder.Append("Name");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(")OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.Name");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append("@Name");
            queryBuilder.Append(",@CreatedBy");
            queryBuilder.Append(",@LastModificationBy");
            queryBuilder.Append(")");

            return await _context.Connection.QuerySingleAsync<Tenant>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<Tenant>> GetTenantsAsync(bool includeDeleted = false)
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",Name");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM Tenants");
            if (!includeDeleted) queryBuilder.Append(" WHERE Deleted = 0");

            return await _context.Connection.QueryAsync<Tenant>(queryBuilder.ToString());
        }

        public async Task<Tenant> GetTenantByIdAsync(Guid tenantId)
        {
            var parameters = new
            {
                tenantId
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(",Name");
            queryBuilder.Append(",Deleted");
            queryBuilder.Append(",CreatedAt");
            queryBuilder.Append(",CreatedBy");
            queryBuilder.Append(",LastModificationAt");
            queryBuilder.Append(",LastModificationBy");
            queryBuilder.Append(" FROM Tenants");
            queryBuilder.Append(" WHERE Id = @tenantId");

            return await _context.Connection.QuerySingleAsync<Tenant>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeletedTenantAsync(Guid id, bool deleted)
        {
            var parameters = new
            {
                id,
                deleted
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Tenants");
            queryBuilder.Append(" SET Deleted = @deleted");
            queryBuilder.Append(" WHERE Id = @id");

            return await _context.Connection.ExecuteAsync(queryBuilder.ToString(), parameters);
        }

        public async Task<Tenant> UpdateTenantAsync(Tenant tenant)
        {
            var parameters = new
            {
                tenant.Id,
                tenant.Name,
                tenant.Deleted,
                tenant.LastModificationBy,
                tenant.LastModificationAt,
            };
            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Tenants");
            queryBuilder.Append(" SET Name = @Name");
            queryBuilder.Append(",Deleted = @Deleted");
            queryBuilder.Append(",LastModificationAt = @LastModificationAt");
            queryBuilder.Append(",LastModificationBy = @LastModificationBy");
            queryBuilder.Append(" OUTPUT INSERTED.Id");
            queryBuilder.Append(",INSERTED.Name");
            queryBuilder.Append(",INSERTED.Deleted");
            queryBuilder.Append(",INSERTED.CreatedAt");
            queryBuilder.Append(",INSERTED.CreatedBy");
            queryBuilder.Append(",INSERTED.LastModificationAt");
            queryBuilder.Append(",INSERTED.LastModificationBy");
            queryBuilder.Append(" WHERE Id = @Id");

            return await _context.Connection.QuerySingleAsync<Tenant>(queryBuilder.ToString(), parameters);
        }
    }
}
