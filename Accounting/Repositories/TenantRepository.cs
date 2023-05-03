﻿using Dapper;
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
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" FROM Tenants");
            queryBuilder.Append(" WHERE Id = @tenantId");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<Tenant>(queryBuilder.ToString(), parameters);
        }

        public async Task<IEnumerable<Tenant>> GetTenantsAsync()
        {
            StringBuilder queryBuilder = new();
            queryBuilder.Append("SELECT Id");
            queryBuilder.Append(" ,Name");
            queryBuilder.Append(" ,Deleted");
            queryBuilder.Append(" ,CreationDate");
            queryBuilder.Append(" ,LastModificationDate");
            queryBuilder.Append(" ,LastModificationByUser");
            queryBuilder.Append(" FROM Tenants");

            return await _context
                .CreateConnection()
                .QueryAsync<Tenant>(queryBuilder.ToString());
        }

        public async Task<Tenant> InsertTenantAsync(Tenant tenant)
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
            queryBuilder.Append(" )OUTPUT INSERTED.Id");
            queryBuilder.Append(" ,INSERTED.Name");
            queryBuilder.Append(" ,INSERTED.Deleted");
            queryBuilder.Append(" ,INSERTED.CreationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationDate");
            queryBuilder.Append(" ,INSERTED.LastModificationByUser");
            queryBuilder.Append(" VALUES(");
            queryBuilder.Append(" @Name");
            queryBuilder.Append(" ,@LastModificationByUser");
            queryBuilder.Append(" )");

            return await _context
                .CreateConnection()
                .QuerySingleAsync<Tenant>(queryBuilder.ToString(), parameters);
        }

        public async Task<int> SetDeleteTenantAsync(Guid id, bool deleted)
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

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
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
            queryBuilder.Append("UPDATE Tenants");
            queryBuilder.Append(" SET Name = @Name");
            queryBuilder.Append(" ,Deleted = @Deleted");
            queryBuilder.Append(" ,LastModificationDate = @LastModificationDate");
            queryBuilder.Append(" ,LastModificationByUser = @LastModificationByUser");
            queryBuilder.Append(" WHERE Id = @Id");

            return await _context
                .CreateConnection()
                .ExecuteAsync(queryBuilder.ToString(), parameters);
        }
    }
}
