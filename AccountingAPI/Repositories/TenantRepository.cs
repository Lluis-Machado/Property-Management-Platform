﻿using AccountingAPI.Context;
using AccountingAPI.Models;
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

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<Tenant>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<Tenant>> GetTenantsAsync(bool includeDeleted = false)
        {
            var parameters = new
            {
                deleted = includeDeleted ? 1 : 0
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
            if (!includeDeleted) queryBuilder.Append(" WHERE Deleted = @deleted");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QueryAsync<Tenant>(queryBuilder.ToString(), parameters);
        }

        public async Task<Tenant?> GetTenantByIdAsync(Guid tenantId)
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

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<Tenant?>(queryBuilder.ToString(), parameters);
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

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.QuerySingleAsync<Tenant>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeletedTenantAsync(Guid id, bool deleted, string userName)
        {
            var parameters = new
            {
                id,
                deleted,
                lastModificationBy = userName,
                lastModificationAt = DateTime.Now,
            };

            StringBuilder queryBuilder = new();
            queryBuilder.Append("UPDATE Tenants");
            queryBuilder.Append(" SET Deleted = @deleted");
            queryBuilder.Append(",LastModificationAt = @lastModificationAt");
            queryBuilder.Append(",LastModificationBy = @lastModificationBy");
            queryBuilder.Append(" WHERE Id = @id");

            using var connection = _context.CreateConnection(); // Create a new connection
            return await connection.ExecuteAsync(queryBuilder.ToString(), parameters);
        }


    }
}
